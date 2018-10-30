using System;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Policy.WebApi;

namespace AzureDevOpsPolicyConfigurator.Logic
{
    /// <summary>
    /// Structure Generator class.
    /// </summary>
    internal class StructureGenerator : ILogicExecuter<GeneratorSettings>
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly JsonFileWriter jsonFileWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureGenerator"/> class.
        /// </summary>
        /// <param name="connectionProvider">Connection Provider</param>
        /// <param name="jsonFileWriter">JsonFileWriter instance</param>
        public StructureGenerator(JsonFileWriter jsonFileWriter, IConnectionProvider connectionProvider)
        {
            this.jsonFileWriter = jsonFileWriter;
            this.connectionProvider = connectionProvider;
        }

        /// <summary>
        /// Generates the policy structure in the given folder.
        /// </summary>
        /// <param name="arguments">Main arguments</param>
        public void Execute(GeneratorSettings arguments)
        {
            var mainFolder = arguments.Destination;

            using (var connection = this.connectionProvider.GetConnection(arguments))
            using (var projectClient = connection.GetClient<ProjectHttpClient>())
            using (var policyClient = connection.GetClient<PolicyHttpClient>())
            using (var gitRepositoryClient = connection.GetClient<Microsoft.TeamFoundation.SourceControl.WebApi.GitHttpClient>())
            {
                if (Directory.Exists(mainFolder))
                {
                    Directory.Delete(mainFolder, true);
                }

                Directory.CreateDirectory(mainFolder);

                var projects = projectClient.GetProjects();

                this.jsonFileWriter.SerializeAndWrite(projects.Result, $@"{mainFolder}\projects.json");

                foreach (var project in projects.Result)
                {
                    string currentProjectFolder = $@"{mainFolder}\{project.Name}";
                    Directory.CreateDirectory(currentProjectFolder);

                    var types = policyClient.GetPolicyTypesAsync(project.Id);
                    this.jsonFileWriter.SerializeAndWrite(types.Result, $@"{mainFolder}\{project.Name}\types.json");

                    var policyConfigurations = policyClient.GetPolicyConfigurationsAsync(project.Id);

                    var grouppedRepositories = policyConfigurations.Result.GroupBy(
                        x => x.Settings["scope"].First.Value<string>("repositoryId"));

                    foreach (var group in grouppedRepositories)
                    {
                        string repository;
                        try
                        {
                            repository = gitRepositoryClient.GetRepositoryAsync(new Guid(group.Key)).Result.Name;
                        }
                        catch (AggregateException)
                        {
                            repository = group.Key;
                        }

                        string currentRepositoryFolder = $@"{currentProjectFolder}\{repository}";
                        Directory.CreateDirectory(currentRepositoryFolder);

                        this.jsonFileWriter.SerializeAndWrite(
                            group.Select(
                                    x => new
                                    {
                                        x.Id,
                                        Type = x.Type.DisplayName,
                                        CreatedBy = x.CreatedBy.DisplayName,
                                        x.IsEnabled,
                                        x.IsBlocking,
                                        x.Settings
                                    }),
                                    $@"{currentRepositoryFolder}\policies.json");
                    }
                }
            }
        }
    }
}
