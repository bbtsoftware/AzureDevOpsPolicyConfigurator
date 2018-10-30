namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// IFileReader Interface.
    /// </summary>
    internal interface IFileReader
    {
        /// <summary>
        /// Loads file content in UTF8.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Returns the file content</returns>
        string GetFileContent(string path);
    }
}
