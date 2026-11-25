using System.Collections.Generic;
using System.Threading.Tasks;
using AzureDevOpsPolicyConfigurator.Logic;
using NSubstitute;

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
            Dictionary<LogLevel, List<string>> result = new()
            {
                { LogLevel.Debug, new List<string>() },
                { LogLevel.Info, new List<string>() },
                { LogLevel.Warn, new List<string>() },
                { LogLevel.Error, new List<string>() }
            };

            var loggerMock = Substitute.For<ILogger>();
            var fileReaderMock = Substitute.For<IFileReader>();

            loggerMock.When(x => x.Debug(Arg.Any<string>())).Do(x => result[LogLevel.Debug].Add(x.Arg<string>()));
            loggerMock.When(x => x.Info(Arg.Any<string>())).Do(x => result[LogLevel.Info].Add(x.Arg<string>()));
            loggerMock.When(x => x.Warn(Arg.Any<string>())).Do(x => result[LogLevel.Warn].Add(x.Arg<string>()));

            var testConfiguration = TestConfiguration.CurrentConfiguration;

            string fileContent =
                testData.FileContent
                .Replace("##Project##", testConfiguration.Project)
                .Replace("##Repository##", testConfiguration.Repository);

            fileReaderMock.GetFileContent("input.json").Returns(fileContent);

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
