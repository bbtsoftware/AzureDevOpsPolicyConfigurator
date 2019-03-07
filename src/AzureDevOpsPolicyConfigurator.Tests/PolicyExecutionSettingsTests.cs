using System.Linq;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy execution settings tests.
    /// </summary>
    public class PolicyExecutionSettingsTests
    {
        [Fact(DisplayName = "Check startings, apply to and disallow deletion", Skip = SkippingInformation.SkippingReason)]
        private async void CheckStartingAndSkippings()
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
            }")).ConfigureAwait(false);

            Assert.NotEmpty(result);
            Assert.NotEmpty(result[LogLevel.Debug]);
            Assert.NotEmpty(result[LogLevel.Info]);

            Assert.Contains(result[LogLevel.Info], x => x == "Starting project: Framework");
            Assert.Contains(result[LogLevel.Info], x => x == "Starting repository: doc.framework");
            Assert.Contains(result[LogLevel.Info], x => x.StartsWith("Existing policy not defined. Skipping removal due to allowDeletion Flag."));

            Assert.Contains(result[LogLevel.Debug], x => x.StartsWith("Skipping repository"));
            Assert.Contains(result[LogLevel.Debug], x => x.StartsWith("Skipping project"));

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
            }")).ConfigureAwait(false);

            Assert.True(result[LogLevel.Info].Count(x => x.StartsWith("Starting project")) > 1);
            Assert.True(result[LogLevel.Info].Count(x => x.StartsWith("Starting repository")) > 1);
            Assert.DoesNotContain(result[LogLevel.Info], x => x.StartsWith("Existing policy not defined. Skipping removal due to allowDeletion Flag."));

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("Skipping repository"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("Skipping project"));
        }

        [Fact(DisplayName = "Check Ignoring Types", Skip = SkippingInformation.SkippingReason)]
        private async void CheckIgnoreTypes()
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
            }")).ConfigureAwait(false);

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
            }")).ConfigureAwait(false);

            Assert.Contains(result[LogLevel.Info], x => x.Contains("Type: Status"));
        }
    }
}
