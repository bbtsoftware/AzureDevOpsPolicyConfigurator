namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Authentication Methods.
    /// </summary>
    internal enum AuthMethod
    {
        /// <summary>
        /// Ntlm
        /// </summary>
        Ntlm,

        /// <summary>
        /// OAuth
        /// </summary>
        OAuth,

        /// <summary>
        /// Basic with username / password or wit PAT
        /// </summary>
        Basic,

        /// <summary>
        /// Azure Active Directory
        /// </summary>
        AzureActiveDirectory
    }
}
