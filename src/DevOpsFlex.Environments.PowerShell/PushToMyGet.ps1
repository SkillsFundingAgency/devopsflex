param
(
    [parameter(Mandatory=$true, Position=0)]
    [string] $ApiKey
)

$repo = Get-PSRepository -Name DevOpsFlex -ErrorAction SilentlyContinue
if($repo -eq $null) {
    Register-PSRepository -Name DevOpsFlex -SourceLocation "https://www.myget.org/F/devopsflex/api/v2" -InstallationPolicy Trusted
}

Publish-Module -Repository DevOpsFlex -Name $PSScriptRoot\DevOpsFlex.Environments.PowerShell\DevOpsFlex.Environments.PowerShell.psd1 -NuGetApiKey $ApiKey
