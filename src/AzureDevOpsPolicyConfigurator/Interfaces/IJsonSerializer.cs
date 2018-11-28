namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// JsonSerializer interface.
    /// </summary>
    internal interface IJsonSerializer
    {
        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">String to deserialize</param>
        /// <returns>A deserialized object</returns>
        T Deserialize<T>(string str);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Serialized string</returns>
        string Serialize<T>(T obj);
    }
}
