Import-Module .\DevOpsFlex.PowerShell.dll

Push-CloudServices -SubscriptionId "102d951b-78c0-4e48-80d4-a9c13baca2ad" `
					 -SettingsPath "C:\PublishSettings\sfa_beta.publishsettings" `
					 -ServiceName "djfrtest" `
					 -StorageAccountName "djfrtest" `
					 -PackagePath "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\TemplateWebRole.cspkg" `
					 -ConfigurationPath "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\ServiceConfiguration.Cloud.cscfg" `
					 -Verbose
