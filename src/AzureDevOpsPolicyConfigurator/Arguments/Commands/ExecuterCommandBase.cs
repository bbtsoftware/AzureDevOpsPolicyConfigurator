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
    /// Executer command base class.
    /// </summary>
    internal abstract class ExecuterCommandBase : CommandBase<ExecuterSettings>
    {
        private const string Log4netFileName = "log4net.config";

        /// <inheritdoc/>
        public override ValidationResult Validate(CommandContext context, ExecuterSettings settings)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(executingAssembly);

            var fileInfo = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Log4netFileName);

            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(fileInfo));

            this.SetVerbosityLevel(settings, logRepository);

            return base.Validate(context, settings);
        }

        private void SetVerbosityLevel(ExecuterSettings settings, ILoggerRepository logRepository)
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
