using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ElasticSearchSearchEnhancement.Models;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using Nest;

namespace QueryEditor.Services.ElasticSearch
{
    internal static class FilterConstructorService
    {
        public static QueryContainer ConstructFilterDefinitionFilter(FilterDefinition filter)
        {
            switch (filter.FilterType)
            {
                case FilterTypes.DateRange:
                    {
                        return ConstructDateRangeFilter(filter);
                    }

                case FilterTypes.Contains:
                    {
                        return ConstructTermsFilter(
                            filter.Field,
                            filter.Values.ToList());
                    }

                case FilterTypes.NotEqualTo:
                    {
                        return ConstructNotEqualToFilter(filter);
                    }

                case FilterTypes.LessThanOrEqualToXDayFromToday:
                    {
                        return ConstructLessThanOrEqualToXDayFromTodayFilter(filter);
                    }

                default:
                    {
                        if (filter.FindExactMatches)
                        {
                            if (filter.Fields.Any())
                            {
                                var queries = new List<QueryContainer>();

                                foreach (var field in filter.Fields)
                                {
                                    queries.Add(ConstructTermsFilter(
                                                field,
                                                filter.Values.ToList()));
                                }

                                return new BoolQuery
                                {
                                    Should = queries,
                                };
                            }

                            if (filter.Values.Any())
                            {
                                return filter.Values.Count > 1 ?
                                        ConstructTermsFilter(
                                            filter.Field,
                                            filter.Values.ToList())
                                            : ConstructSimpleQueryStringFilter(
                                                filter.Field,
                                                filter.Values.ToList()[0].ToString());
                            }
                            else
                            {
                                return ConstructSimpleQueryStringFilter(
                                                filter.Field,
                                                filter.Value.ToString());
                            }
                        }
                        else
                        {
                            if (filter.Values.Any())
                            {
                                var queries = new List<QueryContainer>();

                                foreach (var filterValue in filter.Values)
                                {
                                    queries.Add(
                                        ConstructQueryStringFilter(
                                            filter.Fields.Any() ? filter.Fields.ToList() : new List<string> { filter.Field },
                                            filterValue));
                                }

                                return new BoolQuery
                                {
                                    Should = queries,
                                };
                            }
                            else
                            {
                                var queries = new List<QueryContainer> {
                                    ConstructQueryStringFilter(
                                            filter.Fields.Any() ? filter.Fields.ToList() : new List<string> { filter.Field },
                                            filter.Value.ToString())
                                };

                                return new BoolQuery
                                {
                                    Should = queries,
                                };
                            }
                        }
                    }
            }
        }

        internal static QueryContainer ConstructNotEqualToFilter(FilterDefinition filter)
        {
            return new BoolQuery
            {
                MustNot = new List<QueryContainer>
                        {
                            new QueryStringQuery
                            {
                                Fields = new Field(filter.Field),
                                Query = filter.Values.FirstOrDefault(),
                            },
                        },
            };
        }

        internal static QueryContainer ConstructLessThanOrEqualToXDayFromTodayFilter(FilterDefinition filter)
        {
            return new DateRangeQuery
            {
                Field = new Field(filter.Field),
                Format = "MM/dd/yyyy",
                LessThanOrEqualTo = "12/31/9999",
                GreaterThanOrEqualTo = DateTime.Today.AddDays(Convert.ToInt32(filter.Value)).ToString(@"MM\/dd\/yyyy"),
            };
            ////    new BoolQuery
            ////{
            ////    Must = new List<QueryContainer>
            ////            {
            ////                new DateRangeQuery
            ////                {
            ////                    Field = new Field(filter.Field),
            ////                    Format = "MM/dd/yyyy",
            ////                    LessThanOrEqualTo = "12/31/9999",
            ////                    GreaterThanOrEqualTo = DateTime.Today.AddDays(Convert.ToInt32(filter.Value)).ToString("MM/dd/yyyy"),
            ////                },
            ////            },
            ////};
        }

        internal static QueryContainer ConstructDateRangeFilter(FilterDefinition filter)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            var greaterThanOrEqualToDate = DateTime.Parse(filter.Values.ElementAt(0)).ToString("d", culture);
            var lessThanOrEqualToDate = DateTime.Parse(filter.Values.ElementAt(1)).ToString("d", culture);

            return new DateRangeQuery
            {
                Field = new Field(filter.Field),
                Format = "MM/dd/yyyy",
                GreaterThanOrEqualTo = greaterThanOrEqualToDate,
                LessThanOrEqualTo = lessThanOrEqualToDate,
            };
        }

        internal static QueryContainer ConstructTermsFilter(string field, List<string> filterValues)
        {
            return new TermsQuery()
            {
                Field = new Field(field),
                Terms = filterValues,
            };
        }

        internal static QueryContainer ConstructSimpleQueryStringFilter(string field, string filterValue)
        {
            return new SimpleQueryStringQuery()
            {
                Fields = new Field(field),
                Query = filterValue,
                DefaultOperator = Operator.And,
            };
        }

        internal static QueryContainer ConstructQueryStringFilter(List<string> fields, string filterValue)
        {
            var queryStringQuery = new QueryStringQuery()
            {
                Query = "*" + filterValue + "*",
                DefaultOperator = Operator.And,
            };
            foreach (var field in fields)
            {
                if (queryStringQuery.Fields == null)
                {
                    queryStringQuery.Fields = new Field(field);
                    continue;
                }

                queryStringQuery.Fields.And(new Field(field));
            }

            return queryStringQuery;
        }

        internal static QueryContainer ConstructNestedQuery(
            string path, QueryContainer query, Fields fields, int from = 0, int pageSize = 100)
        {
            return new NestedQuery
            {
                Path = path,
                Query = query,
                InnerHits = new InnerHits
                {
                    From = from,
                    Size = pageSize,
                    Source = new SourceFilter
                    {
                        Includes = fields
                    }
                },
            };
        }
    }
}
