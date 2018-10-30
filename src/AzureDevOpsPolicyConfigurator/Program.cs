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

            app.Run(args);
        }
    }
}
