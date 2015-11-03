Import-Module .\DevOpsFlex.PowerShell.dll

Copy-AzureTables -SourceAccountName "sourceAccount" `
                 -SourceAccountKey "sourceKey" `
                 -TargetAccountName "targetAccount" `
                 -TargetAccountKey "targetKey" `
                 -TableRegexFilter ".*" `
                 -Verbose
