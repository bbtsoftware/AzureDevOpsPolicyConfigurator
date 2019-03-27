namespace AzureDevOpsPolicyConfigurator.Exceptions
{
    /// <summary>
    /// Argument Validation Exception.
    /// </summary>
    internal class ArgumentValidationException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentValidationException"/> class.
        /// </summary>
        /// <param name="message">Message</param>
        public ArgumentValidationException(string message)
            : base(message)
        {
        }
    }
}
