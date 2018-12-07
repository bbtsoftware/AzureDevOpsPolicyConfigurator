namespace AzureDevOpsPolicyConfigurator.Test
{
    /// <summary>
    /// Test Data class.
    /// </summary>
    internal class TestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestData"/> class.
        /// </summary>
        /// <param name="fileContent">File content to return.</param>
        public TestData(string fileContent)
        {
            this.FileContent = fileContent;
        }

        /// <summary>
        /// Gets the file content.
        /// </summary>
        public string FileContent { get; }
    }
}
