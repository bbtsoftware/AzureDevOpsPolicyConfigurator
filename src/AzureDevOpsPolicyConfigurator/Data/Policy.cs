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
        private static readonly List<string> RepositorySpecificTypes = new List<string>() { "Git repository settings" };
        private string branch;
        private MatchKind matchKind;

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
        public bool HasSubType => !string.IsNullOrEmpty(this.SubTypePropertyName);

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
        public string UniquenessDefinition => $"{this.Type}{(this.SubTypePropertyValue != null ? "_" + this.SubTypePropertyValue : string.Empty)}";

        /// <summary>
        /// Gets the TypeString for the policy.
        /// </summary>
        public string TypeString => $"{this.PolicyType?.DisplayName ?? this.Type}{(this.SubTypePropertyValue != null ? "_" + this.SubTypePropertyValue : string.Empty)}";

        /// <summary>
        /// Gets or sets the policy project name or project id.
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the policy branch name.
        /// </summary>
        public string Branch
        {
            get => this.branch;
            set
            {
                if (value != null && value.EndsWith("*"))
                {
                    this.MatchKind = MatchKind.Prefix;
                }

                this.branch = value;
            }
        }

        /// <summary>
        /// Gets or sets the match kind of a branch.
        /// </summary>
        public MatchKind MatchKind
        {
            get
            {
                if (string.IsNullOrEmpty(this.Branch))
                {
                    this.matchKind = MatchKind.Prefix;
                }

                return this.matchKind;
            }
            set => this.matchKind = value;
        }

        /// <summary>
        /// Gets or sets the branches.
        /// </summary>
        public List<string> Branches { get; set; }

        /// <summary>
        /// Gets or sets the policy repository name or repository id.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a blocking is needed.
        /// </summary>
        public bool IsBlocking { get; set; }

        /// <summary>
        /// Gets or sets the policy settings.
        /// </summary>
        public JObject Settings { get; set; }

        private bool IsRepositorySpecific => RepositorySpecificTypes.Contains(this.PolicyType.DisplayName);

        private string ScopeBranch => "refs/heads/" + (this.Branch ?? string.Empty);

        /// <summary>
        /// Returns the Settings property and adds scope
        /// </summary>
        /// <param name="repositoryId">Repository id</param>
        /// <param name="policy">Policy</param>
        /// <returns>Settings JObject</returns>
        public JObject PrepareSettingsWithScopeAndSubType(Guid repositoryId, Policy policy)
        {
            var settings = (JObject)this.Settings.DeepClone();
            settings.Remove("scope");
            settings.Add(this.IsRepositorySpecific ? this.CreateRepositorySpecificScope(repositoryId) : this.CreateScope(repositoryId));

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
            if (!serverConfiguration.IsEnabled || serverConfiguration.IsDeleted || serverConfiguration.IsBlocking != this.IsBlocking)
            {
                return false;
            }

            return this.JsonEquals(serverConfiguration.Settings);
        }

        /// <summary>
        /// Prepares the PolicyType
        /// </summary>
        /// <param name="types">Types</param>
        public void PreparePolicyType(IEnumerable<PolicyType> types)
        {
            this.PolicyType = this.Type.GetPolicyType(types);
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
                    new JProperty("refName", this.ScopeBranch),
                    new JProperty("repositoryId", repositoryId),
                    new JProperty("matchKind", this.MatchKind.ToString()));

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
            var clonedA = this.Settings.DeepClone();
            var clonedB = serverSettings.DeepClone();

            clonedA["scope"]?.Parent.Remove();
            clonedB["scope"]?.Parent.Remove();

            if (this.HasSubType)
            {
                clonedA[this.SubTypePropertyName]?.Parent.Remove();
                clonedB[this.SubTypePropertyName]?.Parent.Remove();
            }

            return JToken.DeepEquals(clonedA, clonedB);
        }
    }
}
