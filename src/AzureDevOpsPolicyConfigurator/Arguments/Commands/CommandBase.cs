using System;
using System.IO;
using System.Reflection;
using AzureDevOpsPolicyConfigurator.Exceptions;
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

            this.SetVerbosityLevel(settings, logRepository);

            this.ValidateSettings(settings);

            return base.Validate(context, settings);
        }

        private void ValidateSettings(TSettings settings)
        {
            if (string.IsNullOrEmpty(settings.CollectionUrl))
            {
                throw new ArgumentValidationException("Argument \"connectionurl\" is missing!");
            }

            if (settings.Auth == null)
            {
                throw new ArgumentValidationException("Argument \"auth\" is missing!");
            }

            if (settings.Auth == AuthMethod.Basic || settings.Auth == AuthMethod.OAuth)
            {
                if (string.IsNullOrEmpty(settings.User))
                {
                    throw new ArgumentValidationException("Argument \"user\" must be provided if \"auth\" is set to Basic or OAuth!");
                }

                if (string.IsNullOrEmpty(settings.Password))
                {
                    throw new ArgumentValidationException("Argument \"password\" must be provided if \"auth\" is set to Basic or OAuth!");
                }
            }

            if (settings is ExecuterSettings eSettings && string.IsNullOrEmpty(eSettings.Input))
            {
                throw new ArgumentValidationException("Argument \"in\" must be provided!");
            }
        }

        private void SetVerbosityLevel(TSettings settings, ILoggerRepository logRepository)
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
