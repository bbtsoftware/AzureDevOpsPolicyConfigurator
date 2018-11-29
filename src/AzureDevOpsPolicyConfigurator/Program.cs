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
            var executingAssembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(executingAssembly);
            var fileInfo = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "log4net.config");

            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(fileInfo));

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
