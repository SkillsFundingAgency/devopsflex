$vaultSubscriptionId = 'b7e3924d-3b0c-4268-bcc6-b70557fa9f3c'

Remove-Module -Name DevOpsFlex.Environments.PowerShell -ErrorAction SilentlyContinue
Import-Module $PSScriptRoot\..\DevOpsFlex.Environments.PowerShell.psd1 -Force -Verbose

$azureAdApplication = New-AzurePrincipalWithCert -SystemName 'djfr' `
                                                 -CertPurpose 'Authentication' `
                                                 -EnvironmentName 'test' `
                                                 -CertFolderPath 'D:\Certificates' `
                                                 -CertPassword 'djfrpwd' `
                                                 -CertSuffix 'sfa.bis.gov.uk' `
                                                 -VaultSubscriptionId $vaultSubscriptionId

$azureAdApplication | Remove-AzurePrincipalWithCert -VaultSubscriptionId $vaultSubscriptionId
#Remove-AzurePrincipalWithCert -ADApplicationId [ID-HERE]
