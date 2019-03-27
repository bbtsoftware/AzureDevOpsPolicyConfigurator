using System.IO;
using System.Text;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// FileReader class.
    /// </summary>
    internal class FileReader : IFileReader
    {
        /// <inheritdoc/>
        public string GetFileContent(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }
    }
}
