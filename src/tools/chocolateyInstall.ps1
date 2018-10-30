$ErrorActionPreference = 'Stop'

$toolsPath = Split-Path $MyInvocation.MyCommand.Definition

# Determine installation path
$pp = Get-PackageParameters
if (!$pp.InstallDir) { 
    $pp.InstallDir = "{0}\{1}" -f (Get-ToolsLocation), "AzureDevOpsPolicyConfigurator" 
}
$installDir = $pp.InstallDir

if (!(Test-Path $installDir)) {
    # Create destination directory if it doesn't exist
    Write-Host "Installing to:" $installDir
    New-Item -ItemType Directory -Force -Path $installDir
}
else {
    # If destination directory already exists, remove all except config files
    Write-Host "'$installDir' already exists and will be updated."
    Remove-Item $installDir\* -Recurse -Force
}

# Move files to install path
Get-ChildItem -Path $toolsPath\bin\* -Recurse | Move-Item -Destination $installDir

# Create custom pointer
$exepath = "{0}\{1}" -f $installDir, 'AzureDevOpsPolicyConfigurator.exe'
Install-BinFile -Name 'AzureDevOpsPolicyConfigurator' -Path $exepath

# Delete bin folder in package
Remove-Item -Path $toolsPath\bin -Recurse -ea 0