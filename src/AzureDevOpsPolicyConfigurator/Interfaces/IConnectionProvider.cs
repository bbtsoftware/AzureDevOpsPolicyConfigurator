using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Connection Provider Interface
    /// </summary>
    internal interface IConnectionProvider
    {
        /// <summary>
        /// Gets the connection
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <returns>Returns a VssConnection</returns>
        VssConnection GetConnection(BaseSettings arguments);
    }
}
