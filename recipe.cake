#load nuget:?package=Cake.Recipe&version=1.0.0

Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    title: "AzureDevOpsPolicyConfigurator",
    repositoryOwner: "bbtsoftware",
    repositoryName: "AzureDevOpsPolicyConfigurator",
    appVeyorAccountName: "BBTSoftwareAG",
    shouldRunCodecov: true,
    shouldRunDupFinder: false);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(
    context: Context,
    testCoverageFilter: "+[AzureDevOpsPolicyConfigurator]*",
    testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
    testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

Task("Publish-Application")
    .IsDependentOn("DotNetCore-Build")
    .Does(() =>
{
    var projectPath = "./src/AzureDevOpsPolicyConfigurator/AzureDevOpsPolicyConfigurator.csproj";

    Information("Publishing {0}", projectPath);

    DotNetCorePublish(
        projectPath,
        new DotNetCorePublishSettings
        {
            Runtime = "win10-x64",
            Configuration = BuildParameters.Configuration,
            OutputDirectory = BuildParameters.Paths.Directories.Build + "/bin"
        });
});

Task("Prepare-Chocolatey-Packages")
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.Build + "/temp/_Packages");
    CopyFile("./LICENSE", BuildParameters.Paths.Directories.Build + "/temp/_Packages/LICENSE.txt");
});

BuildParameters.Tasks.CreateChocolateyPackagesTask
    .IsDependentOn("Publish-Application")
    .IsDependentOn("Prepare-Chocolatey-Packages");

Build.RunDotNetCore();