namespace DevOpsFlex.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Represents the Analyzer that enforces package consolidation (unique reference per package) and a unique packages folder
    /// for each assembly being compiled.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PackageConsolidationAnalyzer : DiagnosticAnalyzer
    {
        private const string PackagesFolderName = "\\packages\\"; // convention
        private static readonly string PackageVersionRegex = PackagesFolderName.Replace("\\", "\\\\") + "[^0-9]*([0-9]+(?:\\.[0-9]+)+)\\\\";
        private static readonly string PackageNameRegex = PackagesFolderName.Replace("\\", "\\\\") + "([a-zA-Z]+(?:\\.[a-zA-Z]+)*)[^\\\\]*\\\\";

        private static readonly DiagnosticDescriptor SinglePackagesFolderRule =
            new DiagnosticDescriptor(
                id: "DevOpsFlex.PackageConsolidationAnalyzer.SinglePackagesFolder",
                title: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderTitle), Resources.ResourceManager, typeof(Resources)),
                messageFormat: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderMessageFormat), Resources.ResourceManager, typeof(Resources)),
                category: "Packages",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.SinglePackagesFolderDescription), Resources.ResourceManager, typeof(Resources)));

        private static readonly DiagnosticDescriptor UniqueVersionRule =
            new DiagnosticDescriptor(
                id: "DevOpsFlex.PackageConsolidationAnalyzer.UniqueVersion",
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
                                           .Select(r => r.FilePath)
                                           .Where(p => p.ToLower().Contains(PackagesFolderName))
                                           .ToList();

            var packagesFolder = packageReferences.First().Substring(0, packageReferences.First().IndexOf(PackagesFolderName, StringComparison.Ordinal) + PackagesFolderName.Length);

            // 1. Make sure there's only a packages folder
            if (packageReferences.Any(p => !p.Contains(packagesFolder)))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SinglePackagesFolderRule,
                        context.Compilation.Assembly.Locations[0],
                        context.Compilation.AssemblyName // {0} MessageFormat
                        ));
            }

            var allPackages = Directory.EnumerateDirectories(packagesFolder).ToList();

            // 2. Make sure for each reference in the packages folder, we're only dealing with a unique version
            foreach (var reference in packageReferences)
            {
                var packageName = Regex.Match(reference, PackageNameRegex, RegexOptions.Singleline)
                                       .Groups[1].Value;

                if (allPackages.Count(p => p.Contains(packageName)) > 1)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            UniqueVersionRule,
                            context.Compilation.Assembly.Locations[0],
                            context.Compilation.AssemblyName, // {0} MessageFormat
                            packageName // {1} MessageFormat
                            ));
                }
            }
        }
    }
}
