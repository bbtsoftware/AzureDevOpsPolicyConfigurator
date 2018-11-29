using System.ComponentModel;
using AzureDevOpsPolicyConfigurator.Logic;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// ExecuteCommand class.
    /// </summary>
    [Description("Executes the policy changes")]
    internal sealed class ExecuteCommand : CommandBase<ExecuterSettings>
    {
        /// <summary>
        /// Executes the PolicyExecuter.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>int</returns>
        public override int Execute(CommandContext context, ExecuterSettings settings)
        {
            new PolicyExecuter(new JsonSerializer(), new FileReader(), new ConnectionProvider()).Execute(settings);
            return 0;
        }
    }
}