$vaultSubscriptionId = '[YOUR-SUBSCRIPTION-ID-HERE]'

Remove-Module -Name DevOpsFlex.Environments.PowerShell -ErrorAction SilentlyContinue
Import-Module $PSScriptRoot\..\DevOpsFlex.Environments.PowerShell.psd1 -Force -Verbose

$azureAdApplication = New-AzurePrincipalWithCert -SystemName djfr `
                                                 -CertPurpose 'Authentication' `
                                                 -EnvironmentName 'test' `
                                                 -CertFolderPath 'D:\Certificates' `
                                                 -CertPassword 'djfrpwd' `
                                                 -CertSuffix 'sfa.bis.gov.uk' `
                                                 -VaultSubscriptionId $vaultSubscriptionId

$azureAdApplication | Remove-AzurePrincipalWithCert -VaultSubscriptionId $vaultSubscriptionId
#Remove-AzurePrincipalWithCert -ADApplicationId [ID-HERE]
