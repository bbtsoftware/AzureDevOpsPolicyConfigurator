using System.Collections.Generic;
using AzureDevOpsPolicyConfigurator.Data;

namespace AzureDevOpsPolicyConfigurator.Logic
{
    /// <summary>
    /// BranchPolicies class.
    /// </summary>
    internal class BranchPolicies
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchPolicies"/> class.
        /// </summary>
        /// <param name="branch">Branch</param>
        /// <param name="policies">policies</param>
        public BranchPolicies(string branch, IEnumerable<Policy> policies)
        {
            this.Branch = branch;
            this.Policies = policies;
        }

        /// <summary>
        /// Gets the Branch.
        /// </summary>
        public string Branch { get; }

        /// <summary>
        /// Gets the Policies.
        /// </summary>
        public IEnumerable<Policy> Policies { get; }
    }
}
