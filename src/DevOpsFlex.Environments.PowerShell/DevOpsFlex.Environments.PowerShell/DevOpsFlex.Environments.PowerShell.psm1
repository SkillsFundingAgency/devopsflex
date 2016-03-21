<#
    Root Empty module
#>

$rootModulePath = Split-Path $script:MyInvocation.MyCommand.Path
Import-Module "$rootModulePath\AzurePrincipalWithCert.psm1"
Import-Module "$rootModulePath\AzurePrincipalWithSecret.psm1"
Import-Module "$rootModulePath\AzureADHelpers.psm1"
Import-Module "$rootModulePath\ResizeASMDisk.psm1"

Export-ModuleMember -Function @('New-AzurePrincipalWithCert', 'Remove-AzurePrincipalWithCert', 'New-AzurePrincipalWithSecret', 'Remove-AzurePrincipalWithSecret', 'Get-MyUserObjectId', 'Add-MeToKeyvault', 'Set-AzureVMOSDiskSize')