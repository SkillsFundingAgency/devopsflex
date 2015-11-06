namespace DevOpsFlex.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extends the AppDomain with usefull PowerShell related functionality.
    /// </summary>
    public static class AppDomainExtensions
    {
        private static readonly Dictionary<int, bool> RedirectsApplied = new Dictionary<int, bool>();

        /// <summary>
        /// Adds an <see cref="ResolveEventHandler"/> to redirect all attempts to load a specific assembly name to the specified version.
        /// </summary>
        /// <param name="appDomain">The <see cref="AppDomain"/> where we want to apply the redirect.</param>
        /// <param name="shortName">The short assembly name.</param>
        /// <param name="targetVersion">The target version of the assembly that we want to redirect to.</param>
        /// <param name="publicKeyToken">The public key token of the assembly we are redirecting.</param>
        public static void RedirectAssembly(this AppDomain appDomain, string shortName, Version targetVersion, string publicKeyToken)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(shortName));
            Contract.Requires(targetVersion != null);

            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName) return null;

                var requestingName = args.RequestingAssembly?.FullName ?? "(unknown)";
                Debug.WriteLine($"Redirecting assembly load of {args.Name},\tloaded by {requestingName}");

                requestedAssembly.Version = targetVersion;
                if (!string.IsNullOrWhiteSpace(publicKeyToken))
                {
                    requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                }
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                appDomain.AssemblyResolve -= handler;

                var asm = appDomain.GetAssemblies().SingleOrDefault(a => a.GetName().FullName == requestedAssembly.FullName);
                return asm ?? Assembly.Load(requestedAssembly);
            };

            appDomain.AssemblyResolve += handler;
        }

        /// <summary>
        /// Applies DevOps.Flex assembly redirects required for the PowerShell module.
        /// </summary>
        /// <param name="appDomain">The <see cref="AppDomain"/> where we want to apply the redirects to. Usually this is AppDomain.Current.</param>
        public static void ApplyRedirects(this AppDomain appDomain)
        {
            if (RedirectsApplied.ContainsKey(appDomain.Id) && RedirectsApplied[appDomain.Id]) return;

            AppDomain.CurrentDomain.RedirectAssembly("Newtonsoft.Json", new Version(7, 0, 0, 0), "30ad4fe6b2a6aeed");

            RedirectsApplied.Add(appDomain.Id, true);
        }
    }
}
