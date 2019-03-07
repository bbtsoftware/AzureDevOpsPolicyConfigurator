using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy execution sub grouping tests.
    /// </summary>
    public class PolicyExecutionSubGroupingTests
    {
        [Fact(DisplayName = "Check project wide over for subgrouping", Skip = SkippingInformation.SkippingReason)]
        private async void CheckProjectWideOverGlobal()
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
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity1"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                },
                {
                  ""type"": ""Status"",
                  
                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity2"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
        }

        [Fact(DisplayName = "Check repository over global and project wide for subgrouping", Skip = SkippingInformation.SkippingReason)]
        private async void CheckRepoOverGlobalAndProjectWide()
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
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity1"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                },
                {
                  ""type"": ""Status"",
                  
                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity2"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                },
                {
                  ""type"": ""Status"",
                  
                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity3"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity3"));
        }

        [Fact(DisplayName = "Check no interaction between grouping types", Skip = SkippingInformation.SkippingReason)]
        private async void CheckNoInteractionBeetweenGroupingTypes()
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

                  ""project"": """",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity1"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                },
                {
                  ""type"": ""Status"",
                  
                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": """",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity2"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                },
                {
                  ""type"": ""Status"",
                  
                  ""SubTypePropertyName"": ""statusName"",
                  ""SubTypePropertyValue"": ""pr-title2"",

                  ""project"": ""##Project##"",
                  ""branch"": """",
                  ""repository"": ""##Repository##"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity3"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                }
              ]
            }")).ConfigureAwait(false);

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity3"));
        }
    }
}
