# Azure DevOps policy configurator

This tool gets the current policies for projects, repositories and branches, or applies policy settings over a team project collection based on a JSON configuration file.

[![License](http://img.shields.io/:license-mit-blue.svg)](https://github.com/bbtsoftware/AzureDevOpsPolicyConfigurator/blob/develop/LICENSE)

## Information

| | Stable | Pre-release |
|:--:|:--:|:--:|
|GitHub Release|-|[![GitHub release](https://img.shields.io/github/release/bbtsoftware/AzureDevOpsPolicyConfigurator.svg)](https://github.com/bbtsoftware/AzureDevOpsPolicyConfigurator/releases/latest)|
|Chocolatey|[![Chocolatey](https://img.shields.io/chocolatey/v/azure-devops-policy-configurator.svg)](https://www.chocolatey.org/packages/azure-devops-policy-configurator)|[![Chocolatey](https://img.shields.io/chocolatey/vpre/azure-devops-policy-configurator.svg)](https://www.chocolatey.org/packages/azure-devops-policy-configurator)|

## Build Status

|Develop|Master|
|:--:|:--:|
|[![Build status](https://ci.appveyor.com/api/projects/status/koamlur75cwr4h6y/branch/develop?svg=true)](https://ci.appveyor.com/project/BBTSoftwareAG/azuredevopspolicyconfigurator/branch/develop)|[![Build status](https://ci.appveyor.com/api/projects/status/koamlur75cwr4h6y/branch/master?svg=true)](https://ci.appveyor.com/project/BBTSoftwareAG/azuredevopspolicyconfigurator/branch/master)|

## Documentation

### General

The tool is targeted to .Net Core 2.0 / .Net Standard 2.0, meaning, it can be used besides windows
machines also on linux, macs, etc. The tool is wrapped in a Chocolatey package, so it can be easily
installed onto a windows 10 machine, where the package builds an executable itself, and installs
it to the client machine. Nevertheless, the tool can be targeted onto any runtime.
See runtime catalog [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)

#### Installation

```ps
choco install azure-devops-policy-configurator
```

#### Generation to other targets

##### Windows 10 example

```ps
dotnet publish -c Release -r win10-x64
```

##### Linux example

```ps
dotnet publish -c Release -r ubuntu.16.10-x64
```

##### Mac example

```ps
dotnet publish -c Release -r osx.10.12-x64
```

### Generate current structure

 `AzureDevOpsPolicyConfigurator generate` command generates JSON files from the current policy
 structure. The tool iterates through every Team Project, gets every policy set groups them by
 repository and saves in the `policies.json` file. In the main directory, every project is saved
 to the `projects.json` file, and the tool also gets every type available in the current Team
 Project, it stores them into the `types.json` file in each project directory.

#### Generation example

```cmd
AzureDevOpsPolicyConfigurator generate --auth ntlm --collectionurl https://tfs.yourserver.ch/DefaultCollection
```

#### Command arguments for generation

| Option                       | Description
|------------------------------|---------------------------------------------------------------------------------------------------------------------
| `--collectionUrl` or `-c`    | The Team Collection URL, e.g.: `tfs.yourserver.ch/DefaultCollection`
| `--auth` or `-a`             | The authentication method, option values: `ntlm`, `oauth`, `basic`, `azureactivedirectory`
| `--user` or `-u`             | The user for authentication (optional for PAT authentication, or for `ntlm` authentication with current user)
| `--password` or `-p`         | The password, PAT or token for the authentication (optional for `ntlm` authentication with current user)
| `--verbosity` or `-v`        | Sets the log4net root level, option values: `info`, `debug`, `warn`, `error` (optional, default is `info`)
| `--destination` or `-d`      | optional, default value is `.\Project`

### Simulate and execute changes

 `AzureDevOpsPolicyConfigurator whatif` and `AzureDevOpsPolicyConfigurator execute` command executes
 a strategy stored in an input JSON file

#### Simulation example

```cmd
AzureDevOpsPolicyConfigurator whatif -a ntlm -c https://tfs.yourserver.ch/DefaultCollection -i Sample\\testfile.json
```

#### Execution example

```cmd
AzureDevOpsPolicyConfigurator execute -a ntlm -c https://tfs.yourserver.ch/DefaultCollection -i Sample\\testfile.json
```

#### Command arguments for execution

| Option                       | Description
|------------------------------|---------------------------------------------------------------------------------------------------------------------
| `--collectionUrl` or `-c`    | The Team Collection URL, e.g.: `tfs.yourserver.ch/DefaultCollection`
| `--auth` or `-a`             | The authentication method, option values: `ntlm`, `oauth`, `basic`, `azureactivedirectory`
| `--user` or `-u`             | The user for authentication (optional for PAT authentication, or for `ntlm` authentication with current user)
| `--password` or `-p`         | The password or PAT for the authentication (optional for `ntlm` authentication with current user)
| `--verbosity` or `-v`        | Sets the log4net root level, option values: `info`, `debug`, `warn`, `error` (optional, default is `info`)
| `--in` or `-i`               | the input JSON settings file (Multiple files can be specified with the delimiter: `;`. These files will be merged.)

### Input file

Policy definitions are stored in the input file, which can be defined globally, global for a project,
global for a repository, global for specific branches, global for specific branches in a project,
and for specific branches in a repository.
It is possible to ignore policies during the simulation or execution run, if some of them are set
or handled manually on the UI. Use the `ignoreTypes` section for that.
For project, repository and type definitions, both ids and entity names can be used to ease the
setting of the rules.

Azure DevOps stores the policies only on project level, nevertheless the tool gathers every definition and
takes and sets the most specific for a repository and type, meaning, first a branch and repository
specific, then, if that does not exist, a branch specific global policy. It applies then the
non-branch specific policies, the order is repository, then project specific, then global. The tool
goes through each and every repository in every project in the specified team collection and
updates it (if the rule is different, otherwise the rule won't be touched), creates the rule, or
deletes it, if it's not defined int the JSON file!

> Note:
> Do not define the `isEnabled` and `isDeleted` flags, these will be automatically set by the tool,
> and would lead to endless updates due to permanent difference!

There are types which can be used more then once on a repository (like `Status`), and defers only in
some sort of subtype. There is a possibility to specify these subtypes with the `subTypePropertyName`
and `subTypePropertyValue` properties in the definition, besides this, these policy definitions
can also be set hierarchically.

> Note:
> At creation or the scope property in the definition, if defined will be removed and set by the tool,
> also, if the subType feature is used, this property will be overriden by the tool. For comparison
> these properties are ignored.

The tool has been built defensive, meaning, deletion is not allowed by default. If you want to
enable this functionality, set the `allowDeletion` property to `true`. (The property is optional,
default is `false`)

If the tool should only touch specific project(s) or repository(ies), the restriction should be
added to the `applyTo` section of the yaml file. (The section and its properties is optional, can
be removed, if not needed.)

#### Input file example

```json
{
  "allowDeletion":  false,
  "ignoreTypes": [
    "Status"
  ],
  "applyTo": {
    "projects": [],
    "repositories": []
  },
  "policies": [
    {
        "type": "Minimum number of reviewers",

        "project": "",
        "branch": "",
        "repository": "",

        "isBlocking": true,

        "settings": {
            "minimumApproverCount": 2,
            "creatorVoteCounts": false,
            "allowDownvotes": false,
            "resetOnSourcePush": false
        }
    },
    {
        "type": "Minimum number of reviewers",

        "project": "MyProject",
        "branch": "",
        "repository": "",

        "isBlocking": true,

        "settings": {
            "minimumApproverCount": 2,
            "creatorVoteCounts": false,
            "allowDownvotes": false,
            "resetOnSourcePush": false
        }
    }
  ]
}
```

#### Global, for every project and repository

```json
{
    "type": "Minimum number of reviewers",

    "project": "",
    "branch": "",
    "repository": "",

    "isBlocking": true,

    "settings": {
        "minimumApproverCount": 2,
        "creatorVoteCounts": false,
        "allowDownvotes": false,
        "resetOnSourcePush": false
    }
}
```

#### Global, for every repository in the "MyProject" project

```json
{
    "type": "Minimum number of reviewers",

    "project": "MyProject",
    "branch": "",
    "repository": "",

    "isBlocking": true,

    "settings": {
        "minimumApproverCount": 2,
        "creatorVoteCounts": false,
        "allowDownvotes": false,
        "resetOnSourcePush": false
    }
}
```

#### For a repository (Can be more strict with specifying also a project)

```json
{
    "type": "Minimum number of reviewers",

    "project": "",
    "branch": "",
    "repository": "MyRepository",

    "isBlocking": true,

    "settings": {
        "minimumApproverCount": 2,
        "creatorVoteCounts": false,
        "allowDownvotes": false,
        "resetOnSourcePush": false
    }
}
```

#### Global, for every branch prefixed with the phrase "release/"

```json
{
    "type": "Require a merge strategy",

    "project": "",
    "branch": "release/*",
    "repository": "",

    "isBlocking": true,

    "settings": {
        "useSquashMerge":  false
    }
}
```

#### Global, for every "master" branch

```json
{
    "type": "Require a merge strategy",

    "project": "",
    "branch": "master",
    "matchKind": "exact",
    "repository": "",

    "isBlocking": true,

    "settings": {
        "useSquashMerge":  false
    }
}
```

#### Global, for every "master" and "release" branch

```json
{
    "type": "Require a merge strategy",

    "project": "",
    "branches": ["master", "release/*"],
    "matchKind": "exact",
    "repository": "",

    "isBlocking": true,

    "settings": {
        "useSquashMerge":  false
    }
}
```

#### For every feature branch under the "MyRepository" repository in the "MyProject" project

```json
{
    "type": "Require a merge strategy",

    "project": "MyProject",
    "branch": "feature/*",
    "repository": "MyRepository",

    "isBlocking": true,

    "settings": {
        "useSquashMerge":  false
    }
}
```

#### Subtype definition

```json
{
    "type": "Status",
    "subTypePropertyName": "statusName",
    "subTypePropertyValue": "work-in-progress",
    "isBlocking": true,
    "settings": {
        "statusGenre": "MyGenre",
        "authorId": "88148664-b0c6-4fab-bbd8-aae5d3e7d233",
        "invalidateOnSourceUpdate": false,
        "filenamePatterns": []
    }
},
{
    "type": "Status",
    "subTypePropertyName": "statusName",
    "subTypePropertyValue": "pullrequest-title",
    "isBlocking": true,
    "settings": {
        "statusName": "pullrequest-title",
        "statusGenre": "MyGenre",
        "authorId": "88148664-b0c6-4fab-bbd8-aae5d3e7d233",
        "invalidateOnSourceUpdate": false,
        "filenamePatterns": []
    }
}
```