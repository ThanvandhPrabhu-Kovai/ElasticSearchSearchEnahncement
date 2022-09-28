using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchSearchEnhancement.Models;
using Nest;
using QueryEditor.Services.ElasticSearch;
using SearchReq = ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates.SearchRequest;
using DateRan = ElasticSearchSearchEnhancement.Models.Search.DateRange;
using System;

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

            var searchRequest = new SearchReq { };
            searchRequest.FilterDefinitions.AddRange(
                new List<FilterDefinition> {
                    ElasticSearchService.ConstructFilterDefinitionForCustomerByIds(new List<int> { 3 }),
                    ElasticSearchService.ConstructFilterDefinitionForSurveyById(1),
                    ElasticSearchService.ConstructFilterDefinitionForDateRange(new DateRan {
                        StartDate = new DateTime(2022,01,01).Date,
                        EndDate = new DateTime(2022,12,12).Date,
                    }),
                });

            var request = ElasticSearchService.ConstructSearchRequest(elasticClient, searchRequest);

            var searchRes = await ElasticSearchService.SearchAsync(elasticClient);

            var response = await elasticSearch.SearchThroughNestedObjectsAsync(
                elasticClient,
                searchRequest,
                "contacts.campaigns");

        }


    }
}
