using System.ComponentModel;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Logic;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// WhatIfCommand class.
    /// </summary>
    [Description("Simulates the execution of the policy changes")]
    internal sealed class WhatIfCommand : ExecuterCommandBase
    {
        /// <summary>
        /// Executes the WhatIfExecuter.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task<int> Execute(CommandContext context, ExecuterSettings settings)
        {
            await new WhatIfExecuter(new JsonSerializer(), new FileReader(), new ConnectionProvider(), new Logger()).Execute(settings).ConfigureAwait(false);
            return 0;
        }
    }
}