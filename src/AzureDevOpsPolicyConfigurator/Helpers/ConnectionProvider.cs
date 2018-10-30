using System;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Connection Provider class.
    /// </summary>
    internal class ConnectionProvider : IConnectionProvider
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="arguments">Main arguments</param>
        /// <returns>Returns Credential</returns>
        public VssConnection GetConnection(BaseSettings arguments)
        {
            var uri = new Uri(arguments.CollectionUrl);
            var credentials = this.GetCredentials(arguments);

            return new VssConnection(uri, credentials);
        }

        private VssCredentials GetCredentials(BaseSettings arguments)
        {
            switch (arguments.Auth)
            {
                case AuthMethod.Basic:
                    return new VssBasicCredential(arguments.User ?? string.Empty, arguments.Password);
                case AuthMethod.Ntlm:
                    return new VssCredentials(new WindowsCredential());
                case AuthMethod.OAuth:
                    return new VssCredentials(new VssOAuthAccessTokenCredential(arguments.Password));
                case AuthMethod.AzureActiveDirectory:
                    return new VssCredentials(new VssAadCredential(arguments.User ?? string.Empty, arguments.Password));
                default:
                    throw new ArgumentException("Not supported authentication method");
            }
        }
    }
}
