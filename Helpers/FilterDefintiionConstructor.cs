using ElasticSearchSearchEnhancement.Models;
using ElasticSearchSearchEnhancement.Models.Search;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchSearchEnhancement.Helpers
{
    public class FilterDefintiionConstructor
    {
        private FilterDefinition ConstructFilterDefinitionForCustomerByIds(IEnumerable<int> ids)
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

        private FilterDefinition ConstructFilterDefinitionForDateRange(DateRange dateRange)
        {
            return new FilterDefinition
            {
                Field = "contacts.campaigns.respondedOn",
                LogicalOperator = LogicalOperator.AND,
                FilterType = FilterTypes.DateRange,
                Values = new List<string>
                {
                    dateRange.StartDate.ToString(),
                    dateRange.EndDate.ToString(),
                },
                FindExactMatches = true,
            };
        }

        private FilterDefinition ConstructFilterDefinitionForSurveyById(int id)
        {
            return new FilterDefinition
            {
                Field = "contacts.campaigns.surveyTypeId",
                LogicalOperator = LogicalOperator.AND,
                Value = id,
                FindExactMatches = true,
            };
        }
    }
}
