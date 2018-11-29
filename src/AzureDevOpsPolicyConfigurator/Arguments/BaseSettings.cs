using System.ComponentModel;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// BaseSettings class.
    /// </summary>
    internal class BaseSettings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the collection url.
        /// </summary>
        [CommandOption("-c|--collectionurl <COLLECTIONURL>")]
        [Description("Team Collection URL")]
        public string CollectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the authentication mode.
        /// </summary>
        [CommandOption("-a|--auth <AUTH>")]
        [Description("Authentication mode")]
        public AuthMethod Auth { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [CommandOption("-p|--password <PASSWORD>")]
        [Description("Password|PAT|Token for authentication")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [CommandOption("-u|--user <USER>")]
        [Description("User for authentication")]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output should be verbose.
        /// </summary>
        [CommandOption("-v|--verbosity <VERBOSITY>")]
        [Description("Sets the log4net log level")]
        public LogLevel Verbosity { get; set; }
    }
}