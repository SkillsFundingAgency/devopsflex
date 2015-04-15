namespace DevOpsFlex.Data.Naming
{
    using Core;

    /// <summary>
    /// Default naming strategy for dev ops components.
    /// </summary>
    /// <typeparam name="T">The type of the component that we are naming.</typeparam>
    public class DefaultNaming<T> : IName<T>
        where T : DevOpsComponent
    {
        /// <summary>
        /// Slot name for medium to long names.
        /// </summary>
        /// <param name="component">The component that we want the generate the slot name for.</param>
        /// <returns>The slot name.</returns>
        public string GetSlotName(T component)
        {
            return component.System.LogicalName.ToLower() + "-" + component.LogicalName.ToLower();
        }
    }
}
