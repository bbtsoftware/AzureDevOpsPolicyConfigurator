using System.ComponentModel;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Logic;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// GenerateCommand class.
    /// </summary>
    [Description("Generates the current policy structure.")]
    internal sealed class GenerateCommand : CommandBase<GeneratorSettings>
    {
        /// <summary>
        /// Executes the StructureGenerator.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task<int> Execute(CommandContext context, GeneratorSettings settings)
        {
            await new StructureGenerator(new JsonFileWriter(), new ConnectionProvider()).Execute(settings).ConfigureAwait(false);
            return 0;
        }
    }
}