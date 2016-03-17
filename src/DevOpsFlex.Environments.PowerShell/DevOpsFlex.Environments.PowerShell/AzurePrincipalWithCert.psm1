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

###########################################################
#       New-AzurePrincipalWithCert
###########################################################

function New-AzurePrincipalWithCert
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true, Position=0)]
        [string] $SystemName,

        [parameter(Mandatory=$true, Position=1)]
        [ValidateSet('Configuration','Authentication')]
        [string] $CertPurpose,

        [parameter(Mandatory=$true, Position=2)]
        [string] $EnvironmentName,

        [parameter(Mandatory=$true, Position=3)]
        [string] $CertFolderPath,

        [parameter(Mandatory=$true, Position=4)]
        [string] $CertPassword,

        [parameter(Mandatory=$true, Position=5)]
        [string] $VaultSubscriptionId,

        [parameter(Mandatory=$false, Position=6)]
        [string] $CertName
    )

    # Uniform all the namings
    if([string]::IsNullOrWhiteSpace($CertName)) {
        $principalIdDashed = "$($SystemName.ToLower())-$($CertPurpose.ToLower())-$($EnvironmentName.ToLower())"
        $principalIdDotted = "$($SystemName.ToLower()).$($CertPurpose.ToLower()).$($EnvironmentName.ToLower())"
    }
    else {
        $principalIdDashed = "$($SystemName.ToLower())-$($CertPurpose.ToLower())-$($EnvironmentName.ToLower())-$($CertName.ToLower())"
        $principalIdDotted = "$($SystemName.ToLower()).$($CertPurpose.ToLower()).$($EnvironmentName.ToLower()).$($CertName.ToLower())"
    }

    # GUARD: There are no non letter characters on the Cert Name
    if(-not [string]::IsNullOrWhiteSpace($CertName) -and -not $CertName -match "^([A-Za-z])*$") {
        throw 'The CertName must be letters only, either lower and upper case. Cannot contain any digits or any non-alpha-numeric characters.'
    }

    # GUARD: AD application already exists
    if((Get-AzureRmADApplication -DisplayNameStartWith $principalIdDotted) -ne $null) {
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

    if($CertFolderPath.EndsWith("\") -eq $false) {
        $CertFolderPath = $CertFolderPath + "\"
    }

    $cert = New-SelfSignedCertificate -CertStoreLocation cert:\localmachine\my `
                                      -DnsName $principalIdDotted `
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

    # Aquire the tenant ID on the subscription we are creating the principal on, not on the vault subscription!
    $tenantId = (Get-AzureRmContext).Subscription.TenantId

    # Create the Azure Active Directory Application
    $identifierUri = "https://$principalIdDotted"
    $azureAdApplication = New-AzureRmADApplication -DisplayName $principalIdDotted `
                                                   -HomePage $identifierUri `
                                                   -IdentifierUris $identifierUri `
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
    $void = Set-KeyVaultCertSecret -CertFolderPath $certPath -CertPassword $CertPassword -VaultName $certVaultName -SecretName "$principalIdDashed-Cert"
    $void = Set-AzureKeyVaultSecret -VaultName 'sfa-certpwds' -Name $principalIdDashed -SecretValue (ConvertTo-SecureString -String $CertPassword -AsPlainText –Force)

    # Populate the system keyvault with all relevant principal configuration information
    $void = Set-AzureKeyVaultSecret -VaultName $certVaultName -Name "$principalIdDashed-TenantId" -SecretValue (ConvertTo-SecureString -String $tenantId -AsPlainText –Force)
    $void = Set-AzureKeyVaultSecret -VaultName $certVaultName -Name "$principalIdDashed-IdentifierUri" -SecretValue (ConvertTo-SecureString -String $identifierUri -AsPlainText –Force)
    $void = Set-AzureKeyVaultSecret -VaultName $certVaultName -Name "$principalIdDashed-ApplicationId" -SecretValue (ConvertTo-SecureString -String $($azureAdApplication.ApplicationId) -AsPlainText –Force)

    # Swap back to the subscription the user was in
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $currentSubId | Out-Null
    }

    return $azureAdApplication
}

###########################################################
#       Remove-AzurePrincipalWithCert
###########################################################

function Remove-AzurePrincipalWithCert
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$false, Position=0)]
        [string] $ADApplicationId,

        [parameter(
            Mandatory=$false,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true,
            Position=1)]
        [Microsoft.Azure.Commands.Resources.Models.ActiveDirectory.PSADApplication] $ADApplication,

        [parameter(Mandatory=$true, Position=2)]
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
    if(-not ($identifierUri -match 'https:\/\/(?<system>[^.]*).(?<purpose>[^.]*).(?<environment>[^.]*).*(?<certname>[^.]*)')) {
        throw "Can't infer the correct system information from the identifier URI [$identifierUri] in the AD Application, was this service principal created with this Module?"
    }

    $systemName = $Matches['system']
    $certPurpose = $Matches['purpose']
    $environmentName = $Matches['environment']
    $certName = $Matches['certname']

    # Uniform all the namings
    if([string]::IsNullOrWhiteSpace($certName)) {
        $dashName = "$systemName-$certPurpose-$environmentName"
        $dotName = "$systemName.$certPurpose.$environmentName"
    }
    else {
        $dashName = "$systemName-$certPurpose-$environmentName-$cerName"
        $dotName = "$systemName.$certPurpose.$environmentName.$cerName"
    }

    # Switch to the KeyVault Techops-Management subscription
    $currentSubId = (Get-AzureRmContext).Subscription.SubscriptionId
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $VaultSubscriptionId -ErrorAction Stop
    }

    # 1. Remove the cert from the system keyvault
    $certVaultName = "$systemName-keyvault"
    Remove-AzureKeyVaultSecret -VaultName $certVaultName -Name "$dashName-Cert" -Force -Confirm:$false

    # 2. Remove the principal configuration information from the system keyvault
    Remove-AzureKeyVaultSecret -VaultName $certVaultName -Name "$dashName-TenantId" -Force -Confirm:$false
    Remove-AzureKeyVaultSecret -VaultName $certVaultName -Name "$dashName-IdentifierUri" -Force -Confirm:$false
    Remove-AzureKeyVaultSecret -VaultName $certVaultName -Name "$dashName-ApplicationId" -Force -Confirm:$false

    # 3. Remove the cert password from the certs keyvault
    Remove-AzureKeyVaultSecret -VaultName 'sfa-certpwds' -Name $dashName -Force -Confirm:$false

    # Swap back to the subscription the user was in
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $currentSubId | Out-Null
    }

    # 4. Remove the AD Service Principal
    $servicePrincipal = Get-AzureRmADServicePrincipal -SearchString $dotName -ErrorAction SilentlyContinue
    if($servicePrincipal) {
        Remove-AzureRmADServicePrincipal -ObjectId $servicePrincipal.Id -Force
    }
    else {
        Write-Warning "Couldn't find any Service Principal using the search string [$dotName]"
    }

    # 5. Remove the AD Application
    $adApplication = Get-AzureRmADApplication -DisplayNameStartWith $dotName -ErrorAction SilentlyContinue
    if($adApplication) {
        Remove-AzureRmADApplication -ApplicationObjectId $adApplication.ApplicationObjectId -Force
    }
}
