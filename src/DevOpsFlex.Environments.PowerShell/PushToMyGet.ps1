param
(
    [parameter(Mandatory=$true)]
    [string] $ApiKey
)

Install-PackageProvider -Name NuGet -Force

$repo = Get-PSRepository -Name DevOpsFlex -ErrorAction SilentlyContinue
if($repo -eq $null) {
    Register-PSRepository -Name DevOpsFlex -SourceLocation "https://www.myget.org/F/devopsflex/api/v2" -InstallationPolicy Trusted
}

Publish-Module -Repository DevOpsFlex -Name .\DevOpsFlex.Environments.PowerShell\DevOpsFlex.Environments.PowerShell.psd1 -NuGetApiKey $ApiKey