using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// JsonSerializer class
    /// </summary>
    internal class JsonSerializer : IJsonSerializer
    {
        /// <inheritdoc/>
        public T Deserialize<T>(IEnumerable<string> contents)
        {
            JObject mergedContent = new JObject();

            foreach (var content in contents)
            {
                mergedContent.Merge(JObject.Parse(content), new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
            }

            return this.Deserialize<T>(mergedContent.ToString());
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string content)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.DeserializeObject<T>(content, settings);
        }

        /// <inheritdoc/>
        public string Serialize<T>(T obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <inheritdoc/>
        public T Clone<T>(T obj)
        {
            return this.Deserialize<T>(this.Serialize(obj));
        }
    }
}
