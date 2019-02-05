using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Programm class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entrypoint
        /// </summary>
        /// <param name="args">Main arguments</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            await new Program().Run(args, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <param name="propagateException">Propagate exception</param>
        /// <returns>int</returns>
        internal async Task<int> Run(string[] args, bool propagateException)
        {
            if (args == null)
            {
                args = System.Array.Empty<string>();
            }

            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddCommand<GenerateCommand>("generate");
                config.AddCommand<WhatIfCommand>("whatif");
                config.AddCommand<ExecuteCommand>("execute");

                if (propagateException)
                {
                    config.PropagateExceptions();
                }
            });

            var executingAssembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(executingAssembly);

            return await app.RunAsync(args).ConfigureAwait(false);
        }
    }
}
