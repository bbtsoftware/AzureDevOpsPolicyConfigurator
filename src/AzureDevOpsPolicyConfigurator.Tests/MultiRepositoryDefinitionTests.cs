using System.Collections.Generic;
using System.Linq;
using AzureDevOpsPolicyConfigurator.Data;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Multi branch definition test class.
    /// </summary>
    public class MultiRepositoryDefinitionTests
    {
        [Fact(DisplayName = "Check multi repository definition")]
        private void TestMultiRepositoryDefinition()
        {
            var serializer = new JsonSerializer();

            var policies = new List<Policy>()
            {
                new Policy()
                {
                    Repositories = new List<string>()
                    {
                        "myrepo1", "myrepo2"
                    }
                }
            };

            policies = policies.FlattenRepositories(serializer).ToList();

            Assert.Equal(2, policies.Count);
            Assert.Equal("myrepo1", policies[0].Repository);
            Assert.Null(policies[0].Repositories);
            Assert.Equal("myrepo2", policies[1].Repository);
            Assert.Null(policies[1].Repositories);
        }

        [Fact(DisplayName = "Check multi repository with overriding", Skip = SkippingInformation.SkippingReason)]
        private async void CheckMultiRepositoryWithOverriding()
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
                  ""repository"": ""##Repository##"",

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
                  ""repositories"": [ ""##Repository##"" ],

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 6,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
        }
    }
}
