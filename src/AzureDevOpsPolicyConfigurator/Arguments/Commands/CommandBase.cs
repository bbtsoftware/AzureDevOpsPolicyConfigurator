using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Spectre.Cli;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// CommandBase class.
    /// </summary>
    /// <typeparam name="TSettings">Settings type</typeparam>
    internal abstract class CommandBase<TSettings> : Command<TSettings>
        where TSettings : BaseSettings
    {
        private const string Log4netFileName = "log4net.config";

        /// <summary>
        /// Validate base method.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>ValidationResult</returns>
        public override ValidationResult Validate(CommandContext context, TSettings settings)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(executingAssembly);

            var fileInfo = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Log4netFileName);

            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(fileInfo));

            SetVerbosityLevel(settings, logRepository);

            return base.Validate(context, settings);
        }

        private static void SetVerbosityLevel(TSettings settings, ILoggerRepository logRepository)
        {
            switch (settings.Verbosity)
            {
                case LogLevel.Info:
                    ((Hierarchy)logRepository).Root.Level = Level.Info;
                    break;
                case LogLevel.Debug:
                    ((Hierarchy)logRepository).Root.Level = Level.Debug;
                    break;
                case LogLevel.Warn:
                    ((Hierarchy)logRepository).Root.Level = Level.Warn;
                    break;
                case LogLevel.Error:
                    ((Hierarchy)logRepository).Root.Level = Level.Error;
                    break;
            }
        }
    }
}
