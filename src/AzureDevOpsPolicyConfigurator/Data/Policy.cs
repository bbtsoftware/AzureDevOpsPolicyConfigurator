using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Policy.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureDevOpsPolicyConfigurator.Data
{
    /// <summary>
    /// Policy class.
    /// </summary>
    internal class Policy
    {
        private string branch;

        /// <summary>
        /// Gets or sets the policy name or id.
        /// </summary>
        public string Type { get; set; }

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
                if (value.EndsWith("*"))
                {
                    this.MatchKind = MatchKind.Prefix;
                    value = value.Substring(0, value.Length - 1);
                }

                this.branch = value;
            }
        }

        /// <summary>
        /// Gets or sets the match kind of a branch.
        /// </summary>
        public MatchKind MatchKind { get; set; }

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

        /// <summary>
        /// Returns the Settings property and adds scope
        /// </summary>
        /// <param name="repositoryId">Repository id</param>
        /// <returns>JObject</returns>
        public JObject SettingsWithScope(Guid repositoryId)
        {
            var settings = (JObject)this.Settings.DeepClone();
            settings.Add(this.CreateScope(repositoryId));
            return settings;
        }

        /// <summary>
        /// Creates the scope section of the Settings property.
        /// </summary>
        /// <param name="repositoryId">Repository id</param>
        /// <returns>JProperty</returns>
        public JProperty CreateScope(Guid repositoryId)
        {
            var scopeObject = new JObject(
                    new JProperty("refName", string.IsNullOrEmpty(this.Branch) ? string.Empty : "refs/heads/" + this.Branch),
                    new JProperty("repositoryId", repositoryId),
                    new JProperty("matchKind", this.MatchKind.ToString()));

            var scope = new JArray
            {
                scopeObject
            };

            return new JProperty("scope", scope);
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
            if (!serverConfiguration.IsEnabled && serverConfiguration.IsDeleted && serverConfiguration.IsBlocking != this.IsBlocking)
            {
                return false;
            }

            return this.JsonEquals(this.Settings, serverConfiguration.Settings);
        }

        /// <summary>
        /// Deepcompares the 2 JObject, and ignores scope property.
        /// </summary>
        /// <param name="a">Object a</param>
        /// <param name="b">Object b</param>
        /// <returns>Boolean</returns>
        private bool JsonEquals(JObject a, JObject b)
        {
            var clonedA = a.DeepClone();
            var clonedB = b.DeepClone();

            clonedA["scope"]?.Parent.Remove();
            clonedB["scope"]?.Parent.Remove();

            return JToken.DeepEquals(clonedA, clonedB);
        }
    }
}
