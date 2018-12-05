using AzureDevOpsPolicyConfigurator.Exceptions;
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
        /// <summary>
        /// Validate base method.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="settings">Settings</param>
        /// <returns>ValidationResult</returns>
        public override ValidationResult Validate(CommandContext context, TSettings settings)
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

            return base.Validate(context, settings);
        }
    }
}
