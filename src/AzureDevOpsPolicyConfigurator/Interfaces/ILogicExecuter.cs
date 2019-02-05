using System.Threading.Tasks;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Logic Executer Interface.
    /// </summary>
    /// <typeparam name="T">Argument type</typeparam>
    internal interface ILogicExecuter<T>
        where T : CommandSettings
    {
        /// <summary>
        /// Execute Logic.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Execute(T arguments);
    }
}
