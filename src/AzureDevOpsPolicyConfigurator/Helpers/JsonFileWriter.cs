using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// JsonFileWriter class.
    /// </summary>
    internal class JsonFileWriter
    {
        /// <summary>
        /// Serialize Object and write to file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="filePath">File path</param>
        public void SerializeAndWrite<T>(T obj, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var converted = JsonConvert.SerializeObject(obj);
                var bytes = new UTF8Encoding(true).GetBytes(converted);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
