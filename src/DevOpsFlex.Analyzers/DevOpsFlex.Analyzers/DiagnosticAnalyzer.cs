namespace DevOpsFlex.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Represents the Analyzer that enforces package consolidation (unique reference per package) and a unique packages folder
    /// for each assembly being compiled.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PackageConsolidationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// This exists as a private static for performance reasons. We might get into the space where the HashSet might become too big,
        /// but we'll re-strategize if we get there.
        /// </summary>
        private static readonly HashSet<Package> Packages = new HashSet<Package>();

        private static readonly DiagnosticDescriptor SinglePackagesFolderRule =
            new DiagnosticDescriptor(
                id: "PackageConsolidationSinglePackagesFolder",
                title: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderTitle), Resources.ResourceManager, typeof(Resources)),
                messageFormat: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderMessageFormat), Resources.ResourceManager, typeof(Resources)),
                category: "Packages",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderDescription), Resources.ResourceManager, typeof(Resources)));

        private static readonly DiagnosticDescriptor UniqueVersionRule =
            new DiagnosticDescriptor(
                id: "PackageConsolidationUniqueVersion",
                title: new LocalizableResourceString(nameof(Resources.UniqueVersionTitle), Resources.ResourceManager, typeof(Resources)),
                messageFormat: new LocalizableResourceString(nameof(Resources.UniqueVersionMessageFormat), Resources.ResourceManager, typeof(Resources)),
                category: "Packages",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.UniqueVersionDescription), Resources.ResourceManager, typeof(Resources)));

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(SinglePackagesFolderRule, UniqueVersionRule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">The <see cref="AnalysisContext"/> context used to register actions.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(AnalyzePackageConsolidation);
        }

        /// <summary>
        /// Analyzes that package consolidation (unique reference per package) and a unique packages folder
        /// are in place for each assembly being compiled. Because this is being run per assembly you might
        /// see a repetition of the same error.
        /// </summary>
        /// <param name="context">The <see cref="CompilationAnalysisContext"/> context that parents all analysis elements.</param>
        private static void AnalyzePackageConsolidation(CompilationAnalysisContext context)
        {
            var packageReferences = context.Compilation
                                           .References
                                           .Cast<PortableExecutableReference>()
                                           .Where(r => r.FilePath.ToLower().Contains(Package.PackagesFolderName))
                                           .ToList();

            var firstReferencePath = packageReferences.First().FilePath;
            var packagesFolder = firstReferencePath.Substring(0, firstReferencePath.IndexOf(Package.PackagesFolderName, StringComparison.Ordinal) + Package.PackagesFolderName.Length);

            // 1. Make sure there's only a packages folder
            if (packageReferences.Any(r => !r.FilePath.Contains(packagesFolder)))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SinglePackagesFolderRule,
                        context.Compilation.Assembly.Locations[0],
                        context.Compilation.AssemblyName // {0} MessageFormat
                        ));
            }

            // 2. Make sure for each reference in the packages folder, we're only dealing with a unique version
            var newPackages = Directory.EnumerateDirectories(packagesFolder).Select(d => new Package(d)).Except(Packages);
            foreach (var package in newPackages)
            {
                Packages.Add(package);
            }

            var packagesNotConsolidated = packageReferences.Select(r => new Package(r.FilePath))
                                                           .Where(r => Packages.Count(p => p.Name == r.Name) > 1);

            foreach (var referencePackage in packagesNotConsolidated)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        UniqueVersionRule,
                        context.Compilation.Assembly.Locations[0],
                        context.Compilation.AssemblyName, // {0} MessageFormat
                        referencePackage.Name // {1} MessageFormat
                        ));
            }
        }
    }
}
