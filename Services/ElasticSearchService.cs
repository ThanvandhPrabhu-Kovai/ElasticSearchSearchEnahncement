﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nest;
using Elasticsearch.Net;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using ElasticSearchSearchEnhancement.Models;
using SearchReq = ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates.SearchRequest;
using DateRan = ElasticSearchSearchEnhancement.Models.Search.DateRange;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using Kovai.Churn360.Customers.Core.Models;
using System.Globalization;

namespace QueryEditor.Services.ElasticSearch
{
    public class ElasticSearchService
    {
        public static IProperties IndexMapping { get; set; }

        private const string ElasticSearchClusterUri = "http://localhost:9200";

        private const string IndexNameCustomers = "18455014ef7f413c8be61acca225d24e-customers-test";

        private static ConnectionSettings GetElasticSearchConnectionSettings(StaticConnectionPool connectionPool) => new ConnectionSettings(connectionPool)
                                                  .DisableDirectStreaming()
                                                  .BasicAuthentication(username: "elastic", password: "pass@123")
                                                  .DefaultIndex(IndexNameCustomers)
                                                  .DisablePing();

        public static ElasticClient GetElasticClient()
        {
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;

            var nodes = new Uri[] { new Uri(ElasticSearchClusterUri) };

            connectionPool = new StaticConnectionPool(nodes);
            elasticClient = new ElasticClient(GetElasticSearchConnectionSettings(connectionPool));

            return elasticClient;
        }

        public virtual async Task<IEnumerable<CustomerSearch>> SearchThroughNestedObjectsAsync(
           ElasticClient elasticClient,
           SearchReq searchRequest,
           string nestedPath)
        {
            if (string.IsNullOrEmpty(nestedPath))
            {
                throw new ArgumentNullException(nestedPath);
            }

            var request = ConstructSearchRequest(elasticClient, searchRequest);

            var searchResponse = await elasticClient
                .SearchAsync<CustomerSearch>(request)
                .ConfigureAwait(false);

            var searchResultsWithOnlyChildren = new List<object>();

            var customers = new List<CustomerSearch>();

            var hits = searchResponse.Hits;

            foreach (var hit in hits)
            {
                var parent = hit.Source;
                var children = new List<object>();

                // to do check this code
                foreach (var innerHit in hit.InnerHits)
                {
                    if (innerHit.Key == nestedPath)
                    {
                        var matches = innerHit.Value.Hits.Hits.Select(document => document.Source.As<object>());
                        children.AddRange(matches);
                    }
                }

                customers.Add(parent);
                searchResultsWithOnlyChildren.AddRange(children);
            }

            return customers;
        }

        public static SearchRequest<CustomerSearch> ConstructSearchRequest(
            ElasticClient elasticClient,
            SearchReq searchRequest)
        {
            var elasticSearchRequest = new SearchRequest<CustomerSearch>
            {
                //// TODO: Handle the scenario where there is an int overflow.
                From = searchRequest.From.HasValue ? (int)searchRequest.From
                    : (int)((searchRequest.PageNumber - 1) * searchRequest.PageSize),
                Size = searchRequest.PageSize == 0 ? null : (int?)searchRequest.PageSize,
            };

            AddFieldsToInclude(searchRequest, elasticSearchRequest);

            elasticSearchRequest.Query = ConstructSearchQuery(
                searchRequest.Query,
                searchRequest.Fields,
                searchRequest.Filters);

            AddSorting(searchRequest, elasticSearchRequest);

            string requestJson = string.Empty;

            using (var stream = new MemoryStream())
            {
                elasticClient.RequestResponseSerializer.Serialize<SearchRequest<CustomerSearch>>(elasticSearchRequest, stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream);
                requestJson = reader.ReadToEnd();
            }

            return elasticSearchRequest;
        }

        private static void AddSorting(SearchReq searchRequest, Nest.SearchRequest elasticSearchRequest)
        {
            var sorts = new List<ISort>();

            foreach (var sortDefinition in searchRequest.SortBy)
            {
                if (sortDefinition.SortedValues.Any())
                {
                    AddLookupSort(sorts, sortDefinition);

                    continue;
                }

                AddSimpleSort(sorts, sortDefinition);
            }

            elasticSearchRequest.Sort = sorts;
        }

