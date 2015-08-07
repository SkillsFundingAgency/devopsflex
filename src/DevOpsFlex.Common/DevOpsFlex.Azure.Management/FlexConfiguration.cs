namespace DevOpsFlex.Azure.Management
{
    using System;
    using Choosing;
    using Data;
    using Data.Naming;

    /// <summary>
    /// Allows for configuration of the Naming pipeline for <see cref="DevOpsComponent"/>.
    /// </summary>
    public static class FlexConfiguration
    {
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
        /// Holds the Extension Id that is created and used when checking for the PaaS Diagnostics extension
        /// on a Cloud Service that's being deployed by DevOpsFlex.
        /// </summary>
        public const string FlexDiagnosticsExtensionId = "DevOpsFlex-PaaSDiagnostics";

        /// <summary>
        /// Gets the Azure SQL database SA username.
        /// </summary>
        public static string FlexSaUser => Environment.GetEnvironmentVariable(EnvFlexSaUser);

        /// <summary>
        /// Gets the Azure SQL database SA user password.
        /// </summary>
        public static string FlexSaPwd => Environment.GetEnvironmentVariable(EnvFlexSaPwd);

        /// <summary>
        /// Gets the Azure SQL database application user username.
        /// </summary>
        public static string FlexAppUser => Environment.GetEnvironmentVariable(EnvFlexAppUser);

        /// <summary>
        /// Gets the Azure SQL database application user password.
        /// </summary>
        public static string FlexAppPwd => Environment.GetEnvironmentVariable(EnvFlexAppPwd);

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
            WebPlanChooser = chooser;
        }

        /// <summary>
        /// Gets the <see cref="IChooseWebPlan"/> object in configuration.
        /// </summary>
        public static IChooseWebPlan WebPlanChooser { get; private set; } = new DefaultWebPlanChooser();

        /// <summary>
        /// Overrides the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        /// <param name="chooser">The current azure sql server chooser object.</param>
        public static void UseSqlServerChooser(IChooseSqlServer chooser)
        {
            SqlServerChooser = chooser;
        }

        /// <summary>
        /// Gets the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        public static IChooseSqlServer SqlServerChooser { get; private set; } = new DefaultAzureSqlServerChooser();

        /// <summary>
        /// Overrides the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        /// <param name="chooser">The current azure sql server chooser object.</param>
        public static void UseStorageAccountChooser(IChooseStorageAccount chooser)
        {
            StorageAccountChooser = chooser;
        }

        /// <summary>
        /// Gets the <see cref="IChooseSqlServer"/> object in configuration.
        /// </summary>
        public static IChooseStorageAccount StorageAccountChooser { get; private set; } = new DefaultStorageAccountChooser();
    }
}
