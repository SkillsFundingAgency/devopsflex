@{

# Script module or binary module file associated with this manifest.
RootModule = 'DevOpsFlex.PowerShell.dll'

# Version number of this module.
ModuleVersion = '0.1'

# ID used to uniquely identify this module
GUID = 'e4e3a0c3-715d-4c1f-8271-f2f974ee8504'

# Author of this module
Author = 'David Rodrigues'

# Company or vendor of this module
CompanyName = 'Skills Funding Agency (UK)'

# Copyright statement for this module
Copyright = '(C) 2015 Skills Funding Agency'

# Description of the functionality provided by this module
Description = 'Contains several commandlets that facilitate DevOps around release pipeline and Azure automation activities'

# Minimum version of the Windows PowerShell engine required by this module
# PowerShellVersion = ''

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module
DotNetFrameworkVersion = '4.6'

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = '4.0'

# Processor architecture (None, X86, Amd64) required by this module
ProcessorArchitecture = 'None'

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module
FunctionsToExport = '*'

# Cmdlets to export from this module
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = @('DevOps', 'Azure', 'TFS Build', 'Deployment')

        # A URL to the license for this module.
        LicenseUri = 'https://github.com/sfa-gov-uk/devopsflex/blob/master/LICENSE'

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/sfa-gov-uk/devopsflex'

        # A URL to an icon representing this module.
        IconUri = 'https://devopsflex.blob.core.windows.net/icons/devopsflex_50_50.png'

        # ReleaseNotes of this module
        ReleaseNotes = 'Initial add to the PowerShell gallery, contains: Push-CloudServices and Push-DevOpsFlexConfiguration'

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
HelpInfoURI = 'https://github.com/sfa-gov-uk/devopsflex/blob/master/README.md'

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}
