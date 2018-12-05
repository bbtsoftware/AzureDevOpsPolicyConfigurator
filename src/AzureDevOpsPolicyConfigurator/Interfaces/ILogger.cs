namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// ILogger interface.
    /// </summary>
    internal interface ILogger
    {
        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Logs a warn message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warn(string message);
    }
}
