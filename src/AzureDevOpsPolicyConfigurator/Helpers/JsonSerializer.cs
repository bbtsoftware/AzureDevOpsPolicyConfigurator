using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// JsonSerializer class
    /// </summary>
    internal class JsonSerializer : IJsonSerializer
    {
        /// <inheritdoc/>
        public T Deserialize<T>(string str)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.DeserializeObject<T>(str, settings);
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
    }
}
