using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    public enum AggregationType
    {
        Count = 0,
        Sum = 1,
        Average = 2,
        Min = 3,
        Max = 4,
        Stats = 5,
        GroupBy = 6,
    }
}
