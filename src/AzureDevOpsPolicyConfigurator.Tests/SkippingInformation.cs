namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Skipping information task.
    /// </summary>
    internal static class SkippingInformation
    {
        /// <summary>
        /// Skipping reason
        /// </summary>
#if DEBUG
        public const string SkippingReason = null;
#else
        public const string SkippingReason = "Can only be run locally";
#endif
    }
}
