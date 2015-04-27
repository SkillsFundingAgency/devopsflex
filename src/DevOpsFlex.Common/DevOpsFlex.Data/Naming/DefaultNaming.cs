namespace DevOpsFlex.Data.Naming
{
    using System;
    using System.Linq;
    using Core;

    /// <summary>
    /// Default naming strategy for dev ops components.
    /// </summary>
    /// <typeparam name="T">The type of the component that we are naming.</typeparam>
    public class DefaultNaming<T> : IName<T>
        where T : DevOpsComponent
    {
        /// <summary>
        /// Defines the maximum length for the system logical name;
        /// </summary>
        internal const int MaxSystemLength = 4;

        /// <summary>
        /// Defines the maximum length for the component logical name;
        /// </summary>
        internal const int MaxComponentLength = 10;

        /// <summary>
        /// Defines the maximum length for the configuration name;
        /// </summary>
        internal const int MaxConfigurationLength = 8;

        /// <summary>
        /// Slot name for medium to long names.
        /// </summary>
        /// <param name="component">The component that we want the generate the slot name for.</param>
        /// <param name="branch">The name of the branch we are deploying.</param>
        /// <param name="configuration">The name of the configuration we are deploying.</param>
        /// <returns>The slot name.</returns>
        public string GetSlotName(T component, string branch, string configuration)
        {
            var branchName = String.Equals(branch, FlexDataConfiguration.RootBranch, StringComparison.CurrentCultureIgnoreCase) ?
                "" :
                branch.GetOneCharOneDigit().ToLower();

            return string.Concat(component.System.LogicalName.ToLower().Take(MaxSystemLength)) +
                   branchName +
                   "-" +
                   string.Concat(component.LogicalName.ToLower().Take(MaxComponentLength)) +
                   "-" +
                   string.Concat(configuration.ToLower().Take(MaxConfigurationLength));
        }
    }
}
