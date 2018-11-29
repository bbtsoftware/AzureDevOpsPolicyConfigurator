using System.ComponentModel;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// GenerateCommand settings.
    /// </summary>
    internal class GeneratorSettings : BaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorSettings"/> class.
        /// </summary>
        public GeneratorSettings()
        {
            this.Destination = @".\Projecť";
        }

        /// <summary>
        /// Gets or sets the out directory.
        /// </summary>
        [CommandOption("-d|--destination <DESTINATION>")]
        [Description("Destination directory")]
        public string Destination { get; set; }
    }
}