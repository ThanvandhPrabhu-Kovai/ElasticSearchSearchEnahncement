using Newtonsoft.Json.Linq;
using System.Linq;

namespace ElasticSearchSearchEnhancement.Services
{
    public static class LookupService
    {
        public static string GetOptionsSubstitutedWithLookupOptions(string queryBuilderInputConfig)
        {
            var typeKey = "type";
            var lookupTypePattern = "lookup:";
            var filterPartConfigsKey = "filterPartConfigs";
            var filterPartConfigOptionsKey = "options";
            var keyDelimitter = ":";

            var multiSelectTypeKey = "multiSelect";

            var parsedConfig = JObject.Parse(queryBuilderInputConfig);

            foreach (var configName in parsedConfig.Properties().Select(item => item.Name))
            {
                var config = parsedConfig[configName];
                var filterPartConfigs = config[filterPartConfigsKey];

                foreach (var filterPartConfig in filterPartConfigs)
                {
                    var type = filterPartConfig[typeKey].ToString();
                    if (type.Contains(lookupTypePattern))
                    {
                        var keyToSubstituteMultiSelectType = type;
                        var keyToSubstituteOptions = filterPartConfig[filterPartConfigOptionsKey].ToString();

                        var lookupKey = keyToSubstituteOptions.Split(keyDelimitter)[0];

                        // TODO: enhance the following to get dynamic list of multiselect options rather than only features

                        queryBuilderInputConfig = queryBuilderInputConfig.Replace(keyToSubstituteMultiSelectType, multiSelectTypeKey);
                        queryBuilderInputConfig = queryBuilderInputConfig.Replace($"\"{keyToSubstituteOptions}\"", new JArray().ToString());
                    }
                }
            }

            var parsed = JObject.Parse(queryBuilderInputConfig);
            return queryBuilderInputConfig;
        }
    }
}
