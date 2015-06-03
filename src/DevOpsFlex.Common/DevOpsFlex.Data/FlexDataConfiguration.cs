namespace DevOpsFlex.Data
{
    using System;
    using System.Collections.Generic;
    using Naming;

    /// <summary>
    /// Allows for configuration of the Naming pipeline for <see cref="DevOpsComponent"/>.
    /// </summary>
    public static class FlexDataConfiguration
    {
        /// <summary>
        /// Holds a <see cref="Type"/> map being objects being named and the naming types.
        /// </summary>
        internal static readonly Dictionary<Type, dynamic> NamingMap = new Dictionary<Type, dynamic>();

        /// <summary>
        /// Gets and sets the current deployment branch.
        /// </summary>
        public static string Branch { get; set; }

        /// <summary>
        /// Gets and sets the current deployment configuration.
        /// </summary>
        public static string Configuration { get; set; }

        /// <summary>
        /// Gets and sets the project's root branch for which we want a specific naming.
        /// </summary>
        public static string RootBranch { get; set; } = "Main";

        /// <summary>
        /// Gets and sets the substring to look for when search for a storage account during container creation.
        /// </summary>
        public static string StorageAccountString { get; set; } = "app";

        /// <summary>
        /// Overrides the <see cref="DefaultNaming{T}"/> for a specific <see cref="DevOpsComponent"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="DevOpsComponent"/> we want to override.</typeparam>
        /// <param name="naming">The <see cref="IName{T}"/> object that will name the overrides.</param>
        public static void UseNaming<T>(IName<T> naming)
            where T : DevOpsComponent
        {
            NamingMap[typeof(T)] = naming;
        }

        /// <summary>
        /// Get the naming object for a specific <see cref="DevOpsComponent"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="DevOpsComponent"/>.</typeparam>
        /// <returns>The naming object.</returns>
        public static IName<T> GetNaming<T>()
            where T : DevOpsComponent
        {
            return NamingMap.ContainsKey(typeof (T)) ?
                (IName<T>) NamingMap[typeof (T)] :
                new DefaultNaming<T>();
        }
    }
}
