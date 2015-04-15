namespace DevOpsFlex.Azure.Management
{
    using System;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    public enum VirtualMachineSize
    {
        Small,
        ExtraSmall,
        Large,
        Medium,
        ExtraLarge,
        A5,
        A6,
        A7,
        A8,
        A9,
        Stop
    }

    public static class VirtualMachineSizeExtensions
    {
        public static string ToAzureString(this VirtualMachineSize size)
        {
            var field = typeof(VirtualMachineRoleSize).GetField(size.ToString());

            if (field == null)
                throw new ArgumentOutOfRangeException(
                    "size",
                    "The Size used as part of the enumeration isn't supported in the Azure SDK. Please check the VirtualMachineRoleSize static class and make sure the VirtualMachineSize enum maps it.");

            return field.GetRawConstantValue().ToString();
        }
    }
}
