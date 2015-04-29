namespace DevOpsFlex.Azure.Management
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Choosing;
    using Core.Events;
    using Data;
    using Data.Naming;

    /// <summary>
    /// Allows for configuration of the Naming pipeline for <see cref="DevOpsComponent"/>.
    /// </summary>
    public static class FlexConfiguration
    {
        /// <summary>
        /// Holds a static reference to the build event stream.
        /// </summary>
        private static readonly Subject<BuildEvent> BuildEventStream = new Subject<BuildEvent>();

        /// <summary>
        /// Holds a static reference to the current web plan chooser object.
        /// </summary>
        private static IChooseWebPlan _webPlanChooser = new DefaultWebPlanChooser();

        /// <summary>
        /// Holds a static reference to the current azure sql server chooser object.
        /// </summary>
        private static IChooseSqlServer _sqlServerChooser = new DefaultAzureSqlServerChooser();

        /// <summary>
        /// Holds the environment variable name for the Azure SQL database SA username.
        /// </summary>
        internal const string EnvFlexSaUser = "FlexSaUser";

        /// <summary>
        /// Holds the environment variable name for the Azure SQL database SA user password.
        /// </summary>
        internal const string EnvFlexSaPwd = "FlexSaPwd";

        /// <summary>
        /// Holds the environment variable name for the Azure SQL database application user username.
        /// </summary>
        internal const string EnvFlexAppUser = "FlexAppUser";

        /// <summary>
        /// Holds the environment variable name for the Azure SQL database application user password.
        /// </summary>
        internal const string EnvFlexAppPwd = "FlexAppPwd";

        /// <summary>
        /// Gets the build event stream as an <see cref="IObserver{T}"/>.
        /// </summary>
        public static IObserver<BuildEvent> BuildEventsObserver
        {
            get { return BuildEventStream.AsObserver(); }
        }

        /// <summary>
        /// Gets the build event stream as an <see cref="IObservable{T}"/>.
        /// </summary>
        public static IObservable<BuildEvent> BuildEventsObservable
        {
            get { return BuildEventStream.AsObservable(); }
        }

        /// <summary>
        /// Gets the Azure SQL database SA username.
        /// </summary>
        public static string FlexSaUser
        {
            get { return Environment.GetEnvironmentVariable(EnvFlexSaUser); }
        }

        /// <summary>
        /// Gets the Azure SQL database SA user password.
        /// </summary>
        public static string FlexSaPwd
        {
            get { return Environment.GetEnvironmentVariable(EnvFlexSaPwd); }
        }

        /// <summary>
        /// Gets the Azure SQL database application user username.
        /// </summary>
        public static string FlexAppUser
        {
            get { return Environment.GetEnvironmentVariable(EnvFlexAppUser); }
        }

        /// <summary>
        /// Gets the Azure SQL database application user password.
        /// </summary>
        public static string FlexAppPwd
        {
            get { return Environment.GetEnvironmentVariable(EnvFlexAppPwd); }
        }

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
        /// <param name="chooser">The current web plan chooser object.</param>
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

        /// <summary>
        /// Overrides the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        /// <param name="chooser">The current azure sql server chooser object.</param>
        public static void UseSqlServerChooser(IChooseSqlServer chooser)
        {
            _sqlServerChooser = chooser;
        }

        /// <summary>
        /// Gets the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        public static IChooseSqlServer SqlServerChooser
        {
            get { return _sqlServerChooser; }
        }
    }
}
