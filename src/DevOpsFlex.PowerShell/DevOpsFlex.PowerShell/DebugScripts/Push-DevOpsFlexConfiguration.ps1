Import-Module .\DevOpsFlex.PowerShell.dll

Push-DevOpsFlexConfiguration -SubscriptionId "c991f126-2b16-4a92-af05-85a68e4b9719" `
                             -SettingsPath "C:\PublishSettings\djfr.publishsettings" `
                             -SqlConnectionString "Data Source=.;Initial Catalog=DevOpsFlex;Integrated Security=True;" `
                             -Branch "Main" `
                             -Configuration "DJ" `
                             -Verbose
