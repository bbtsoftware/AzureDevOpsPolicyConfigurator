using System;
using System.Collections.Generic;
using System.Linq;
using AzureDevOpsPolicyConfigurator.Data;
using Microsoft.TeamFoundation.Policy.WebApi;

namespace AzureDevOpsPolicyConfigurator
{
    /// <summary>
    /// Extension class.
    /// </summary>
    internal static class PolicyExtension
    {
        /// <summary>
        /// Gets the type id by name or id if exists.
        /// </summary>
        /// /// <param name="typeIdOrName">type id or name</param>
        /// <param name="types">type array</param>
        /// <returns>PolicyType</returns>
        public static PolicyType GetPolicyType(this string typeIdOrName, IEnumerable<PolicyType> types)
        {
            return GetType(typeIdOrName, types);
        }

        /// <summary>
        /// Gets the type by name or id if exists.
        /// </summary>
        /// /// <param name="typeIdOrName">type id or name</param>
        /// <param name="types">type array</param>
        /// <returns>nullable guid</returns>
        public static PolicyType GetType(this string typeIdOrName, IEnumerable<PolicyType> types)
        {
            if (!Guid.TryParse(typeIdOrName, out var typeGuid))
            {
                return types.FirstOrDefault(x => x.DisplayName == typeIdOrName);
            }

            return types.FirstOrDefault(x => x.Id == typeGuid);
        }

        /// <summary>
        /// Returns the repository id from the settings.
        /// </summary>
        /// <param name="configuration">Policy configuration</param>
        /// <returns>repository id</returns>
        public static MatchKind? GetMatchKind(this PolicyConfiguration configuration)
        {
            var value = GetValueFromScopeElement(configuration, "matchKind");
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return Enum.Parse<MatchKind>(value, true);
        }

        /// <summary>
        /// Returns the repository id from the settings.
        /// </summary>
        /// <param name="configuration">Policy configuration</param>
        /// <returns>repository id</returns>
        public static string GetBranch(this PolicyConfiguration configuration)
        {
            return GetValueFromScopeElement(configuration, "refName")?.Replace("refs/heads/", string.Empty);
        }

        /// <summary>
        /// Returns the repository id from the settings.
        /// </summary>
        /// <param name="configuration">Policy configuration</param>
        /// <param name="policy">Policy</param>
        /// <returns>bool</returns>
        public static bool DoesSubTypeMatch(this PolicyConfiguration configuration, Policy policy)
        {
            if (!policy.HasSubType)
            {
                return true;
            }

            return configuration.Settings.Value<string>(policy.SubTypePropertyName) == policy.SubTypePropertyValue;
        }

        /// <summary>
        /// Flattens the policies by branches.
        /// </summary>
        /// <param name="policies">Policies</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>Flattened policies</returns>
        public static IEnumerable<Policy> FlattenBranches(this IEnumerable<Policy> policies, IJsonSerializer serializer)
        {
            var newPolicies = new List<Policy>();

            foreach (var policy in policies)
            {
                if (policy.Branches != null)
                {
                    foreach (var branch in policy.Branches)
                    {
                        var newPolicy = serializer.Clone(policy);
                        newPolicy.Branch = branch;
                        newPolicy.Branches = null;
                        newPolicies.Add(newPolicy);
                    }

                    continue;
                }

                newPolicies.Add(policy);
            }

            return newPolicies;
        }

        /// <summary>
        /// Flattens the policies by repositories.
        /// </summary>
        /// <param name="policies">Policies</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>Flattened policies</returns>
        public static IEnumerable<Policy> FlattenRepositories(this IEnumerable<Policy> policies, IJsonSerializer serializer)
        {
            var newPolicies = new List<Policy>();

            foreach (var policy in policies)
            {
                if (policy.Repositories != null)
                {
                    foreach (var repository in policy.Repositories)
                    {
                        var newPolicy = serializer.Clone(policy);
                        newPolicy.Repository = repository;
                        newPolicy.Repositories = null;
                        newPolicies.Add(newPolicy);
                    }

                    continue;
                }

                newPolicies.Add(policy);
            }

            return newPolicies;
        }

        /// <summary>
        /// Returns the branch from the settings.
        /// </summary>
        /// <param name="configuration">Policy configuration</param>
        /// <returns>branch</returns>
        public static string GetRepositoryId(this PolicyConfiguration configuration)
        {
            return GetValueFromScopeElement(configuration, "repositoryId");
        }

        private static string GetValueFromScopeElement(PolicyConfiguration configuration, string property)
        {
            return configuration.Settings["scope"].First.Value<string>(property);
        }
    }
}
