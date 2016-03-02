<#
    Root Empty module
#>

Import-Module .\AzurePrincipalWithCert.psm1
Import-Module .\ResizeASMDisk.psm1

Export-ModuleMember -Function @('New-AzurePrincipalWithCert', 'Remove-AzurePrincipalWithCert', 'Set-AzureVMOSDiskSize')