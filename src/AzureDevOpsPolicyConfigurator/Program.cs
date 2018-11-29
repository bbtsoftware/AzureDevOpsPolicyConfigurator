using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
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
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddCommand<GenerateCommand>("generate");
                config.AddCommand<WhatIfCommand>("whatif");
                config.AddCommand<ExecuteCommand>("execute");
            });

            var executingAssembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(executingAssembly);

            app.Run(args);
        }
    }
}
