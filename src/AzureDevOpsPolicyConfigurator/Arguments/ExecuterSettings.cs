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
        [CommandOption("-i|--in <INPUT>")]
        [Description("Json input file")]
        public string Input { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output should be verbose.
        /// </summary>
        [CommandOption("-v|--verbosity <VERBOSITY>")]
        [Description("Sets the log4net log level")]
        public LogLevel Verbosity { get; set; }
    }
}