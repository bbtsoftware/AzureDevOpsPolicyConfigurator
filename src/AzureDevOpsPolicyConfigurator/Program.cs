using System.Reflection;
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
        public static void Main(string[] args)
        {
            new Program().Run(args, false);
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <param name="propagateException">Propagate exception</param>
        /// <returns>int</returns>
        internal int Run(string[] args, bool propagateException)
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

            return app.Run(args);
        }
    }
}
