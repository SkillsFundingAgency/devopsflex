function Set-KeyVaultCertSecret
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $CertFolderPath,

        [parameter(Mandatory=$true)]
        [string] $CertPassword,

        [parameter(Mandatory=$true)]
        [string] $SecretName,

        [parameter(Mandatory=$true)]
        [string] $VaultName
    )

    $collection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
    $collection.Import($CertFolderPath, $CertPassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)

    $clearBytes = $collection.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12)
    $fileContentEncoded = [System.Convert]::ToBase64String($clearBytes)

    $secret = ConvertTo-SecureString -String $fileContentEncoded -AsPlainText –Force
    $secretContentType = 'application/x-pkcs12'

    Set-AzureKeyVaultSecret -VaultName $VaultName -Name $SecretName -SecretValue $secret -ContentType $secretContentType -Verbose
}

function New-AzurePrincipalWithCert
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $SystemName,

        [parameter(Mandatory=$true)]
        [ValidateSet('Configuration','Authentication')]
        [string] $CertPurpose,

        [parameter(Mandatory=$true)]
        [string] $EnvironmentName,

        [parameter(Mandatory=$true)]
        [string] $CertFolderPath,

        [parameter(Mandatory=$true)]
        [string] $CertPassword,

        [parameter(Mandatory=$true)]
        [string] $CertSuffix,

        [parameter(Mandatory=$true)]
        [string] $VaultSubscriptionId
    )

    # GUARD: AD application already exists
    $displayName = "$SystemName.$CertPurpose.$EnvironmentName"
    if((Get-AzureRmADApplication -DisplayNameStartWith $displayName) -ne $null) {
        throw 'An AD Application Already exists that looks identical to what you are trying to create'
    }

    # GUARD: Certificate system vault exists
    $certVaultName = "$SystemName-keyvault"
    if((Get-AzureRmKeyVault -VaultName $certVaultName) -eq $null) {
        throw "The system vault $certVaultName doesn't exist in the current subscription. Create it before running this cmdlet!"
    }

    # Create the self signed cert
    $currentDate = Get-Date
    $endDate = $currentDate.AddYears(1)
    $notAfter = $endDate.AddYears(1)
    $certCN = "$($SystemName.ToLower()).$($CertPurpose.ToLower()).$CertSuffix"

    if($CertFolderPath.EndsWith("\") -eq $false) {
        $CertFolderPath = $CertFolderPath + "\"
    }

    $cert = New-SelfSignedCertificate -CertStoreLocation cert:\localmachine\my `
                                      -DnsName $certCN `
                                      -KeyExportPolicy Exportable `
                                      -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider" `
                                      -NotAfter $notAfter -ErrorAction Stop

    $pwd = ConvertTo-SecureString -String $CertPassword -Force -AsPlainText
    $certPath = "$CertFolderPath$SystemName-$EnvironmentName.pfx"
    $void = Export-PfxCertificate -cert "cert:\localmachine\my\$($cert.Thumbprint)" -FilePath $certPath -Password $pwd -ErrorAction Stop

    # Load the certificate
    $cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate($certPath, $pwd)

    Import-Module AzureRM.Resources
    $keyCredential = New-Object  Microsoft.Azure.Commands.Resources.Models.ActiveDirectory.PSADKeyCredential
    $keyCredential.StartDate = $currentDate
    $keyCredential.EndDate= $endDate
    $keyCredential.KeyId = [guid]::NewGuid()
    $keyCredential.Type = 'AsymmetricX509Cert'
    $keyCredential.Usage = 'Verify'
    $keyCredential.Value = [System.Convert]::ToBase64String($cert.GetRawCertData())

    # Create the Azure Active Directory Application
    $azureAdApplication = New-AzureRmADApplication -DisplayName $displayName `
                                                   -HomePage "https://$($SystemName.ToLower()).$($CertPurpose.ToLower()).$($EnvironmentName.ToLower())" `
                                                   -IdentifierUris "https://$($SystemName.ToLower()).$($CertPurpose.ToLower()).$($EnvironmentName.ToLower())" `
                                                   -KeyCredentials $keyCredential `
                                                   -Verbose

    Write-Host -ForegroundColor DarkYellow 'Write down this ID because you will need it for future reference.'
    Write-Host -ForegroundColor DarkYellow 'You can still get it through PowerShell, but writing it down now will save you the hassle.'
    Write-Host -ForegroundColor Green  "Application ID: $($azureAdApplication.ApplicationId)"

    # Create the Service Principal and connect it to the Application
    $void = New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId

    # Switch to the KeyVault Techops-Management subscription
    $currentSubId = (Get-AzureRmContext).Subscription.SubscriptionId
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $VaultSubscriptionId -ErrorAction Stop
    }

    # Upload the cert and cert passwords to the right keyvaults
    $secretName = "$SystemName-$CertPurpose-$EnvironmentName"
    $void = Set-KeyVaultCertSecret -CertFolderPath $certPath -CertPassword $CertPassword -VaultName $certVaultName -SecretName $secretName
    $void = Set-AzureKeyVaultSecret -VaultName 'sfa-certpwds' -Name $secretName -SecretValue (ConvertTo-SecureString -String $CertPassword -AsPlainText –Force)

    # Swap back to the subscription the user was in
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $currentSubId | Out-Null
    }

    return $azureAdApplication
}



