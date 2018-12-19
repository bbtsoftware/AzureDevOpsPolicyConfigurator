using System.Collections.Generic;
using AzureDevOpsPolicyConfigurator.Logic;
using Moq;

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
        public Dictionary<LogLevel, List<string>> RunTest(TestData testData)
        {
            Dictionary<LogLevel, List<string>> result = new Dictionary<LogLevel, List<string>>()
            {
                { LogLevel.Debug, new List<string>() },
                { LogLevel.Info, new List<string>() },
                { LogLevel.Warn, new List<string>() },
                { LogLevel.Error, new List<string>() }
            };

            var loggerMock = new Mock<ILogger>();
            var fileReaderMock = new Mock<IFileReader>();

            loggerMock.Setup(x => x.Debug(It.IsAny<string>())).Callback((string x) => result[LogLevel.Debug].Add(x));
            loggerMock.Setup(x => x.Info(It.IsAny<string>())).Callback((string x) => result[LogLevel.Info].Add(x));
            loggerMock.Setup(x => x.Warn(It.IsAny<string>())).Callback((string x) => result[LogLevel.Warn].Add(x));

            var testConfiguration = TestConfiguration.CurrentConfiguration;

            string fileContent =
                testData.FileContent
                .Replace("##Project##", testConfiguration.Project)
                .Replace("##Repository##", testConfiguration.Repository);

            fileReaderMock.Setup(x => x.GetFileContent("input.json")).Returns(fileContent);

            var whatIfExecuter = new WhatIfExecuter(new JsonSerializer(), fileReaderMock.Object, new ConnectionProvider(), loggerMock.Object);

            whatIfExecuter.Execute(new ExecuterSettings()
            {
                Auth = AuthMethod.Ntlm,
                CollectionUrl = testConfiguration.CollectionUrl,
                Input = "input.json",
                Verbosity = LogLevel.Debug
            });

            return result;
        }
    }
}
