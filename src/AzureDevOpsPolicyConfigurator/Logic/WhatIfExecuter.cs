using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Data;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsPolicyConfigurator.Logic
{
    /// <summary>
    /// WhatIfExecuter class.
    /// </summary>
    internal class WhatIfExecuter : PolicyExecuterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhatIfExecuter"/> class.
        /// </summary>
        /// <param name="serializer">Json serializer</param>
        /// <param name="reader">File reader</param>
        /// <param name="connectionProvider">Connection provider</param>
        /// <param name="logger">Logger</param>
        public WhatIfExecuter(IJsonSerializer serializer, IFileReader reader, IConnectionProvider connectionProvider, ILogger logger)
            : base(serializer, reader, connectionProvider, logger)
        {
        }

        /// <summary>
        /// Logs the change.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="types">Types</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="repository">Git repository</param>
        /// <param name="currentPolicy">Branch policy</param>
        /// <param name="policy">Policy</param>
        /// <returns>Task</returns>
        #pragma warning disable 1998
        protected override async Task CreatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy)
        {
            this.Logger.Info($"Policy not found in Azure DevOps, would be created. (Repository: {repository.Name}, Branch: {currentPolicy.Branch}, Type: {policy.TypeString})");
            this.Logger.Debug($"Settings is: {policy.PrepareSettingsWithScopeAndSubType(repository.Id, policy)}");
        }
        #pragma warning restore 1998

        /// <summary>
        /// Logs the change.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="types">Types</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="repository">Git repository</param>
        /// <param name="currentPolicy">Branch policy</param>
        /// <param name="policy">Policy</param>
        /// <param name="serverPolicy">Server policy</param>
        /// <returns>Task</returns>
        #pragma warning disable 1998
        protected override async Task UpdatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy,
            PolicyConfiguration serverPolicy)
        {
            this.Logger.Info($"Policy found, would be updated in Azure DevOps. (Repository: {repository.Name}, Branch: {currentPolicy.Branch}, Type: {policy.TypeString})");
            this.Logger.Debug($"Settings is: {policy.PrepareSettingsWithScopeAndSubType(repository.Id, policy)}");
        }
        #pragma warning restore 1998

        /// <summary>
        /// Logs the change.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="policy">Policy</param>
        protected override void DeletePolicy(PolicyHttpClient policyClient, Guid projectId, PolicyConfiguration policy)
        {
            this.Logger.Info($"Policy not in the definition, would be removed from Azure DevOps. (Repository: {policy.GetRepositoryId()}, Branch: {policy.GetBranchFriendlyName()}, Type: {policy.Type.DisplayName})");
        }
    }
}