        private static void AddLookupSort(List<ISort> sorts, SortByDefinition sortDefinition)
        {
            var scriptParams = new Dictionary<string, object>
                {
                    { "ids", sortDefinition.SortedValues },
                };

            sorts.Add(new ScriptSort
            {
                Type = "number",
                Script = new InlineScript($"int idsCount = params.ids.size();" +
                    $"def id = doc['{sortDefinition.Name}.keyword'].size() > 0 ?" +
                    $" doc['{sortDefinition.Name}.keyword'].value : null;" +
                    $" int foundIdx = params.ids.indexOf(id);return foundIdx > -1 ?" +
                    $" foundIdx: " +
                    (sortDefinition.SortType == SortType.Desc ? "idsCount + 1;" : "-1"))
                {
                    Lang = "painless",
                    Params = scriptParams,
                },
            });
        }

        private static void AddSimpleSort(List<ISort> sorts, SortByDefinition sortDefinition)
        {
            sorts.Add(new FieldSort
            {
                Field = sortDefinition.Name + ".keyword",
                Order = sortDefinition.SortType == SortType.Desc ?
                                        SortOrder.Descending : SortOrder.Ascending,
                Missing = sortDefinition.SortType == SortType.Desc ? "_last" : "_first",
            });
        }

        private static void AddFieldsToInclude(SearchReq searchRequest, Nest.SearchRequest elasticSearchRequest)
        {
            if (searchRequest.FieldsToReturn == null
                || searchRequest.FieldsToReturn.Any() == false)
            {
                return;
            }

            Fields fields = null;

            foreach (var field in searchRequest.FieldsToReturn)
            {
                if (fields == null)
                {
                    fields = new Field(field);
                    continue;
                }

                fields.And(new Field(field));
            }

            elasticSearchRequest.Source = new SourceFilter
            {
                Includes = fields,
            };
        }

        private BoolQuery ConstructSearchQueryForFilterGroups(
          string searchText,
          IEnumerable<string> searchFields,
          IEnumerable<FilterGroup> filterGroups)
        {
            var filtersWithMustCondition = new List<QueryContainer>();
            var filtersWithShouldCondition = new List<QueryContainer>();
            var filtersWithMustNotCondition = new List<QueryContainer>();

            if (searchFields != null && searchFields.Any())
            {
                filtersWithMustCondition.AddRange(
                    ConstructSearchFieldsFilter(
                        searchText,
                        searchFields));
            }

            foreach (var filterGroup in filterGroups)
            {
                if (filterGroup.Filters == null || !filterGroup.Filters.Any())
                {
                    continue;
                }

                var filterDefinitions = filterGroup.Filters;

                if (filterDefinitions != null && filterDefinitions.Any())
                {
                    switch (filterGroup.LogicalOperator)
                    {
                        case LogicalOperator.ANDNOT:
                            ////filtersWithMustNotCondition.AddRange(
                            ////    ConstructFilterDefinitionsFilter(filterDefinitions));
                            break;
                        case LogicalOperator.OR:
                            ////filtersWithShouldCondition.AddRange(
                            ////    ConstructFilterDefinitionsFilter(filterDefinitions));
                            break;
                        default:
                            ////filtersWithMustCondition.AddRange(
                            ////    ConstructFilterDefinitionsFilter(filterDefinitions));
                            break;
                    }
                }
            }

            return new BoolQuery
            {
                MustNot = filtersWithMustNotCondition,
                Should = filtersWithShouldCondition,
                Must = filtersWithMustCondition,
            };
        }

        private static List<QueryContainer> ConstructSearchFieldsFilter(string searchText, IEnumerable<string> searchFields)
        {
            var queries = new List<QueryContainer>();
            ////var nestedFields = new Dictionary<string, List<string>>();
            ////var directFields = new List<string>();

            ////if (searchFields != null && searchFields.Any())
            ////{
            ////    var seperatedFields = SeperateSearchFieldsAsNestedOrDirect(searchFields);
            ////    nestedFields = seperatedFields.nestedFields;
            ////    directFields = seperatedFields.directFields;
            ////}

            ////if (!string.IsNullOrWhiteSpace(searchText)
            ////         && directFields != null
            ////         && directFields.Any())
            ////{
            ////    queries.Add(
            ////        FilterConstructorService.ConstructQueryStringFilter(
            ////            directFields,
            ////            searchText));
            ////}

            ////if (!string.IsNullOrWhiteSpace(searchText)
            ////         && nestedFields != null
            ////         && nestedFields.Any())
            ////{
            ////    nestedFields.Keys.ToList().ForEach((nestedPath) =>
            ////    {
            ////        List<string> searchFields = nestedFields[nestedPath];
            ////        queries.Add(
            ////            FilterConstructorService.ConstructNestedQuery(
            ////                nestedPath,
            ////                FilterConstructorService.ConstructQueryStringFilter(searchFields, searchText)));
            ////    });
            ////}

            return queries;
        }

