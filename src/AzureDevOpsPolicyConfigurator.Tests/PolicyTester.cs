using System.Collections.Generic;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Logic;
using FakeItEasy;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Policy tester class.
    /// </summary>
    internal class PolicyTester
    {
        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="testData">Test data</param>
        /// <returns>A dictionary of logs grouped by LogLevel.</returns>
        public async Task<Dictionary<LogLevel, List<string>>> RunTest(TestData testData)
        {
            Dictionary<LogLevel, List<string>> result = new Dictionary<LogLevel, List<string>>()
            {
                { LogLevel.Debug, new List<string>() },
                { LogLevel.Info, new List<string>() },
                { LogLevel.Warn, new List<string>() },
                { LogLevel.Error, new List<string>() }
            };

            var loggerMock = A.Fake<ILogger>();
            var fileReaderMock = A.Fake<IFileReader>();

            A.CallTo(() => loggerMock.Debug(A<string>._)).Invokes((string x) => result[LogLevel.Debug].Add(x));
            A.CallTo(() => loggerMock.Info(A<string>._)).Invokes((string x) => result[LogLevel.Info].Add(x));
            A.CallTo(() => loggerMock.Warn(A<string>._)).Invokes((string x) => result[LogLevel.Warn].Add(x));

            var testConfiguration = TestConfiguration.CurrentConfiguration;

            string fileContent =
                testData.FileContent
                .Replace("##Project##", testConfiguration.Project)
                .Replace("##Repository##", testConfiguration.Repository);

            A.CallTo(() => fileReaderMock.GetFileContent("input.json")).Returns(fileContent);

            var whatIfExecuter = new WhatIfExecuter(new JsonSerializer(), fileReaderMock, new ConnectionProvider(), loggerMock);

            await whatIfExecuter.Execute(new ExecuterSettings()
            {
                Auth = AuthMethod.Ntlm,
                CollectionUrl = testConfiguration.CollectionUrl,
                Input = "input.json",
                Verbosity = LogLevel.Debug
            }).ConfigureAwait(true);

            return result;
        }
    }
}
