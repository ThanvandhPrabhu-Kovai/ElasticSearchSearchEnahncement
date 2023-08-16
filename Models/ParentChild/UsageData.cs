using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.ParentChild
{
    public class UsageData
    {
        public JoinField UsageJoinField { get; set; }

        public string ChildIdentifier { get; set; }

        public int CustomerId { get; set; }

        public List<object> Dau { get; set; }

        public List<object> Features { get; set; }

        public List<object> Browser { get; set; }

        public List<object> Location { get; set; }
    }
}
