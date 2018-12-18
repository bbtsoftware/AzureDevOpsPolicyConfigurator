using System.Collections.Generic;

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
        /// <param name="contents">String contents to merge and deserialize</param>
        /// <returns>A deserialized object</returns>
        T Deserialize<T>(IEnumerable<string> contents);

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="content">String content to deserialize</param>
        /// <returns>A deserialized object</returns>
        T Deserialize<T>(string content);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Serialized string</returns>
        string Serialize<T>(T obj);
    }
}