function Remove-AzurePrincipalWithCert
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$false)]
        [string] $ADApplicationId,

        [parameter(
            Mandatory=$false,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true)]
        [Microsoft.Azure.Commands.Resources.Models.ActiveDirectory.PSADApplication] $ADApplication,

        [parameter(Mandatory=$true)]
        [string] $VaultSubscriptionId
    )

    # GUARD: At least one parameter is supplied
    if((-not $ADApplicationId) -and (-not $ADApplication)) {
        throw 'You must either supply the PSADApplication object in the pipeline or the ApplicationID guid.'
    }

    if(-not $ADApplication) {
        $adApp = Get-AzureRmADApplication -ApplicationId $ADApplicationId

        # GUARD: If ADApplicationId is supplied, make sure it's a valid one that really exists
        if(-not $adApp) {
            throw "The specified ADApplicationID [$ADApplicationId] doesn't exist"
        }

        $identifierUri = $adApp.IdentifierUris[0]
    }
    else {
        $identifierUri = $ADApplication.IdentifierUris[0]
    }

    # Break the Identifier URI of the AD Application into it's individual components so that we can infer everything else.
    if(-not $identifierUri -match "https:\/\/([^.]*).([^.]*).([^.]*)") {
        throw "Can't infer the correct system information from the identifier URI [$identifierUri] in the AD Application, was this service principal created with this Module?"
    }

    $systemName = $Matches[1]
    $certPurpose = $Matches[2]
    $environmentName = $Matches[3]

    $dotName = "$systemName.$certPurpose.$environmentName"
    $dashName = "$systemName-$certPurpose-$environmentName"

    # Switch to the KeyVault Techops-Management subscription
    $currentSubId = (Get-AzureRmContext).Subscription.SubscriptionId
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $VaultSubscriptionId -ErrorAction Stop
    }

    # 1. Remove the cert from the system keyvault
    $certVaultName = "$systemName-keyvault"
    Remove-AzureKeyVaultSecret -VaultName $certVaultName -Name $dashName -Force -Confirm:$false

    # 2. Remove the cert password from the certs keyvault
    Remove-AzureKeyVaultSecret -VaultName 'sfa-certpwds' -Name $dashName -Force -Confirm:$false

    # Swap back to the subscription the user was in
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $currentSubId | Out-Null
    }

    # 3. Remove the AD Service Principal
    $servicePrincipal = Get-AzureRmADServicePrincipal -SearchString $dotName -ErrorAction SilentlyContinue
    if($servicePrincipal) {
        Remove-AzureRmADServicePrincipal -ObjectId $servicePrincipal.Id -Force
    }
    else {
        Write-Warning "Couldn't find any Service Principal using the search string [$dotName]"
    }

    # 4. Remove the AD Application
    $adApplication = Get-AzureRmADApplication -DisplayNameStartWith $dotName -ErrorAction SilentlyContinue
    if($adApplication) {
        Remove-AzureRmADApplication -ApplicationObjectId $adApplication.ApplicationObjectId -Force
    }
}
