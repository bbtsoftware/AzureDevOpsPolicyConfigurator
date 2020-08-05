using System.ComponentModel;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Logic;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// ExecuteCommand class.
    /// </summary>
    [Description("Executes the policy changes")]
    internal sealed class ExecuteCommand : ExecuterCommandBase
    {
        /// <summary>
        /// Executes the PolicyExecuter.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task<int> ExecuteAsync(CommandContext context, ExecuterSettings settings)
        {
            await new PolicyExecuter(new JsonSerializer(), new FileReader(), new ConnectionProvider(), new Logger()).Execute(settings).ConfigureAwait(false);
            return 0;
        }
    }
}