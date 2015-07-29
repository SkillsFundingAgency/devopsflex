Import-Module .\DevOpsFlex.PowerShell.dll

Set-ReservedIpConfiguration -SubscriptionId "102d951b-78c0-4e48-80d4-a9c13baca2ad" `
                            -SettingsPath "C:\PublishSettings\sfa_beta.publishsettings" `
                            -SqlConnectionString "Data Source=.;Initial Catalog=DevOpsFlex;Integrated Security=True;" `
                            -Branch "Main" `
                            -Configuration "AT" `
                            -TfsProjectMapPath "C:\Projects\FCT" `
                            -Verbose
