using System;
using System.Collections.Generic;
using AzureDevOpsPolicyConfigurator.Data;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsPolicyConfigurator.Logic
{
    /// <summary>
    /// PolicyExecuter class.
    /// </summary>
    internal class PolicyExecuter : PolicyExecuterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyExecuter"/> class.
        /// </summary>
        /// <param name="serializer">Json serializer</param>
        /// <param name="reader">File reader</param>
        /// <param name="connectionProvider">Connection provider</param>
        /// <param name="logger">Logger</param>
        public PolicyExecuter(IJsonSerializer serializer, IFileReader reader, IConnectionProvider connectionProvider, ILogger logger)
            : base(serializer, reader, connectionProvider, logger)
        {
        }

        /// <summary>
        /// Creates a policy in Azure DevOps.
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
            var policyConfiguration = this.GetPolicyConfiguration(types, repository, policy);

            var async = policyClient.CreatePolicyConfigurationAsync(policyConfiguration, projectId);

            this.Logger.Debug(this.Serializer.Serialize(async.Result));
            this.Logger.Info($"Policy created. (Repository: {repository.Name}, Branch: {currentPolicy.Branch}, Type: {policy.TypeString})");
        }

        /// <summary>
        /// Updates a policy in Azure DevOps.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="types">Types</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="repository">Git repository</param>
        /// <param name="currentPolicy">Branch policy</param>
        /// <param name="policy">Policy</param>
        /// <param name="serverPolicy">Azure DevOps policy</param>
        protected override void UpdatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy,
            PolicyConfiguration serverPolicy)
        {
            var async = policyClient.UpdatePolicyConfigurationAsync(this.GetPolicyConfiguration(types, repository, policy), projectId, serverPolicy.Id);

            this.Logger.Debug(this.Serializer.Serialize(async.Result));
            this.Logger.Info($"Policy updated. (Repository: {repository.Name}, Branch: {currentPolicy.Branch}, Type: {policy.TypeString})");
        }

        /// <summary>
        /// Deletes a policy in Azure DevOps.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="policy">Policy</param>
        protected override void DeletePolicy(PolicyHttpClient policyClient, Guid projectId, PolicyConfiguration policy)
        {
            var async = policyClient.DeletePolicyConfigurationAsync(projectId, policy.Id);
            async.Wait();

            this.Logger.Info($"Policy removed. (Repository: {policy.GetRepositoryId()}, Branch: {policy.GetBranch()}, Type: {policy.Type.DisplayName})");
        }

        private PolicyConfiguration GetPolicyConfiguration(IEnumerable<PolicyType> types, GitRepository repository, Policy policy)
        {
            return new PolicyConfiguration()
            {
                Type = policy.Type.GetType(types),
                IsEnabled = true,
                IsBlocking = policy.IsBlocking,
                IsDeleted = false,
                Settings = policy.PrepareSettingsWithScopeAndSubType(repository.Id, policy)
            };
        }
    }
}
