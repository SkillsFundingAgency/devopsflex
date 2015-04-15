namespace DevOpsFlex.Activities
{
    using Azure.Management;

    public class VmScaleDefinition
    {
        public string Name { get; set; }

        public VirtualMachineSize Size { get; set; }
    }
}
