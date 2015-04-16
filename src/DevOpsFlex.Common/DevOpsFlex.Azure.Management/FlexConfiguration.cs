namespace DevOpsFlex.Azure.Management
{
    using Choosing;
    using Data;
    using Data.Naming;

    /// <summary>
    /// Allows for configuration of the Naming pipeline for <see cref="DevOpsComponent"/>.
    /// </summary>
    public static class FlexConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        private static IChooseWebPlan _webPlanChooser = new DefaultWebPlanChooser();

        /// <summary>
        /// Overrides the <see cref="DefaultNaming{T}"/> for a specific <see cref="DevOpsComponent"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="DevOpsComponent"/> we want to override.</typeparam>
        /// <param name="naming">The <see cref="IName{T}"/> object that will name the overrides.</param>
        public static void UseNaming<T>(IName<T> naming)
            where T : DevOpsComponent
        {
            FlexDataConfiguration.UseNaming(naming);
        }

        /// <summary>
        /// Get the naming object for a specific <see cref="DevOpsComponent"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="DevOpsComponent"/>.</typeparam>
        /// <returns>The naming object.</returns>
        public static IName<T> GetNaming<T>()
            where T : DevOpsComponent
        {
            return FlexDataConfiguration.GetNaming<T>();
        }

        /// <summary>
        /// Overrides the <see cref="IChooseWebPlan"/> object in configuration.
        /// </summary>
        /// <param name="chooser"></param>
        public static void UseWebPlanChooser(IChooseWebPlan chooser)
        {
            _webPlanChooser = chooser;
        }

        /// <summary>
        /// Gets the <see cref="IChooseWebPlan"/> object in configuration.
        /// </summary>
        public static IChooseWebPlan WebPlanChooser
        {
            get { return _webPlanChooser; }
        }
    }
}
