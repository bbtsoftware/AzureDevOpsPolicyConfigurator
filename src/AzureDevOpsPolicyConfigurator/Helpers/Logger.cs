using System.Reflection;
using log4net;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Logger class.
    /// </summary>
    internal class Logger : ILogger
    {
        private static readonly ILog Log = LogManager.GetLogger(Assembly.GetExecutingAssembly().GetType());

        /// <inheritdoc/>
        public void Info(string message)
        {
            Log.Info(message);
        }

        /// <inheritdoc/>
        public void Debug(string message)
        {
            Log.Debug(message);
        }

        /// <inheritdoc/>
        public void Warn(string message)
        {
            Log.Warn(message);
        }
    }
}
