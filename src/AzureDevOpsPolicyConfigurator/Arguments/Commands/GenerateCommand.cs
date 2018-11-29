using System.ComponentModel;
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
        /// <returns>int</returns>
        public override int Execute(CommandContext context, GeneratorSettings settings)
        {
            new StructureGenerator(new JsonFileWriter(), new ConnectionProvider()).Execute(settings);
            return 0;
        }
    }
}