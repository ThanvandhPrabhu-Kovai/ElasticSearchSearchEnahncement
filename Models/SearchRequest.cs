using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models
{
    public class SearchRequest
    {
        public string Filters { get; set; }
    }

    public class SearchReq : SearchRequest
    {
        public new int Filters { get; set; }
    }
}