        public class NestedItem
        {
            public NestedItem()
            {
                this.NestedChildren = new List<NestedItem>();
                this.Queries = new List<QueryContainer>();
            }

            public string Path { get; set; }

            public ICollection<NestedItem> NestedChildren { get; set; }

            public ICollection<QueryContainer> Queries { get; set; }
        }

        public static IProperties GetMapping(ElasticClient elastic)
        {
            if (elastic == null)
            {
                return null;
            }

            var mappings = elastic.Indices.GetMapping<CustomerSearch>();
            if (!mappings.IsValid)
            {
                return null;
            }

            var mapping = mappings.Indices["18455014ef7f413c8be61acca225d24e-customers-test"].Mappings;
            if (mapping == null)
            {
                return null;
            }

            return mapping.Properties;
        }

        private static NestedItem GetParent(
            NestedItem nestedItem,
            FilterDefinition filterDefinition)
        {
            var path = filterDefinition.Field;
            var parts = path.Split('.').ToList();
            string previousParentPath = null;
            string currentPath = null;
            var parent = nestedItem;
            var nestedParents = nestedItem.NestedChildren;

            IProperties properties = IndexMapping;

            foreach (var part in parts)
            {
                previousParentPath = currentPath;
                currentPath = (currentPath != null ? currentPath + "." : currentPath) + part;

                var propertyInfo = properties.ContainsKey(part)
                    ? properties[part]
                    : null;

                if (propertyInfo == null)
                {
                    break;
                }

                if (propertyInfo.Type == "nested")
                {
                    parent = nestedParents.FirstOrDefault(_ => _.Path == currentPath);

                    if (parent == null)
                    {
                        parent = new NestedItem
                        {
                            Path = currentPath,
                        };

                        nestedParents.Add(parent);
                    }

                    nestedParents = parent.NestedChildren;
                }

                properties = (Properties)propertyInfo
                    .GetType()
                    .GetProperty("Properties")
                    ?.GetValue(propertyInfo, null);

                if (properties == null)
                {
                    break;
                }
            }

            return parent;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> property)
        {
            List<string> stack = new List<string>();
            LambdaExpression lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(lambda.Body);
            }

            var current = memberExpression;

            while (current != null)
            {
                stack.Add(((PropertyInfo)current.Member).Name);

                if (current.Expression is MemberExpression)
                {
                    current = (MemberExpression)current.Expression;
                }
                else
                {
                    current = null;
                }
            }
            stack.Reverse();
            return string.Join('.', stack);
        }

        public static async Task<List<CustomerSearch>> SearchAsync(ElasticClient elasticClient)
        {
            var request = new Nest.SearchRequest<object>
            {
                Query = new BoolQuery
                {
                    Must = new List<QueryContainer> {
                            new TermsQuery {
                                Field = "id",
                                Terms = new List<object>{
                                    1, 2, 3, 4, 501
                                }
                            },
                            new NestedQuery{
                                Path = "contacts",
                                InnerHits = new InnerHits {
                                    From = 0,
                                    Size = 10
                                },
                                Query = new BoolQuery{
                                    Must = new List<QueryContainer>{
                                        new QueryStringQuery{
                                            Fields = new Field("contacts.firstName"),
                                            Query = "*l*",
                                        }
                                    }
                                }
                            },
                            new NestedQuery{
                                Path = "opportunities",
                                InnerHits = new InnerHits {
                                    From = 0,
                                    Size = 10
                                },
                                Query = new BoolQuery{
                                    Must = new List<QueryContainer>{
                                        new QueryStringQuery{
                                            Fields = new Field("opportunities.name"),
                                            Query = "*july*",
                                        }
                                    }
                                }
                            }
                        },
                },
            };
            string requestJson = string.Empty;

            using (var stream = new MemoryStream())
            {
                elasticClient.RequestResponseSerializer.Serialize<SearchRequest<object>>(request, stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream);
                requestJson = reader.ReadToEnd();
            }

            var response = await elasticClient.SearchAsync<object>(request);

            var parents = response.Hits.Select(_ => _.Source).ToList();
            var parentsWithFilteredChildren = new List<CustomerSearch> { };

            var sourceType = response.Hits.GetType().GetGenericArguments().Single();

            foreach (IHit<object> hit in response.Hits.ToList())
            {
                var source = hit.Source as Dictionary<string, object>;
                var customer = new CustomerSearch();
                source.Keys.ToList().ForEach((key) =>
                {
                    customer.SetProperty(key, source[key]);
                });

                var currentInnerHits = hit.InnerHits;
                string nestedPath = string.Empty;

                do
                {
                    IReadOnlyDictionary<string, InnerHitsResult> nestedInnerHits = null;

                    currentInnerHits.ToList().ForEach((innerHit) =>
                    {
                        nestedPath = innerHit.Key;

                        var matches = innerHit.Value.Hits.Hits.Select(_ =>
                        {
                            nestedInnerHits = _.InnerHits;
                            return _.Source.As<object>();
                        }).ToList<object>();

                        customer[nestedPath] = matches;
                    });

                    currentInnerHits = nestedInnerHits;

                } while (currentInnerHits != null);

                parentsWithFilteredChildren.Add(customer);
            }

            return parentsWithFilteredChildren;
        }

