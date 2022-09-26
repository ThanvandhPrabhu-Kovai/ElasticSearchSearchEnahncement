using ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models
{
    public class FilterDefinition : IFilterDefinition
    {
        public const string TypeName = "filter";

        private ICollection<string> values = new List<string>();

        public FilterDefinition()
        {
        }

        public FilterDefinition(string field, string @operator, object value)
        {
            this.Field = field;
            this.Operator = @operator;
            this.LogicalOperator = LogicalOperator.AND;
            this.Value = value;
        }

        public FilterDefinition(string field, string @operator, LogicalOperator logicalOperator, object value)
        {
            this.Field = field;
            this.Operator = @operator;
            this.LogicalOperator = logicalOperator;
            this.Value = value;
        }

        public FilterDefinition(List<string> fields, string @operator, LogicalOperator logicalOperator, object value)
        {
            this.Fields = fields;
            this.Operator = @operator;
            this.LogicalOperator = logicalOperator;
            this.Value = value;
        }

        ////public string Type { get; } = TypeName;

        public string Field { get; set; }

        public string Operator { get; set; }

        public LogicalOperator LogicalOperator { get; set; }

        public object Value { get; set; }

        public ICollection<string> Values
        {
            get
            {
                if (this.values != null && this.values.Any())
                {
                    return this.values;
                }

                this.values = new List<string>();

                return this.values;
            }
            set => this.values = value;
        }

        public IEnumerable<string> Fields { get; set; } = new List<string>();

        public FilterTypes FilterType { get; set; }

        public bool FindExactMatches { get; set; }
    }
}
