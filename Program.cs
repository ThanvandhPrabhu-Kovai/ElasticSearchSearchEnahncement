using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchSearchEnhancement.Models;
using Nest;
using QueryEditor.Services.ElasticSearch;
using SearchReq = ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates.SearchRequest;
using DateRan = ElasticSearchSearchEnhancement.Models.Search.DateRange;
using System;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using ElasticSearchSearchEnhancement.Models.Search;
using System.Linq.Expressions;
using System.Reflection;
using Kovai.Churn360.Customers.Core.Models;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace QueryEditor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ElasticClient elasticClient = ElasticSearchService.GetElasticClient();
            ElasticSearchService elasticSearch = new ElasticSearchService();
            ElasticSearchService.IndexMapping = ElasticSearchService.GetMapping(elasticClient);

            var customerCampaignResponsePath = GetPropertyName<CustomerSearch>(
                    customer => customer.Contacts) + "." + GetPropertyName<CustomerContact>(
                    contact => contact.Campaigns) + "." + GetPropertyName<CustomerContactCampaign>(
                    campaign => campaign.RespondedOn);

            var usageFilterDefinition = new FilterGroup
            {
                LogicalOperator = LogicalOperator.AND,
                Filters = new List<IFilterDefinition>
                {
                    new FilterDefinition
                    {
                        Field = "usage.features.name",
                        FilterType = FilterTypes.Equals,
                        Value = "campaigns"
                    },
                    new FilterDefinition
                    {
                        Field = "usage.features.lastRecorded",
                        FilterType = FilterTypes.LessThanOrEqualToXDayFromToday,
                        Value = 10
                    },
                },
            };

            var searchRequest = new SearchReq
            {
                Filters = new List<FilterDefinition>
                {
                    new FilterDefinition
                    {
                        Field = "usage.features.name",
                        FilterType = FilterTypes.Equals,
                        FindExactMatches = true,
                        Value = "campaigns"
                    },
                    new FilterDefinition
                    {
                        Field = "usage.features.lastRecorded",
                        FilterType = FilterTypes.LessThanOrEqualToXDayFromToday,
                        Value = 10
                    },
                },
                PageSize = 0,
            };

            var getLookupSubsctitutedConfig = GetOptionsSubstitutedWithLookupOptions(
                @"{
                ""features"": {
                    ""name"": ""Features"",
                    ""value"": ""features"",
                    ""type"": ""multiPart"",
                    ""filterPartConfigs"": [
                        {
                            ""identifier"": ""feature.query"",
                            ""type"": ""features:lookup:1"",
                            ""options"": ""featureId:lookupOptions:1""
                        },
                        {
                            ""identifier"": ""subQuery"",
                            ""type"": ""dropDown"",
                            ""options"": [
                                {
                                    ""label"": ""was used"",
                                    ""value"": ""wasUsed""
                                },
                                {
                                    ""label"": ""was never used"",
                                    ""value"": ""wasNeverUsed""
                                },
                                {
                                    ""label"": ""was not used in the last"",
                                    ""value"": ""wasNotUsedInTheLastXDays"",
                                    ""children"": [
                                        {
                                            ""identifier"": ""feature.query"",
                                            ""type"": ""number""
                                        },
                                        {
                                            ""type"": ""text"",
                                            ""text"": ""days""
                                        }
                                    ]
                                },
                                {
                                    ""label"": ""was used in the last"",
                                    ""value"": ""wasUsedInTheLastXDays"",
                                    ""children"": [
                                        {
                                            ""identifier"": ""feature.query"",
                                            ""type"": ""number""
                                        },
                                        {
                                            ""type"": ""text"",
                                            ""text"": ""days""
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            }");

            var convertedRequest = ElasticSearchService.ConstructSearchRequest(
                            elasticClient,
                            searchRequest);

            var deserialised = JsonConvert.DeserializeObject<List<FilterDefinition>>("[{\"field\":\"customFields.texts.id\",\"type\":\"filterDefinition\",\"logicalOperator\":1,\"filterType\":6,\"value\":\"3FA25452-6BD7-458D-818B-A303CA4C8E88\",\"values\":\"\", \"findExactMatches\":\"true\"},{\"field\":\"customFields.texts.value\",\"type\":\"filterDefinition\",\"logicalOperator\":1,\"filterType\":3,\"value\":\"Space\",\"values\":\"\"}]");


            //searchRequest.FieldsToReturn = new List<string> { "contacts", "contacts.campaigns" };

            //var fields = new List<string>
            //{
            //    "contacts.firstName",
            //    "contacts.lastName",
            //    "contacts.email",
            //};
            //var fieldsToRetrieveOnResponse = new List<string>
            //{
            //    "contacts",
            //    "id",
            //    "name",
            //    "contacts.campaigns"
            //};
            //searchRequest.Fields = fields;
            //searchRequest.FieldsToReturn = fieldsToRetrieveOnResponse;

            //var model = new SearchReq();

            //model.Filters.Add(new FilterDefinition
            //{
            //    Field = "contacts.shield",
            //    LogicalOperator = LogicalOperator.AND,
            //    Value = "Others",
            //    FindExactMatches = false,
            //});

            //model.FieldsToReturn = new List<string> { "contacts.shield", "contacts.id" };
            //var result = await elasticSearch.ComplexSearchAsync(elasticClient, model);
            //var aggregationRequest = new AggregationRequest
            //{
            //    FieldToAggregateBy = "industryId",
            //    AggregationType = AggregationType.GroupBy,
            //};
            ///Get AggregationReq sep or get searchreq child named complexSearchReq
            ///Separate metohd for aggregation with aggregation as separate param, source false

            //var aggResp = await elasticSearch.AggregateAsync(
            //    elasticClient,
            //    searchRequest,
            //    aggregationRequest);
            searchRequest.Filters = deserialised;

            var searchResp = await elasticSearch.ComplexSearchAsync(elasticClient, searchRequest);
        }

        public static string GetOptionsSubstitutedWithLookupOptions(string queryBuilderInputConfig)
        {
            var typeKey = "type";
            var lookupTypePattern = "lookup:";
            var filterPartConfigsKey = "filterPartConfigs";
            var filterPartConfigOptionsKey = "options";
            var keyDelimitter = ":";

            var multiSelectTypeKey = "multiSelect";

            var parsedConfig = JObject.Parse(queryBuilderInputConfig);

            foreach (var configName in parsedConfig.Properties().Select(item => item.Name))
            {
                var config = parsedConfig[configName];
                var filterPartConfigs = config[filterPartConfigsKey];

                foreach (var filterPartConfig in filterPartConfigs)
                {
                    var type = filterPartConfig[typeKey].ToString();
                    if (type.Contains(lookupTypePattern))
                    {
                        var keyToSubstituteMultiSelectType = type;
                        var keyToSubstituteOptions = filterPartConfig[filterPartConfigOptionsKey].ToString();

                        var lookupKey = keyToSubstituteOptions.Split(keyDelimitter)[0];

                        // TODO: enhance the following to get dynamic list of multiselect options rather than only features

                        queryBuilderInputConfig = queryBuilderInputConfig.Replace(keyToSubstituteMultiSelectType, multiSelectTypeKey);
                        queryBuilderInputConfig = queryBuilderInputConfig.Replace($"\"{keyToSubstituteOptions}\"", new JArray().ToString());
                    }
                }
            }

            var parsed = JObject.Parse(queryBuilderInputConfig);
            return queryBuilderInputConfig;
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
            var result = string.Join('.', stack);
            return char.ToLowerInvariant(result[0]) + result.Substring(1);
        }
    }
}

////var searchRequest = new SearchReq
////{
////    Filters = new List<IFilterDefinition> {
////        ElasticSearchService.ConstructFilterDefinitionForCustomerByIds(new List<int> { 3 }),
////        ElasticSearchService.ConstructFilterDefinitionForSurveyById(1),
////        ElasticSearchService.ConstructFilterDefinitionForDateRange(new DateRan
////        {
////            StartDate = new DateTime(2022, 01, 01).Date,
////            EndDate = new DateTime(2022, 12, 12).Date,
////        }),
////        new FilterDefinition
////            {
////                Field = "contacts.shield",
////                LogicalOperator = LogicalOperator.OR,
////                Values = new List<string> {
////                    "Advocate",
////                    "CEO"
////                },
////                FindExactMatches = true,
////            },
////        new FilterDefinition
////            {
////                Field = "contacts.campaigns.response.responderSentiment",
////                LogicalOperator = LogicalOperator.OR,
////                Values = new List<string> {
////                    "promotors",
////                    "passives"
////                },
////                FindExactMatches = true,
////            },
////        new FilterDefinition
////            {
////                Field = "opportunities.tag",
////                LogicalOperator = LogicalOperator.OR,
////                Values = new List<string> {
////                    "test",
////                    "sample"
////                },
////                FindExactMatches = true,
////            },

////    }
////};
//searchRequest.FieldsToReturn = new List<string> {
//    "contacts.name",
//    "contacts.campaigns.response.responderSentiment",
//    "contacts.shield",
//    "name"
//};

//var simpleReq = new SearchReq
//{
//    Filters = new List<IFilterDefinition> {
//        new FilterDefinition{
//            Field = "contacts.firstName",
//                LogicalOperator = LogicalOperator.OR,
//                FindExactMatches = false,

//            Value = "test"
//        }
//    },
//    FieldsToReturn = new List<string> {
//        "contacts.firstName",
//        "name",
//        "email"
//    }
//};

//var simpleReq = new SearchReq
//{
//    Filters = new List<IFilterDefinition> {
//        new FilterDefinition
//        {
//            Field = "contacts.campaigns.respondedOn",
//            LogicalOperator = LogicalOperator.AND,
//            FilterType = FilterTypes.DateRange,
//            Values = new List<string>
//            {
//                "01-01-2021",
//                "12-01-2022",
//            },
//            FindExactMatches = true,
//        },
//        new FilterDefinition
//        {
//            Field = "contacts.campaigns.surveyTypeId",
//            LogicalOperator = LogicalOperator.AND,
//            Value = 1,
//            FindExactMatches = true,
//        },
//        new FilterDefinition
//        {
//            Field = "contacts.campaigns.response.responderSentiment",
//            LogicalOperator = LogicalOperator.AND,
//            Value = "passives",
//            FindExactMatches = true,
//        },
//        new FilterDefinition
//        {
//            Field = "contacts.shield",
//            LogicalOperator = LogicalOperator.AND,
//            Value = "Advocate",
//            FindExactMatches = true,
//        }
//    }
//};

//var searchFieldsContacts = new SearchReq
//{
//    Fields = new List<string>
//{
//    "contacts.firstName",
//    "contacts.lastName",
//    "contacts.email",
//},
//    FieldsToReturn = new List<string>
//{
//    "contacts",
//    "id",
//    "name",
//},
//    Query = "test"

//};

//var searchFieldsCustomers = new SearchReq
//{
//    Fields = new List<string>
//{
//    "name",
//    "domain",
//    "customTags",
//},
//    FieldsToReturn = new List<string>
//{
//    "name",
//    "domain",
//    "customTags",
//    "id",
//},
//    Query = "test"

//};

//var searchFieldsOpps = new SearchReq
//{
//    Fields = new List<string>
//{
//    "opportunities.name",
//    "opportunities.customTags",
//},
//    FieldsToReturn = new List<string>
//{
//    "opportunities",
//    "id",
//    "name",
//},
//    Query = "test"
//};

//var searchFields = new SearchReq
//{
//    Fields = new List<string>
//{
//    //"name",
//    //"domain",
//    //"customTags",
//    "opportunities.name",
//    "opportunities.customTags",
//    "contacts.firstName",
//    "contacts.lastName",
//    "contacts.email",
//},
//    Query = "test",
//    FieldsToReturn = new List<string>
//{
//        "id",
//    "name",
//    "domain",
//    "customTags",
//    "opportunities.id",
//    "opportunities.name",
//    "opportunities.customTags",
//    "contacts.id",
//    "contacts.firstName",
//    "contacts.lastName",
//    "contacts.email",
//},
//    PageSize = 10000
//};
//var customerIds = new List<string> { };           

//var model = new SearchReq();

//model.Filters.Add(new FilterDefinition
//{
//    Field = "contacts.campaigns.respondedOn",
//    LogicalOperator = LogicalOperator.AND,
//    FilterType = FilterTypes.DateRange,
//    Values = new List<string>
//    {
//        "01-01-2021",
//        "12-01-2022",
//    },
//    FindExactMatches = true,
//});
//model.Filters.Add(new FilterDefinition
//{
//    Field = "contacts.campaigns.surveyTypeId",
//    LogicalOperator = LogicalOperator.AND,
//    Value = 1,
//    FindExactMatches = true,
//});



//model.Filters.Add(new FilterDefinition
//{
//    Field = "contacts.campaigns.response.responderSentiment",
//    LogicalOperator = LogicalOperator.AND,
//    Values = new List<string>
//    {
//        "passives",
//    },
//    FindExactMatches = true,
//});



//model.Filters.Add(new FilterDefinition
//{
//    Field = "contacts.shield",
//    LogicalOperator = LogicalOperator.AND,
//    Values = new List<string>
//    {
//        "advocate",
//    },
//    FindExactMatches = true,
//});//var request = ElasticSearchService.ConstructSearchRequest(elasticClient, simpleReq);

//var response = await elasticSearch.ComplexSearchAsync(
//    elasticClient,
//    model); //var jsonList = new List<string> { };
//var responseJson = new StringBuilder();
//response.ToList().ForEach((_) =>
//{
//    jsonList.Add(_.ToJson());
//    responseJson.Append((string.IsNullOrEmpty(responseJson.ToString()) ? string.Empty : ",") + _.ToJson());
//});