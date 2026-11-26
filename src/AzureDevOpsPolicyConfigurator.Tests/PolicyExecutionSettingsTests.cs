using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy execution settings tests.
    /// </summary>
    public class PolicyExecutionSettingsTests
    {
        [Fact(DisplayName = "Check startings, apply to and disallow deletion", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckStartingAndSkippings()
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

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 2,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.NotEmpty(result);
            Assert.NotEmpty(result[LogLevel.Debug]);
            Assert.NotEmpty(result[LogLevel.Info]);

            Assert.Contains(result[LogLevel.Info], x => x == "Starting project: Framework");
            Assert.Contains(result[LogLevel.Info], x => x == "Starting repository: doc.framework");
            Assert.Contains(result[LogLevel.Info], x => x.StartsWith("Existing policy not defined. Skipping removal due to allowDeletion Flag.", StringComparison.OrdinalIgnoreCase));

            Assert.Contains(result[LogLevel.Debug], x => x.StartsWith("Skipping repository", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result[LogLevel.Debug], x => x.StartsWith("Skipping project", StringComparison.OrdinalIgnoreCase));

            result = await new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  true,
              ""policies"": [
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 2,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.True(result[LogLevel.Info].Count(x => x.StartsWith("Starting project", StringComparison.OrdinalIgnoreCase)) > 1);
            Assert.True(result[LogLevel.Info].Count(x => x.StartsWith("Starting repository", StringComparison.OrdinalIgnoreCase)) > 1);
            Assert.DoesNotContain(result[LogLevel.Info], x => x.StartsWith("Existing policy not defined. Skipping removal due to allowDeletion Flag.", StringComparison.OrdinalIgnoreCase));

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("Skipping repository", StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("Skipping project", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Check Ignoring Types", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckIgnoreTypes()
        {
            var result = await new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""ignoreTypes"": [ ""Status"" ],
              ""applyTo"": {
                ""projects"": [ ""##Project##"" ],
                ""repositories"": [""##Repository##""]
              },
              ""policies"": [
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 2,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Info], x => x.Contains("Type: Status"));

            result = await new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""applyTo"": {
                ""projects"": [ ""##Project##"" ],
                ""repositories"": [""##Repository##""]
              },
              ""policies"": [
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 2,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.Contains(result[LogLevel.Info], x => x.Contains("Type: Status", StringComparison.OrdinalIgnoreCase));
        }
    }
}
