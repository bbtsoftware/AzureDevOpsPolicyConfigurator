using System.Collections.Generic;

namespace AzureDevOpsPolicyConfigurator.Data
{
    /// <summary>
    /// ApplyTo class.
    /// </summary>
    internal class ApplyTo
    {
        /// <summary>
        /// Gets or sets the projects to apply to.
        /// </summary>
        public List<string> Projects { get; set; }

        /// <summary>
        /// Gets or sets the repositories to apply to.
        /// </summary>
        public List<string> Repositories { get; set; }
    }
}