using System.Collections.Generic;
using AzureDevOpsPolicyConfigurator.Data;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Json Merge Tests class.
    /// </summary>
    public class JsonMergeTests
    {
        [Fact(DisplayName = "Check JSON merge")]
        private void CheckJSONMerge()
        {
            var jsonSerializer = new JsonSerializer();

            var result = jsonSerializer.Deserialize<PolicyDefinition>(new List<string>
            {
@"{
    ""allowDeletion"":  false,
    ""ignoreTypes"": [],
    ""applyTo"": {
                ""projects"": [ ""MyProject"" ],
      ""repositories"": []
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
  }",
@"{
    ""applyTo"": {
        ""repositories"": [""MyRepository""]
    }
}" });

            Assert.Contains("MyProject", result.ApplyTo.Projects);
            Assert.Contains("MyRepository", result.ApplyTo.Repositories);
        }
    }
}
