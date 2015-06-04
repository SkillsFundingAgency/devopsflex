namespace DevOpsFlex.Azure.Management
{
    using System.ComponentModel;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    /// <summary>
    /// Defines a VM size enumeration with a direct map to the Azure SDK constants.
    /// </summary>
    public enum VirtualMachineSize
    {
        [Description(VirtualMachineRoleSize.Small)]         Small,
        [Description(VirtualMachineRoleSize.ExtraSmall)]    ExtraSmall,
        [Description(VirtualMachineRoleSize.Large)]         Large,
        [Description(VirtualMachineRoleSize.Medium)]        Medium,
        [Description(VirtualMachineRoleSize.ExtraLarge)]    ExtraLarge,
        [Description(VirtualMachineRoleSize.A5)]            A5,
        [Description(VirtualMachineRoleSize.A6)]            A6,
        [Description(VirtualMachineRoleSize.A7)]            A7,
        [Description(VirtualMachineRoleSize.A8)]            A8,
        [Description(VirtualMachineRoleSize.A9)]            A9,
        Stop
    }
}
