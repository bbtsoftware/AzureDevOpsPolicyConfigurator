namespace AzureDevOpsPolicyConfigurator.Test
{
    /// <summary>
    /// Test file representation.
    /// </summary>
    internal class TestConfiguration
    {
        private static TestConfiguration currentConfiguration;

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public static TestConfiguration CurrentConfiguration
        {
            get
            {
                if (currentConfiguration == null)
                {
                    currentConfiguration = new JsonSerializer().Deserialize<TestConfiguration>(new FileReader().GetFileContent(".\\test.json"));
                }

                return currentConfiguration;
            }
        }

        /// <summary>
        /// Gets or sets the CollectionUrl
        /// </summary>
        public string CollectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the Project
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the Repository
        /// </summary>
        public string Repository { get; set; }
    }
}
