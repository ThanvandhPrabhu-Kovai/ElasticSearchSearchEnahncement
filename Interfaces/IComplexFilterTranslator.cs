using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchSearchEnhancement.Models;

namespace QueryEditor.Services.ElasticSearch.Contracts
{
    public interface IComplexFilterTranslator
    {
        Task<IEnumerable<IFilterDefinition>> TranslateFromQueries(IEnumerable<IFilterDefinition> complexFilters);
    }
}
