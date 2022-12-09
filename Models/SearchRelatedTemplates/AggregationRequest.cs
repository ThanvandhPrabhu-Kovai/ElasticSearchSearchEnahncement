using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    public class AggregationRequest
    {
        public string FieldToAggregateBy { get; set; }

        public AggregationType AggregationType { get; set; }

        public IEnumerable<string>? KeysToReturn { get; set; }
    }
}
