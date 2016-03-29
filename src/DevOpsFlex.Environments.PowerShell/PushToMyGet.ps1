param
(
    [parameter(Mandatory=$true, Position=0)]
    [string] $ApiKey
)

Install-PackageProvider -Name NuGet -Force -Scope CurrentUser
Import-PackageProvider -Name NuGet -Force -ForceBootstrap
Invoke-WebRequest -Uri "http://go.microsoft.com/fwlink/?LinkID=690216&clcid=0x409" -OutFile "C:\ProgramData\Microsoft\Windows\PowerShell\PowerShellGet\NuGet.exe"

$repo = Get-PSRepository -Name DevOpsFlex -ErrorAction SilentlyContinue -Debug
if($repo -eq $null) {
    Register-PSRepository -Name DevOpsFlex -SourceLocation "https://www.myget.org/F/devopsflex/api/v2" -InstallationPolicy Trusted
}

Publish-Module -Repository DevOpsFlex -Name .\DevOpsFlex.Environments.PowerShell\DevOpsFlex.Environments.PowerShell.psd1 -NuGetApiKey $ApiKey
