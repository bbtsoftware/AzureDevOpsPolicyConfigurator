using System;
using System.Collections.Generic;
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
        public WhatIfExecuter(IJsonSerializer serializer, IFileReader reader, IConnectionProvider connectionProvider)
            : base(serializer, reader, connectionProvider)
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
        protected override void CreatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy)
        {
            Log.Info($"Policy not found in Azure DevOps, would be created. (Branch: {currentPolicy.Branch}, Repository: {repository.Name}, Type: {policy.Type})");
            Log.Debug($"Settings is: {policy.SettingsWithScope(repository.Id)}");
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
        /// <param name="serverPolicy">Server policy</param>
        protected override void UpdatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy,
            PolicyConfiguration serverPolicy)
        {
            Log.Info($"Policy found, would be updated in Azure DevOps. (Branch: {currentPolicy.Branch}, Repository: {repository.Name}, Type: {serverPolicy.Type.DisplayName}, Branch: {serverPolicy.GetBranch()})");
            Log.Debug($"Settings is: {policy.SettingsWithScope(repository.Id)}");
        }

        /// <summary>
        /// Logs the change.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="policy">Policy</param>
        protected override void DeletePolicy(PolicyHttpClient policyClient, Guid projectId, PolicyConfiguration policy)
        {
            Log.Warn($"Policy not in the definition, would be removed from Azure DevOps. (Repository: {policy.GetRepositoryId()}, Type: {policy.Type.DisplayName}, Branch: {policy.GetBranch()})");
        }
    }
}
