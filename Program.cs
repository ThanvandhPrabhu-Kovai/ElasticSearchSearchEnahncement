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

namespace QueryEditor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ElasticClient elasticClient = ElasticSearchService.GetElasticClient();
            ElasticSearchService elasticSearch = new ElasticSearchService();
            ElasticSearchService.IndexMapping = ElasticSearchService.GetMapping(elasticClient);

            var searchRequest = new SearchReq
            {
                Filters = new List<IFilterDefinition> {
                    new FilterDefinition
                        {
                            Field = "revenueGenerationPotential",
                            LogicalOperator = LogicalOperator.OR,
                            Values = new List<string> { "10000", "200000" },
                            FindExactMatches = false,
                            FilterType = FilterTypes.Range
                    },
                }
            };

            //var aggregationRequest = new AggregationRequest
            //{
            //    FieldToAggregateBy = "industryId",
            //    AggregationType = AggregationType.GroupBy,
            //};
            ///Get AggregationReq sep or get searchreq child named complexSearchReq
            ///Separate metohd for aggregation with aggregation as separate param, source false

            var response = await elasticSearch.ComplexSearchAsync(
                elasticClient,
                searchRequest);
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