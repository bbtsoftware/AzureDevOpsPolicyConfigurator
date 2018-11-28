using System;
using System.Collections.Generic;
using System.Linq;
using AzureDevOpsPolicyConfigurator.Logic;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsPolicyConfigurator.Data
{
    /// <summary>
    /// Policies class.
    /// </summary>
    internal class PolicyDefinition
    {
        /// <summary>
        /// Gets or sets the list of policy type which should be ignored.
        /// </summary>
        public List<string> IgnoreTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deletion is allowed.
        /// </summary>
        public bool AllowDeletion { get; set; }

        /// <summary>
        /// Gets or sets the apply to restriction.
        /// </summary>
        public ApplyTo ApplyTo { get; set; }

        /// <summary>
        /// Gets or sets gets the list of policies.
        /// </summary>
        public List<Policy> Policies { get; set; }

        /// <summary>
        /// Determines whether the repository is allowed to be touched.
        /// </summary>
        /// <param name="guid">Guid to check</param>
        /// <param name="policyTypes">types</param>
        /// <returns>boolean</returns>
        public bool IgnoreType(Guid guid, List<PolicyType> policyTypes)
        {
            if (this.IgnoreTypes == null || this.IgnoreTypes.Count == 0)
            {
                return false;
            }

            return this.IgnoreTypes.Any(t => guid == t.GetPolicyTypeId(policyTypes));
        }

        /// <summary>
        /// Determines whether the repository is allowed to be touched.
        /// </summary>
        /// <param name="repostiroty">GitRepository</param>
        /// <returns>boolean</returns>
        public bool IsRepositoryAllowed(GitRepository repostiroty)
        {
            if (this.ApplyTo == null || this.ApplyTo.Repositories == null || this.ApplyTo.Repositories.Count == 0)
            {
                return true;
            }

            return this.ApplyTo.Repositories.Any(x => x == repostiroty.Name || (Guid.TryParse(x, out var guid) && guid == repostiroty.Id));
        }

        /// <summary>
        /// Determines whether the repository is allowed to be touched.
        /// </summary>
        /// <param name="project">Team project</param>
        /// <returns>boolean</returns>
        public bool IsProjectAllowed(TeamProjectReference project)
        {
            if (this.ApplyTo == null || this.ApplyTo.Projects == null || this.ApplyTo.Projects.Count == 0)
            {
                return true;
            }

            return this.ApplyTo.Projects.Any(x => x == project.Name || (Guid.TryParse(x, out var guid) && guid == project.Id));
        }
    }
}