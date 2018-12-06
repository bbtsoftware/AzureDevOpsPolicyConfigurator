using Xunit;

namespace AzureDevOpsPolicyConfigurator.Test
{
    /// <summary>
    /// Policy execution special test.
    /// </summary>
    public class PolicyExecutionSpecialTest
    {
        [Fact(DisplayName = "Check scope and subtype property creation", Skip = SkippingInformation.SkippingReason)]
        private void CheckScopeAndSubTypePropertyCreation()
        {
            var result = new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""ignoreTypes"": [],
              ""applyTo"": {
                ""projects"": [ ""Framework"" ],
                ""repositories"": [""doc.framework""]
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
            }"));

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("\"scope\": ["));
        }

        [Fact(DisplayName = "Check scope and subtype property removal", Skip = SkippingInformation.SkippingReason)]
        private void CheckScopeAndSubTypePropertyRemoval()
        {
            var result = new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""ignoreTypes"": [],
              ""applyTo"": {
                ""projects"": [ ""Framework"" ],
                ""repositories"": [""doc.framework""]
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
            }"));

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("create-new"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("myrefname"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("create-old"));
        }

        [Fact(DisplayName = "Check repository specific scope creation", Skip = SkippingInformation.SkippingReason)]
        private void CheckRepositorySpecificScopeCreation()
        {
            var result = new PolicyTester().RunTest(new TestData(@"
            {
              ""allowDeletion"":  false,
              ""ignoreTypes"": [],
              ""applyTo"": {
                ""projects"": [ ""Framework"" ],
                ""repositories"": [""doc.framework""]
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
            }"));

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("scope"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("refName"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("matchKind"));
        }
    }
}
