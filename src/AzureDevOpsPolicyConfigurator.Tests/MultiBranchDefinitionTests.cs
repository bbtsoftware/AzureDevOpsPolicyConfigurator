﻿using System.Collections.Generic;
using System.Linq;
using AzureDevOpsPolicyConfigurator.Data;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Multi branch definition test class.
    /// </summary>
    public class MultiBranchDefinitionTests
    {
        [Fact(DisplayName = "Check multi branch definition")]
        private void TestMultiBranchDefinition()
        {
            var serializer = new JsonSerializer();

            var policies = new List<Policy>()
            {
                new Policy()
                {
                    Branches = new List<string>()
                    {
                        "master", "release/my/master"
                    }
                }
            };

            policies = policies.FlattenBranches(serializer).ToList();

            Assert.Equal(2, policies.Count);
            Assert.Equal("master", policies[0].Branch);
            Assert.Null(policies[0].Branches);
            Assert.Equal("release/my/master", policies[1].Branch);
            Assert.Null(policies[1].Branches);
        }

        [Fact(DisplayName = "Check multi branch with overriding", Skip = SkippingInformation.SkippingReason)]
        private async void CheckMultiBranchWithOverriding()
        {
            var result = await new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""ignoreTypes"": [],
              ""applyTo"": {
                ""projects"": [ ""##Project##"" ],
                ""repositories"": [""##Repository##""]
              },
              ""policies"": [
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": ""master"",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branches"": [""master""],
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 6,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
        }
    }
}
