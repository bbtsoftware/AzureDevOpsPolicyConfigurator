using Xunit;

namespace AzureDevOpsPolicyConfigurator.Test
{
    /// <summary>
    /// Policy execution test.
    /// </summary>
    public class PolicyExecutionSubGroupingTest
    {
        [Fact(DisplayName = "Check project wide over for subgrouping", Skip = SkippingInformation.SkippingReason)]
        private void CheckProjectWideOverGlobal()
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""Framework"",
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
            }"));

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
        }

        [Fact(DisplayName = "Check repository over global and project wide for subgrouping", Skip = SkippingInformation.SkippingReason)]
        private void CheckRepoOverGlobalAndProjectWide()
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""Framework"",
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""Framework"",
                  ""branch"": """",
                  ""repository"": ""doc.framework"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity3"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                }
              ]
            }"));

            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.DoesNotContain(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity3"));
        }

        [Fact(DisplayName = "Check no interaction between grouping types", Skip = SkippingInformation.SkippingReason)]
        private void CheckNoInteractionBeetweenGroupingTypes()
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
                  ""SubTypePropertyValue"": ""pr-title"",

                  ""project"": ""Framework"",
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
                  
                  ""SubTypePropertyName"": ""pr-statusName"",
                  ""SubTypePropertyValue"": ""pr-title2"",

                  ""project"": ""Framework"",
                  ""branch"": """",
                  ""repository"": ""doc.framework"",

                  ""isBlocking"": true,

                  ""settings"": {
                    ""statusGenre"": ""TeamCity3"",
                    ""authorId"": ""ecba3d80-fec6-4826-9f11-f4dc6cbd4d17"",
                    ""invalidateOnSourceUpdate"": true,
                  }
                }
              ]
            }"));

            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity1"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity2"));
            Assert.Contains(result[LogLevel.Debug], x => x.Contains("TeamCity3"));
        }
    }
}
