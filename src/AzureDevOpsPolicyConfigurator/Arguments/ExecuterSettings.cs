using System.ComponentModel;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// ExecuterSettings class.
    /// </summary>
    internal class ExecuterSettings : BaseSettings
    {
        /// <summary>
        /// Gets or sets the input directory.
        /// </summary>
        [CommandOption("-i|--in <IN>")]
        [Description("Json input file")]
        public string Input { get; set; }
    }
}