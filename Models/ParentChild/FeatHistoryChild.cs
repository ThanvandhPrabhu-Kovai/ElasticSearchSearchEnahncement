using Nest;

namespace ElasticSearchSearchEnhancement.Models.ParentChild
{
    public class FeatHistoryChild
    {
        public JoinField UsageJoinField { get; set; }

        public object History { get; set; }
    }
}
