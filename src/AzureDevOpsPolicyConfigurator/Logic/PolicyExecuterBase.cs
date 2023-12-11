using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Data;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsPolicyConfigurator.Logic
{
    /// <summary>
    /// ApplyPolicyset class.
    /// </summary>
    internal abstract class PolicyExecuterBase : ILogicExecuter<ExecuterSettings>
    {
        private readonly IFileReader reader;
        private readonly IConnectionProvider connectionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyExecuterBase"/> class.
        /// </summary>
        /// <param name="serializer">JsonSerialzer</param>
        /// <param name="reader">FileReader</param>
        /// <param name="connectionProvider">Connection provider</param>
        /// <param name="logger">Logger</param>
        public PolicyExecuterBase(IJsonSerializer serializer, IFileReader reader, IConnectionProvider connectionProvider, ILogger logger)
        {
            this.Serializer = serializer;
            this.reader = reader;
            this.connectionProvider = connectionProvider;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets serializer
        /// </summary>
        protected IJsonSerializer Serializer { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Executer
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Execute(ExecuterSettings arguments)
        {
            var contents = arguments.Input.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(x => this.reader.GetFileContent(x.Trim()));

            var policyDefinition = this.Serializer.Deserialize<PolicyDefinition>(contents);

            using (var connection = this.connectionProvider.GetConnection(arguments))
            using (var projectClient = connection.GetClient<ProjectHttpClient>())
            using (var policyClient = connection.GetClient<PolicyHttpClient>())
            using (var gitRepositoryClient = connection.GetClient<GitHttpClient>())
            {
                var projects = await projectClient.GetProjects().ConfigureAwait(false);

                foreach (var project in projects)
                {
                    var getPolicyConfigurationsTask = policyClient.GetPolicyConfigurationsAsync(project.Id);
                    var getPolicyTypesTask = policyClient.GetPolicyTypesAsync(project.Id);
                    var getRepositoriesTask = gitRepositoryClient.GetRepositoriesAsync(project.Id);

                    if (!policyDefinition.IsProjectAllowed(project))
                    {
                        this.Logger.Debug($"Skipping project \"{project.Name}\", because it's not listed in the allowed projects list");
                        continue;
                    }

                    this.Logger.Info($"Starting project: {project.Name}");

                    var types = await getPolicyTypesTask.ConfigureAwait(false);
                    var grouppedRepositories =
                        (await getPolicyConfigurationsTask.ConfigureAwait(false))
                            .Where(x => !policyDefinition.IgnoreType(x.Type.Id, types))
                            .GroupBy(x => x.GetRepositoryId());

                    foreach (var repository in await getRepositoriesTask.ConfigureAwait(false))
                    {
                        var branches = await gitRepositoryClient.GetBranchesAsync(repository.Id).ConfigureAwait(false);

                        if (!policyDefinition.IsRepositoryAllowed(repository))
                        {
                            this.Logger.Debug($"Skipping repository \"{repository.Name}\", because it's not listed in the allowed repositories list");
                            continue;
                        }

                        this.Logger.Info($"Starting repository: {repository.Name}");

                        var relevantPolicies = this.GetPoliciesForRepository(policyDefinition.Policies, project, repository);
                        var serverPolicy = grouppedRepositories.FirstOrDefault(x => x.Key == repository.Id.ToString());

                        await this.HandleChanges(policyDefinition.AllowDeletion, policyClient, project.Id, repository, branches, relevantPolicies, serverPolicy, types).ConfigureAwait(false);
                    }
                }
            }
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
        /// <returns>Task</returns>
        protected abstract Task CreatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy);

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
        /// <returns>Task</returns>
        protected abstract Task UpdatePolicy(
            PolicyHttpClient policyClient,
            IEnumerable<PolicyType> types,
            Guid projectId,
            GitRepository repository,
            BranchPolicies currentPolicy,
            Policy policy,
            PolicyConfiguration serverPolicy);

        /// <summary>
        /// Deletes a policy in Azure DevOps.
        /// </summary>
        /// <param name="policyClient">Policy client</param>
        /// <param name="projectId">Team project id</param>
        /// <param name="policy">Policy</param>
        protected abstract void DeletePolicy(PolicyHttpClient policyClient, Guid projectId, PolicyConfiguration policy);

        private async Task HandleChanges(
            bool allowDeletion,
            PolicyHttpClient policyClient,
            Guid projectId,
            GitRepository repository,
            List<GitBranchStats> branches,
            IEnumerable<BranchPolicies> relevantPolicies,
            IEnumerable<PolicyConfiguration> serverPolicy,
            IEnumerable<PolicyType> types)
        {
            var handledServerPolicies = new List<int>();
            var resultList = new List<Task>();

            foreach (var currentPolicy in relevantPolicies)
            {
                // Ignore exact branches not found on the repository
                if (string.IsNullOrEmpty(currentPolicy.Branch) ||
                    currentPolicy.Branch.Contains("*") ||
                    branches.Any(x => x.Name == currentPolicy.Branch))
                {
                    this.HandleBranchChanges(policyClient, projectId, repository, serverPolicy, types, handledServerPolicies, currentPolicy, resultList);
                }
                else
                {
                    this.Logger.Debug($"Policy branch ignored, because branch does not exist on the current repository. (Repository: {repository.Name}, Branch: {currentPolicy.Branch})");
                }
            }

            this.RemoveNoMatchServerPolicies(allowDeletion, policyClient, projectId, serverPolicy, handledServerPolicies);

            await Task.WhenAll(resultList).ConfigureAwait(false);
        }

        private void HandleBranchChanges(
            PolicyHttpClient policyClient,
            Guid projectId,
            GitRepository repository,
            IEnumerable<PolicyConfiguration> serverPolicies,
            IEnumerable<PolicyType> types,
            List<int> handledServerPolicies,
            BranchPolicies currentPolicy,
            List<Task> resultList)
        {
            foreach (var policy in currentPolicy.Policies)
            {
                bool hasMatch = false;

                policy.PreparePolicyType(types);

                if (policy.PolicyType == null)
                {
                    this.Logger.Warn($"Type not found. (Branch: {currentPolicy.Branch}, Type: {policy.Type})");
                }

                if (serverPolicies != null)
                {
                    foreach (var serverPolicy in serverPolicies)
                    {
                        if (policy.PolicyType.Id == serverPolicy.Type.Id && serverPolicy.DoesSubTypeMatch(policy) &&
                                ((serverPolicy.GetBranch() == null) || // Repository specific
                                (serverPolicy.GetBranch() == policy.ScopeBranch && serverPolicy.GetMatchKind() == policy.MatchKind)))
                        {
                            hasMatch = true;
                            handledServerPolicies.Add(serverPolicy.Id);

                            if (policy.PolicyEquals(serverPolicy))
                            {
                                this.Logger.Info($"Policy is up to date. (Repository: {repository.Name}, Branch: {currentPolicy.Branch}, Type: {policy.TypeString})");
                            }
                            else
                            {
                                resultList.Add(this.UpdatePolicy(policyClient, types, projectId, repository, currentPolicy, policy, serverPolicy));
                            }

                            break;
                        }
                    }
                }

                if (!hasMatch)
                {
                    resultList.Add(this.CreatePolicy(policyClient, types, projectId, repository, currentPolicy, policy));
                }
            }
        }

        private void RemoveNoMatchServerPolicies(bool allowDeletion, PolicyHttpClient policyClient, Guid projectId, IEnumerable<PolicyConfiguration> serverPolicy, List<int> handledServerPolicies)
        {
            if (serverPolicy != null)
            {
                var removeables = serverPolicy.Where(x => handledServerPolicies.All(htp => x.Id != htp));

                foreach (var removeable in removeables)
                {
                    if (!allowDeletion)
                    {
                        this.Logger.Info($"Existing policy not defined. Skipping removal due to allowDeletion Flag. (Repository: {removeable.GetRepositoryId()}, Branch: {removeable.GetBranchFriendlyName()}, Type: {removeable.Type.DisplayName})");
                        continue;
                    }

                    this.DeletePolicy(policyClient, projectId, removeable);
                }
            }
        }

        private IEnumerable<BranchPolicies> GetPoliciesForRepository(
            IEnumerable<Policy> policies,
            TeamProjectReference project,
            GitRepository repository)
        {
            return policies
                .FlattenRepositories(this.Serializer)
                .FlattenBranches(this.Serializer)
                .Where(x =>
                        (string.IsNullOrEmpty(x.Project) || // global or branch specific
                        (x.Project.ToLower() == project.Name.ToLower() || x.Project.ToLower() == project.Id.ToString().ToLower())) && // project or project branch specific
                        (string.IsNullOrEmpty(x.Repository) || x.Repository.ToLower() == repository.Name.ToLower() || x.Repository.ToLower() == repository.Id.ToString().ToLower())) // repository
                .GroupBy(
                    x => x.Branch,
                    (e, ee) => new BranchPolicies(e, ee.GroupBy(x => x.UniquenessDefinition, (te, tee) => tee.OrderBy(x => x, new PolicyPriorityComparer()).First())));
        }

        /// <summary>
        /// Policy priority comparer.
        /// </summary>
        private class PolicyPriorityComparer : IComparer<Policy>
        {
            public int Compare(Policy x, Policy y)
            {
                // Repository
                if (x.Repository != null)
                {
                    return -1;
                }

                // Repository
                if (y.Repository != null)
                {
                    return 1;
                }

                // Global project
                if (x.Project != null)
                {
                    return -1;
                }

                // Global project
                if (y.Project != null)
                {
                    return 1;
                }

                return -1;
            }
        }
    }
}
