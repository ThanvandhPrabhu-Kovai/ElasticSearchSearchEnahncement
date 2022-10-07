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

namespace QueryEditor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ElasticClient elasticClient = ElasticSearchService.GetElasticClient();
            ElasticSearchService elasticSearch = new ElasticSearchService();
            ElasticSearchService.IndexMapping = ElasticSearchService.GetMapping(elasticClient);

            var customerIds = new List<string> { };

            var searchRequest = new SearchReq
            {
                Filters = new List<IFilterDefinition> {
                    ElasticSearchService.ConstructFilterDefinitionForCustomerByIds(new List<int> { 3 }),
                    ElasticSearchService.ConstructFilterDefinitionForSurveyById(1),
                    ElasticSearchService.ConstructFilterDefinitionForDateRange(new DateRan
                    {
                        StartDate = new DateTime(2022, 01, 01).Date,
                        EndDate = new DateTime(2022, 12, 12).Date,
                    }),
                    new FilterDefinition
                        {
                            Field = "contacts.shield",
                            LogicalOperator = LogicalOperator.OR,
                            Values = new List<string> {
                                "Advocate",
                                "CEO"
                            },
                            FindExactMatches = true,
                        },
                    new FilterDefinition
                        {
                            Field = "contacts.campaigns.response.responderSentiment",
                            LogicalOperator = LogicalOperator.OR,
                            Values = new List<string> {
                                "promotors",
                                "passives"
                            },
                            FindExactMatches = true,
                        },
                    new FilterDefinition
                        {
                            Field = "opportunities.tag",
                            LogicalOperator = LogicalOperator.OR,
                            Values = new List<string> {
                                "test",
                                "sample"
                            },
                            FindExactMatches = true,
                        },

                }
            };
            searchRequest.FieldsToReturn = new List<string> {
                "contacts.name",
                "contacts.campaigns.response.responderSentiment",
                "contacts.shield",
                "name"
            };

            var simpleReq = new SearchReq
            {
                Filters = new List<IFilterDefinition> {
                    new FilterDefinition{
                        Field = "contacts.firstName",
                            LogicalOperator = LogicalOperator.OR,
                            FindExactMatches = false,

                        Value = "test"
                    }
                },
                FieldsToReturn = new List<string> {
                    "contacts.firstName",
                    "name",
                    "email"
                }
            };

            var request = ElasticSearchService.ConstructSearchRequest(elasticClient, simpleReq);

            var response = await elasticSearch.ComplexSearchAsync(
                elasticClient, 
                simpleReq);

            var jsonList = new List<string> { };

            response.ToList().ForEach((_) =>
            {
                jsonList.Add(_.ToJson());
            });
        }
    }
}
