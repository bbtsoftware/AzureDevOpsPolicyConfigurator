using System.Threading.Tasks;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy execution grouping tests.
    /// </summary>
    public class PolicyExecutionGroupingTests
    {
        [Fact(DisplayName = "Check project wide over", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckProjectWideOverGlobal()
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
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 4,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 4"));
        }

        [Fact(DisplayName = "Check repository over global and project wide", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckRepoOverGlobalAndProjectWide()
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
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 4,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 5,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 4"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 5"));
        }

        [Fact(DisplayName = "Check repository over global and project wide with branch", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckRepoOverGlobalAndProjectWideWithBranch()
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
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 4,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 5,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": ""master"",
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
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 4"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 5"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
        }

        [Fact(DisplayName = "Check repository over global and project wide with project specific branch over global branch", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckRepoOverGlobalAndProjectWideWithProjectSpecificBranchOverGlobalBranch()
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
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 4,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 5,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": ""master"",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 6,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": ""master"",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 7,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 4"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 5"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 7"));
        }

        [Fact(DisplayName = "Check repository over global and project wide with repository specific branch over global and project specific branch", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckRepoOverGlobalAndProjectWideWithRepoSpecificBranchOverGlobalAndProjectSpecificBranch()
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
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 4,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 5,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": ""master"",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 6,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": ""##Project##"",
                  ""branch"": ""master"",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 7,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": ""master"",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 8,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 4"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 5"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 7"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 8"));
        }

        [Fact(DisplayName = "Check star translation to prefix", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckStarTranslationToPrefix()
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
                  ""branch"": ""release/*"",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 3"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("heads/release/*"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("Prefix"));
        }

        [Fact(DisplayName = "Check other repository not included", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckOtherRepositoryNotIncluded()
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
                    ""minimumApproverCount"": 6,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                },
                {
                  ""type"": ""Minimum number of reviewers"",

                  ""project"": """",
                  ""branch"": ""master"",
                  ""repository"": ""myrepo"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 7,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 7"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"minimumApproverCount\": 6"));
        }

        [Fact(DisplayName = "Check branch policy ignored", Skip = SkippingInformation.SkippingReason)]
        private async Task CheckBranchPolicyIgnored()
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
                  ""branch"": ""blablabla"",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""minimumApproverCount"": 3,
                    ""creatorVoteCounts"": false,
                    ""allowDownvotes"": false,
                    ""resetOnSourcePush"": false
                  }
                }
              ]
            }")).ConfigureAwait(true);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("Policy branch ignored, because branch does not exist on the current repository."));
        }
    }
}
