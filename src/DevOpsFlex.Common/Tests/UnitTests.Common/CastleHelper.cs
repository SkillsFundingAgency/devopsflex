namespace UnitTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Handlers;
    using Castle.Windsor;
    using Castle.Windsor.Diagnostics;

    /// <summary>
    /// Constains methods and extensions that help test Castle Components.
    /// </summary>
    public static class CastleHelper
    {
        /// <summary>
        /// Checks if there's any misconfigured components in Castle. This helps
        /// detect any type of configuration problems before run-time.
        /// </summary>
        /// <param name="kernel">The Castle kernel that we want to check.</param>
        /// <returns>A List of misconfigured element information as string.</returns>
        public static IEnumerable<string> CheckForPotentiallyMisconfiguredComponents(this IKernel kernel)
        {
            var host = (IDiagnosticsHost)kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();

            var handlers = diagnostics.Inspect();

            if (!handlers.Any())
            {
                yield break;
            }

            foreach (var handlerRaw in handlers)
            {
                var handler = (IExposeDependencyInfo) handlerRaw;
                var message = new StringBuilder();
                var inspector = new DependencyInspector(message);
                handler.ObtainDependencyDetails(inspector);

                yield return message.ToString();
            }
        }

        /// <summary>
        /// Checks if there's any misconfigured components in Castle. This helps
        /// detect any type of configuration problems before run-time.
        /// </summary>
        /// <param name="container">The Castle container that we want to check.</param>
        /// <returns>A List of misconfigured element information as string.</returns>
        public static IEnumerable<string> CheckForPotentiallyMisconfiguredComponents(this IWindsorContainer container)
        {
            return container.Kernel.CheckForPotentiallyMisconfiguredComponents();
        }

        /// <summary>
        /// Checks if there are components registred that don't satisfy the design by contract
        /// SOLID reference. Note that this implementation doesn't allow for exceptions.
        /// </summary>
        /// <param name="kernel">The Castle kernel that we want to check.</param>
        /// <returns>A List of <see cref="ComponentModel"/> objects that don't follow the Design by Contract principle and are not Castle components.</returns>
        public static IEnumerable<ComponentModel> CheckForBadDesignByContract(this IKernel kernel)
        {
            return kernel.GraphNodes
                         .OfType<ComponentModel>()
                         .Where(c => c.Services
                                      .Any(s => !s.IsInterface &&
                                                s.Namespace != null &&
                                                !s.Namespace.StartsWith("Castle.")));
        }

        /// <summary>
        /// Checks if there are components registred that don't satisfy the design by contract
        /// SOLID reference. Note that this implementation doesn't allow for exceptions.
        /// </summary>
        /// <param name="container">The Castle container that we want to check.</param>
        /// <returns>A List of full <see cref="Type"/> names that don't follow the Design by Contract principle and are not Castle components.</returns>
        public static IEnumerable<string> CheckForBadDesignByContract(this IWindsorContainer container)
        {
            return container.Kernel
                            .CheckForBadDesignByContract()
                            .Select(c => c.Implementation.FullName);
        }
    }
}
