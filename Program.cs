using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchSearchEnhancement.Models;
using Nest;
using QueryEditor.Services.ElasticSearch;
using SearchReq = ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates.SearchRequest;
using DateRan = ElasticSearchSearchEnhancement.Models.Search.DateRange;
using System;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;

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

            var request = ElasticSearchService.ConstructSearchRequest(elasticClient, searchRequest);

            var response = await elasticSearch.SearchThroughNestedObjectsAsync(
                elasticClient,
                searchRequest,
                "contacts.campaigns");

        }


    }
}
