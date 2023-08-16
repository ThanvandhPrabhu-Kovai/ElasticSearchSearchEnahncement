namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    public class ComplexNegationFilterDefinition : FilterDefinition, IFilterDefinition
    {
        public FilterTypes NegationType { get; set; }
    }
}
