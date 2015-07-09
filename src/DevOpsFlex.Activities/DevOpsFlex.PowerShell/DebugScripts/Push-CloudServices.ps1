Import-Module .\DevOpsFlex.PowerShell.dll

Push-CloudServices -SubscriptionId "102d951b-78c0-4e48-80d4-a9c13baca2ad" `
					 -SettingsPath "C:\PublishSettings\sfa_beta.publishsettings" `
					 -StorageAccountName "djfrtest" `
					 -ServiceNames @("djfrtest1", "djfrtest2", "djfrtest3") `
					 -PackagePath @("C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\TemplateWebRole1.cspkg", `
						 "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\TemplateWebRole2.cspkg", `
						 "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\TemplateWebRole3.cspkg") `
					 -ConfigurationPath @("C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\ServiceConfiguration1.Cloud.cscfg", `
						 "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\ServiceConfiguration1.Cloud.cscfg", `
						 "C:\Projects\Training\TemplateWebRole\TemplateWebRole\bin\Release\app.publish\ServiceConfiguration1.Cloud.cscfg") `
					 -Verbose
