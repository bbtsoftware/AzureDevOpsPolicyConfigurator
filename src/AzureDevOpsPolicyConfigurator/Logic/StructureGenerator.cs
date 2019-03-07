using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Execute(GeneratorSettings arguments)
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

                var projects = await projectClient.GetProjects().ConfigureAwait(false);

                this.jsonFileWriter.SerializeAndWrite(projects, $@"{mainFolder}\projects.json");

                foreach (var project in projects)
                {
                    var types = await policyClient.GetPolicyTypesAsync(project.Id).ConfigureAwait(false);
                    var policyConfigurations = await policyClient.GetPolicyConfigurationsAsync(project.Id).ConfigureAwait(false);

                    string currentProjectFolder = $@"{mainFolder}\{project.Name}";
                    Directory.CreateDirectory(currentProjectFolder);

                    this.jsonFileWriter.SerializeAndWrite(types, $@"{mainFolder}\{project.Name}\types.json");

                    var grouppedRepositories = policyConfigurations.GroupBy(
                        x => x.Settings["scope"].First.Value<string>("repositoryId"));

                    foreach (var group in grouppedRepositories)
                    {
                        string repository;
                        try
                        {
                            var repositoryObject = await gitRepositoryClient.GetRepositoryAsync(new Guid(group.Key)).ConfigureAwait(false);
                            repository = repositoryObject.Name;
                        }
                        catch (Exception)
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
