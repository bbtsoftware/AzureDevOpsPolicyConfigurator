using System.ComponentModel;
using AzureDevOpsPolicyConfigurator.Logic;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// WhatIfCommand class.
    /// </summary>
    [Description("Simulates the execution of the policy changes")]
    internal sealed class WhatIfCommand : CommandBase<ExecuterSettings>
    {
        /// <summary>
        /// Executes the WhatIfExecuter.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>int</returns>
        public override int Execute(CommandContext context, ExecuterSettings settings)
        {
            new WhatIfExecuter(new JsonSerializer(), new FileReader(), new ConnectionProvider()).Execute(settings);
            return 0;
        }
    }
}