using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Policy.WebApi;
using Newtonsoft.Json.Linq;

namespace AzureDevOpsPolicyConfigurator.Data
{
    /// <summary>
    /// Policy class.
    /// </summary>
    internal class Policy
    {
        private static readonly List<string> RepositorySpecificTypes = ["Git repository settings", "File size restriction"];
        private string branch;

        /// <summary>
        /// Gets or sets the policy name or id.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the policy type.
        /// </summary>
        public PolicyType PolicyType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the policy has SubType defined.
        /// </summary>
        public bool HasSubType => !string.IsNullOrEmpty(SubTypePropertyName);

        /// <summary>
        /// Gets or sets the policy subtype property name.
        /// </summary>
        public string SubTypePropertyName { get; set; }

        /// <summary>
        /// Gets or sets the policy subtype property value.
        /// </summary>
        public string SubTypePropertyValue { get; set; }

        /// <summary>
        /// Gets the UniquenessDefinition for the policy.
        /// </summary>
        public string UniquenessDefinition => $"{Type}{(SubTypePropertyValue != null ? "_" + SubTypePropertyValue : string.Empty)}";

        /// <summary>
        /// Gets the TypeString for the policy.
        /// </summary>
        public string TypeString => $"{PolicyType?.DisplayName ?? Type}{(SubTypePropertyValue != null ? "_" + SubTypePropertyValue : string.Empty)}";

        /// <summary>
        /// Gets or sets the policy project name or project id.
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the policy branch name.
        /// </summary>
        public string Branch
        {
            get => branch ?? string.Empty;
            set => branch = value;
        }

        /// <summary>
        /// Gets the ScopeBranch with 'refs/heads/' prefix.
        /// This can be different from <see cref="Branch"/> for <see cref="MatchKind"/>.Prefix.
        /// /* or * at the end will be truncated. Otherwise /* or * are also included in the branch name for the policy.
        /// e.g. refs/heads/release/abc* -> refs/heads/release/abc
        /// e.g. refs/heads/release/* -> refs/heads/release
        /// </summary>
        public string ScopeBranch
        {
            get
            {
                var branchName = "refs/heads/" + (Branch ?? string.Empty);

                if (MatchKind == MatchKind.Prefix)
                {
                    if (branchName.EndsWith("/*", StringComparison.Ordinal))
                    {
                        branchName = branchName[..^2];
                    }
                    else if (branchName.EndsWith('*'))
                    {
                        branchName = branchName[..^1];
                    }
                }

                return branchName;
            }
        }

        /// <summary>
        /// Gets the match kind of a branch.
        /// </summary>
        public MatchKind MatchKind => string.IsNullOrEmpty(Branch) || Branch.EndsWith('*') ? MatchKind.Prefix : MatchKind.Exact;

        /// <summary>
        /// Gets or sets the branches.
        /// </summary>
        public List<string> Branches { get; set; }

        /// <summary>
        /// Gets or sets the repository name or repository id.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets a list of repository names or repository ids.
        /// </summary>
        public List<string> Repositories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a blocking is needed.
        /// </summary>
        public bool IsBlocking { get; set; }

        /// <summary>
        /// Gets or sets the policy settings.
        /// </summary>
        public JObject Settings { get; set; }

        private bool IsRepositorySpecific => RepositorySpecificTypes.Contains(PolicyType.DisplayName);

        /// <summary>
        /// Returns the Settings property and adds scope
        /// </summary>
        /// <param name="repositoryId">Repository id</param>
        /// <param name="policy">Policy</param>
        /// <returns>Settings JObject</returns>
        public JObject PrepareSettingsWithScopeAndSubType(Guid repositoryId, Policy policy)
        {
            var settings = (JObject)Settings.DeepClone();
            settings.Remove("scope");
            settings.Add(IsRepositorySpecific ? CreateRepositorySpecificScope(repositoryId) : CreateScope(repositoryId));

            if (policy.HasSubType)
            {
                settings.Remove(policy.SubTypePropertyName);
                settings.Add(new JProperty(policy.SubTypePropertyName, policy.SubTypePropertyValue));
            }

            return settings;
        }

        /// <summary>
        /// Retuns true if every property defined in Settings in this object equals the equivalent PolicyConfigration.Settings property,
        /// and the IsBlocking property equals the PolicyConfiguration.IsBlocking
        /// and the PolicyConfiguration.IsEnabled is true and PolicyConfiguration.IsDeleted is false,
        /// otherwise false.
        /// </summary>
        /// <param name="serverConfiguration">Azure DevOps PolicyConfiguration</param>
        /// <returns>boolean</returns>
        public bool PolicyEquals(PolicyConfiguration serverConfiguration)
        {
            return serverConfiguration.IsEnabled && !serverConfiguration.IsDeleted && serverConfiguration.IsBlocking == IsBlocking && JsonEquals(serverConfiguration.Settings);
        }

        /// <summary>
        /// Prepares the PolicyType
        /// </summary>
        /// <param name="types">Types</param>
        public void PreparePolicyType(IEnumerable<PolicyType> types)
        {
            PolicyType = Type.GetPolicyType(types);
        }

        private JProperty CreateRepositorySpecificScope(Guid repositoryId)
        {
            var scopeObject = new JObject(new JProperty("repositoryId", repositoryId));

            var scope = new JArray
            {
                scopeObject
            };

            return new JProperty("scope", scope);
        }

        private JProperty CreateScope(Guid repositoryId)
        {
            var scopeObject = new JObject(
                    new JProperty("refName", ScopeBranch),
                    new JProperty("repositoryId", repositoryId),
                    new JProperty("matchKind", MatchKind.ToString()));

            var scope = new JArray
            {
                scopeObject
            };

            return new JProperty("scope", scope);
        }

        /// <summary>
        /// Deepcompares the 2 JObject, and ignores scope property.
        /// </summary>
        /// <param name="serverSettings">Server settings</param>
        /// <returns>Boolean</returns>
        private bool JsonEquals(JObject serverSettings)
        {
            var clonedA = Settings.DeepClone();
            var clonedB = serverSettings.DeepClone();

            clonedA["scope"]?.Parent.Remove();
            clonedB["scope"]?.Parent.Remove();

            if (HasSubType)
            {
                clonedA[SubTypePropertyName]?.Parent.Remove();
                clonedB[SubTypePropertyName]?.Parent.Remove();
            }

            return JToken.DeepEquals(clonedA, clonedB);
        }
    }
}
