###########################################################
#       New-AzurePrincipalWithSecret
###########################################################

function New-AzurePrincipalWithSecret
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true, Position=0)]
        [string] $SystemName,

        [parameter(Mandatory=$true, Position=1)]
        [ValidateSet('Configuration','Authentication')]
        [string] $PrincipalPurpose,

        [parameter(Mandatory=$true, Position=2)]
        [string] $EnvironmentName,

        [parameter(Mandatory=$true, Position=4)]
        [string] $PrincipalPassword,

        [parameter(Mandatory=$true, Position=5)]
        [string] $VaultSubscriptionId,

        [parameter(Mandatory=$false, Position=6)]
        [string] $PrincipalName
    )

    # GUARD: There are no non letter characters on the Cert Name
    if(-not [string]::IsNullOrWhiteSpace($CertName) -and -not $CertName -match "^([A-Za-z])*$") {
        throw 'The CertName must be letters only, either lower and upper case. Cannot contain any digits or any non-alpha-numeric characters.'
    }

    # Uniform all the namings
    if([string]::IsNullOrWhiteSpace($CertName)) {
        $principalIdDashed = "$($SystemName.ToLower())-$($PrincipalPurpose.ToLower())"
        $principalIdDotted = "$($SystemName.ToLower()).$($PrincipalPurpose.ToLower())"
        $identifierUri = "https://$($SystemName.ToLower()).$($PrincipalPurpose.ToLower()).$($EnvironmentName.ToLower())"
    }
    else {
        $principalIdDashed = "$($SystemName.ToLower())-$($PrincipalPurpose.ToLower())-$($PrincipalName.ToLower())"
        $principalIdDotted = "$($SystemName.ToLower()).$($PrincipalPurpose.ToLower()).$($PrincipalName.ToLower())"
        $identifierUri = "https://$($SystemName.ToLower()).$($PrincipalPurpose.ToLower()).$($EnvironmentName.ToLower()).$($PrincipalName.ToLower())"
    }

    # GUARD: AD application already exists
    if((Get-AzureRmADApplication -DisplayNameStartWith $principalIdDotted) -ne $null) {
        throw 'An AD Application Already exists that looks identical to what you are trying to create'
    }

    # GUARD: Certificate system vault exists
    $systemVaultName = "$SystemName-$EnvironmentName"
    if((Get-AzureRmKeyVault -VaultName $systemVaultName) -eq $null) {
        throw "The system vault $systemVaultName doesn't exist in the current subscription. Create it before running this cmdlet!"
    }

    # Aquire the tenant ID on the subscription we are creating the principal on, not on the vault subscription!
    $tenantId = (Get-AzureRmContext).Subscription.TenantId

    # Create the Azure Active Directory Application
    $azureAdApplication = New-AzureRmADApplication -DisplayName $principalIdDotted `
                                                   -HomePage $identifierUri `
                                                   -IdentifierUris $identifierUri `
                                                   -Password $PrincipalPassword `
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

    # Populate the system keyvault with all relevant principal configuration information
    $void = Set-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$principalIdDashed-TenantId" -SecretValue (ConvertTo-SecureString -String $tenantId -AsPlainText –Force)
    $void = Set-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$principalIdDashed-IdentifierUri" -SecretValue (ConvertTo-SecureString -String $identifierUri -AsPlainText –Force)
    $void = Set-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$principalIdDashed-ApplicationId" -SecretValue (ConvertTo-SecureString -String $($azureAdApplication.ApplicationId) -AsPlainText –Force)
    $void = Set-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$principalIdDashed-ApplicationSecret" -SecretValue (ConvertTo-SecureString -String $PrincipalPassword -AsPlainText –Force)

    # Swap back to the subscription the user was in
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $currentSubId | Out-Null
    }

    return $azureAdApplication
}

###########################################################
#       Remove-AzurePrincipalWithSecret
###########################################################

function Remove-AzurePrincipalWithSecret
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
        $dashName = "$systemName-$certPurpose"
        $dotName = "$systemName.$certPurpose"
    }
    else {
        $dashName = "$systemName-$certPurpose-$cerName"
        $dotName = "$systemName.$certPurpose.$cerName"
    }

    # Switch to the KeyVault Techops-Management subscription
    $currentSubId = (Get-AzureRmContext).Subscription.SubscriptionId
    if($currentSubId -ne $VaultSubscriptionId) {
        Select-AzureRmSubscription -SubscriptionId $VaultSubscriptionId -ErrorAction Stop
    }

    $systemVaultName = "$systemName-$environmentName"

    # 1. Remove the principal configuration information from the system keyvault
    Remove-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$dashName-TenantId" -Force -Confirm:$false
    Remove-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$dashName-IdentifierUri" -Force -Confirm:$false
    Remove-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$dashName-ApplicationId" -Force -Confirm:$false
    Remove-AzureKeyVaultSecret -VaultName $systemVaultName -Name "$dashName-ApplicationSecret" -Force -Confirm:$false

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
