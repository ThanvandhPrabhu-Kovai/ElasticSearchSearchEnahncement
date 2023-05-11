using System.Collections.Generic;

namespace ElasticSearchSearchEnhancement.Models
{
    public class ComplexFilter : IFilterDefinition
    {
        public const string TypeName = "complexFilter";

        public string Category { get; set; }

        public string Parameter { get; set; }

        public string Identifier { get; set; }

        public string Value { get; set; }

        public Dictionary<string, object> Identifiers { get; set; }

        public object Filters { get; set; }

        public string DisplayText { get; set; }
    }
}
