namespace DevOpsFlex.Data
{
    using System.ComponentModel;

    public enum ServiceBusRegions
    {
        [Description("Central US")] CentralUS,
        [Description("East US")] EastUS,
        [Description("East US 2")] EastUS2,
        [Description("North Central US")] NorthCentralUS,
        [Description("South Central US")] SouthCentralUS,
        [Description("West US")] WestUS,
        [Description("North Europe")] NorthEurope,
        [Description("West Europe")] WestEurope,
        [Description("East Asia")] EastAsia,
        [Description("Southeast Asia")] SoutheastAsia,
        [Description("Brazil South")] BrazilSouth,
        [Description("Japan East")] JapanEast,
        [Description("Japan West")] JapanWest,
    }
}
