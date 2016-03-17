$vaultSubscriptionId = '[ID HERE]'

Remove-Module -Name DevOpsFlex.Environments.PowerShell -ErrorAction SilentlyContinue -Verbose
Import-Module $PSScriptRoot\..\DevOpsFlex.Environments.PowerShell.psd1 -Force -Verbose

$azureAdApplication = New-AzurePrincipalWithCert -SystemName 'djfr' `
                                                 -CertPurpose 'Authentication' `
                                                 -EnvironmentName 'test' `
                                                 -CertFolderPath 'D:\Certificates' `
                                                 -CertPassword 'djfrpwd' `
                                                 -VaultSubscriptionId $vaultSubscriptionId

$azureAdApplication | Remove-AzurePrincipalWithCert -VaultSubscriptionId $vaultSubscriptionId
#Remove-AzurePrincipalWithCert -ADApplicationId [ID HERE] -VaultSubscriptionId $vaultSubscriptionId