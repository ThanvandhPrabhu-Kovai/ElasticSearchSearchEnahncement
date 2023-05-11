using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using ElasticSearchSearchEnhancement.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueryEditor.Services.ElasticSearch.Contracts;
using JsonSubTypes;

namespace QueryEditor.Services.ElasticSearch.Services
{
    public class ComplexFilterTranslator : IComplexFilterTranslator
    {
        private const string CurrentDate = "{currentDate}";
        private const string CurrentDay = "{currentDay}";
        private const string CurrentMonth = "{currentMonth}";
        private const string CurrentYear = "{currentYear}";

        public async Task<IEnumerable<IFilterDefinition>> TranslateFromQueries(IEnumerable<IFilterDefinition> complexFilters)
        {
            var filters = new List<IFilterDefinition>();
            if (complexFilters == null)
            {
                return filters;
            }

            foreach (var filter in complexFilters)
            {
                switch (filter)
                {
                    case FilterGroup complexFilterGroup:
                        var filterGroup = new FilterGroup
                        {
                            LogicalOperator = complexFilterGroup.LogicalOperator,
                            Filters = await this.TranslateFromQueries(complexFilterGroup.Filters),
                        };
                        filters.Add(filterGroup);
                        break;

                    case FilterDefinition filterDefinition:
                        filters.Add(filterDefinition);
                        break;

                    case ComplexFilter complexFilter:
                        filters.AddRange(this.TranslateComplexFilter(complexFilter));
                        break;

                    default:
                        break;
                }
            }

            return filters;
        }

        private IEnumerable<IFilterDefinition> TranslateComplexFilter(ComplexFilter complexFilter)
        {
            var filters = string.Empty;
            if (complexFilter.Identifier != null)
            {
                filters = complexFilter.Filters.ToString()
                    .Replace($"{{{complexFilter.Identifier}}}", complexFilter.Value);
            }

            if (complexFilter.Identifiers?.Count > 0)
            {
                filters = complexFilter.Filters.ToString();
                foreach (var identifier in complexFilter.Identifiers)
                {
                    if (string.IsNullOrEmpty(identifier.Value.ToString()))
                    {
                        continue;
                    }

                    filters = this.ReplaceIdentifiers(filters, identifier);
                }

                filters = filters.Replace(CurrentDate, DateTime.UtcNow.ToString())
                             .Replace(CurrentDay, DateTime.UtcNow.Day.ToString())
                             .Replace(CurrentMonth, DateTime.UtcNow.Month.ToString())
                             .Replace(CurrentYear, DateTime.UtcNow.Year.ToString());
            }

            return JsonConvert.DeserializeObject<List<IFilterDefinition>>(
                    filters.ToString(),
                    FilterConverter);
        }

        public static JsonConverter FilterConverter => JsonSubtypesConverterBuilder
                            .Of<IFilterDefinition>("Type") // type property is only defined here
                            .SetFallbackSubtype(typeof(FilterDefinition))
                            .RegisterSubtype<FilterGroup>(FilterGroup.TypeName)
                            .RegisterSubtype<FilterDefinition>(FilterDefinition.TypeName)
                            .RegisterSubtype<ComplexFilter>(ComplexFilter.TypeName)
                            .SerializeDiscriminatorProperty() // ask to serialize the type property
                            .Build();

        private string ReplaceIdentifiers(string filters, KeyValuePair<string, object> identifier)
        {
            if (identifier.Value.GetType() == typeof(JArray))
            {
                return filters
                            .Replace(
                            "\"" + $"{{{identifier.Key}}}" + "\"",
                            string.Join(",", ((JArray)identifier.Value).ToList<object>()));
            }

            return filters
                        .Replace(
                            $"{{{identifier.Key}}}",
                            identifier.Value.ToString());
        }
    }
}
