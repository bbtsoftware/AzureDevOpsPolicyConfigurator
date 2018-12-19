#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context, 
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    title: "AzureDevOpsPolicyConfigurator",
    repositoryOwner: "bbtsoftware",
    repositoryName: "AzureDevOpsPolicyConfigurator",
    appVeyorAccountName: "BBTSoftwareAG",
    shouldRunCodecov: false,
    shouldRunDupFinder: false);

BuildParameters.PrintParameters(Context);

Task("Publish-Application")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information(BuildParameters.SolutionFilePath.FullPath);
    Information(BuildParameters.Configuration);
    DotNetCorePublish(
        BuildParameters.SolutionFilePath.FullPath,
        new DotNetCorePublishSettings
        {
            Runtime = "win10-x64",
            Configuration = BuildParameters.Configuration,
            OutputDirectory = BuildParameters.Paths.Directories.Build + "/bin"
        });
});

BuildParameters.Tasks.CreateChocolateyPackagesTask.IsDependentOn("Publish-Application");

Build.RunDotNetCore();