        public static FilterDefinition ConstructFilterDefinitionForCustomerByIds(IEnumerable<int> ids)
        {
            var customerIds = new List<string>();
            var s = ids.Select(_ => _.ToString()).ToList();
            return new FilterDefinition
            {
                Field = "id",
                LogicalOperator = LogicalOperator.AND,
                Values = ids.Select(_ => _.ToString()).ToList(),
                FindExactMatches = true,
            };
        }

        public static FilterDefinition ConstructFilterDefinitionForDateRange(DateRan dateRange)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            return new FilterDefinition
            {
                Field = "contacts.campaigns.respondedOn",
                LogicalOperator = LogicalOperator.AND,
                FilterType = FilterTypes.DateRange,
                Values = new List<string>
                {
                    dateRange.StartDate.ToString("d", culture),
                    dateRange.EndDate.ToString("d", culture),
                },
                FindExactMatches = true,
            };
        }

        public static FilterDefinition ConstructFilterDefinitionForSurveyById(int id)
        {
            return new FilterDefinition
            {
                Field = "contacts.campaigns.surveyTypeId",
                LogicalOperator = LogicalOperator.AND,
                Value = id,
                FindExactMatches = true,
            };
        }

        public static FilterDefinition ConstructFilterDefinitionForResponderSentiment()
        {
            return new FilterDefinition
            {
                Field = "contacts.campaigns.response.responderSentiment",
                LogicalOperator = LogicalOperator.AND,
                Value = "passives",
                FindExactMatches = true,
            };
        }

        public static BoolQuery ConstructSearchQuery(
            string searchText,
            IEnumerable<string> searchFields,
            IEnumerable<IFilterDefinition> filterDefinitions)
        {
            var filters = new List<QueryContainer>();

            if (searchFields != null && searchFields.Any())
            {
                filters.AddRange(
                    ConstructSearchFieldsFilter(
                        searchText,
                        searchFields));
            }

            filters.AddRange(AddFilters(filterDefinitions, LogicalOperator.AND));

            var query = new BoolQuery
            {
                Must = filters,
            };

            return query;
        }

        public class NestedQueryParams
        {
            public string Path;
            public List<QueryContainer> Queries;
            public List<NestedQueryParams> NestedParams;
        }

        public static ICollection<QueryContainer> AddFilters(
            IEnumerable<IFilterDefinition> filterDefinitions,
            LogicalOperator logicalOperator)
        {
            var queries = new List<QueryContainer>();

            if (filterDefinitions == null || !filterDefinitions.Any())
            {
                return queries;
            }

            var rootQuery = new NestedItem();

            foreach (var filterDefinition in filterDefinitions)
            {
                NestedItem parent;

                if (filterDefinition is FilterDefinition)
                {
                    parent = GetParent(rootQuery, filterDefinition as FilterDefinition);
                }
                else
                {
                    parent = rootQuery;
                }

                parent.Queries.Add(GetQuery(filterDefinition));
            }

            queries.AddRange(rootQuery.Queries);

            var parentsAndQueries = new List<NestedQueryParams> { };

            foreach (var child in rootQuery.NestedChildren)
            {
                var queriesPartedByParents = GetQueriesSeparatedByParents(
                    logicalOperator, queries,
                    child,
                    new List<NestedQueryParams> { });

                OrganizeQueries(parentsAndQueries, queriesPartedByParents);
            }

            var nestedQueries = new List<QueryContainer> { };

            foreach (var querySet in parentsAndQueries)
            {
                if (querySet.NestedParams.Any())
                {
                    var queriesOfCurrentParent = GetQueries(querySet);
                    queriesOfCurrentParent.AddRange(querySet.Queries);
                    nestedQueries.Add(
                            FilterConstructorService.ConstructNestedQuery(querySet.Path, GetBoolQuery(LogicalOperator.AND, queriesOfCurrentParent))
                        );
                }
                else
                {
                    nestedQueries.AddRange(GetQueries(querySet));
                }
            }

            queries.AddRange(nestedQueries);

            return queries;
        }

        private static void OrganizeQueries(List<NestedQueryParams> parentsAndQueries, List<NestedQueryParams> queriesPartedByParents)
        {
            queriesPartedByParents.ToList().ForEach((_) =>
            {
                var parts = _.Path.Split('.');
                var currentPath = string.Empty;

                foreach (var part in parts)
                {
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + $".{part}";

                    var parent = parentsAndQueries.Find(nestedParam => nestedParam.Path == currentPath);

                    if (parent != null)
                    {
                        var indexOfParent = parentsAndQueries.IndexOf(parent);
                        parentsAndQueries[indexOfParent].NestedParams.Add(_);
                        break;
                    }
                    else
                    {
                        parentsAndQueries.Add(_);
                    }
                }
            });
        }

        public static List<QueryContainer> GetQueries(
            NestedQueryParams nestedParam,
            List<QueryContainer> queries = null)
        {
            if (queries == null || queries.Any() == false)
            {
                queries = new List<QueryContainer> { };
            }

            if (nestedParam.NestedParams.Any() == false)
            {
                var boolQuery = GetBoolQuery(LogicalOperator.AND, nestedParam.Queries);
                queries.Add(FilterConstructorService.ConstructNestedQuery(
                    nestedParam.Path,
                    boolQuery));
            }
            else
            {
                foreach (var child in nestedParam.NestedParams)
                {
                    queries.AddRange(GetQueries(child, queries));
                }
            }
            return queries;
        }

        public static List<NestedQueryParams> GetQueriesSeparatedByParents(
            LogicalOperator logicalOperator,
            List<QueryContainer> queries,
            NestedItem child,
            List<NestedQueryParams> nestedQueryParams)
        {
            var containerForChildren = queries;
            //var children = new List<NestedItem> { };

            if (child.Queries.Any() == true)
            {
                //var boolQuery = FilterConstructorService.ConstructNestedQuery(
                //    child.Path,
                //    GetBoolQuery(logicalOperator, child.Queries));
                nestedQueryParams.Add(
                    new NestedQueryParams
                    {
                        Path = child.Path,
                        Queries = child.Queries.ToList(),
                        NestedParams = new List<NestedQueryParams> { },
                    });
                //queries.Add(boolQuery);
            }

            foreach (var grandChild in child.NestedChildren)
            {
                GetQueriesSeparatedByParents(
                    logicalOperator,
                    containerForChildren,
                    grandChild,
                    nestedQueryParams);
            }

            return nestedQueryParams;
        }

        //public static void GetNestedQueries(
        //    LogicalOperator logicalOperator,
        //    List<QueryContainer> queries,
        //    NestedItem child)
        //{
        //    var containerForChildren = queries;

        //    if (child.Queries.Any() == true)
        //    {
        //        var boolQuery = FilterConstructorService.ConstructNestedQuery(
        //            child.Path,
        //            GetBoolQuery(logicalOperator, child.Queries));

        //        queries.Add(boolQuery);
        //    }

        //    foreach (var grandChild in child.NestedChildren)
        //    {
        //        GetNestedQueries(
        //            logicalOperator,
        //            containerForChildren,
        //            grandChild);
        //    }
        //}

        public static QueryContainer GetQuery(IFilterDefinition filterDefinition)
        {
            switch (filterDefinition)
            {
                case FilterGroup filterGroup:
                    {
                        var filters = AddFilters(
                            filterGroup.Filters,
                            filterGroup.LogicalOperator);

                        return GetBoolQuery(filterGroup.LogicalOperator, filters);
                    }

                case FilterDefinition childFilterDefinition:
                    {
                        return FilterConstructorService.ConstructFilterDefinitionFilter(childFilterDefinition);
                    }
            }

            return null;
        }

        public static BoolQuery GetBoolQuery(
            LogicalOperator logicalOperator,
            ICollection<QueryContainer> filters)
        {
            var boolQuery = new BoolQuery();

            switch (logicalOperator)
            {
                case LogicalOperator.ANDNOT:
                    boolQuery.MustNot = filters;
                    break;
                case LogicalOperator.OR:
                    boolQuery.Should = filters;
                    break;
                default:
                    boolQuery.Must = filters;
                    break;
            }

            return boolQuery;
        }

    }
}
