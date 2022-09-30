using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    public class FilterGroup : IFilterDefinition
    {
        public const string TypeName = "filterGroup";

        public FilterGroup()
        {
        }

        public FilterGroup(LogicalOperator logicalOperator, IEnumerable<IFilterDefinition> filters)
        {
            this.LogicalOperator = logicalOperator;
            this.Filters = filters;
        }

        ////public string Type { get; } = TypeName;

        public LogicalOperator LogicalOperator { get; set; }

        public IEnumerable<IFilterDefinition> Filters { get; set; }

        public FilterGroup Child { get; set; }
    }
}
