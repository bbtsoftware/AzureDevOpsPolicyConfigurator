using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy execution special tests.
    /// </summary>
    public class PolicyExecutionSpecialTests
    {
        [Fact(DisplayName = "Check scope and subtype property creation", Skip = SkippingInformation.SkippingReason)]
        private async void CheckScopeAndSubTypePropertyCreation()
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
                  ""type"": ""Status"",

                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""create-new"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""filenamePatterns"": []
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"scope\": ["));
        }

        [Fact(DisplayName = "Check scope and subtype property removal", Skip = SkippingInformation.SkippingReason)]
        private async void CheckScopeAndSubTypePropertyRemoval()
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
                  ""type"": ""Status"",

                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""create-new"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusName"": ""create-old"",
                    ""filenamePatterns"": [],
                    ""scope"": [
                        {
                            ""refName"": ""myrefname"",
                        }
                    ]
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("create-new"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("myrefname"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("create-old"));
        }

        [Fact(DisplayName = "Check repository specific scope creation", Skip = SkippingInformation.SkippingReason)]
        private async void CheckRepositorySpecificScopeCreation()
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
                  ""type"": ""Git repository settings"",

                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""create-new"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""enforceConsistentCase"": null,
                    ""rejectDotGit"": null,
                    ""optimizedByDefault"": null,
                    ""breadcrumbDays"": null,
                    ""allowedForkTargets"": 1,
                    ""gvfsOnly"": null,
                    ""gvfsExemptUsers"": null,
                    ""gvfsAllowedVersionRanges"": null
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("scope"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("refName"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("matchKind"));
        }
    }
}
