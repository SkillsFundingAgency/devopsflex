namespace DevOpsFlex.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Represents the Analyzer that enforces bad request returns inside a catch clause in an Mvc Controller (found by naming convention).
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MvcReturnBadHttpCodeAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Defines the naming convention for a controller BaseClass implementation.
        /// </summary>
        private const string ControllerName = "Controller"; // convention

        internal const string DiagnosticId = "MvcReturnBadHttpCode";

        private static readonly DiagnosticDescriptor MvcReturnBadHttpCodeRule =
            new DiagnosticDescriptor(
                id: DiagnosticId,
                title: new LocalizableResourceString(nameof(Resources.MvcReturnBadHttpCodeTitle), Resources.ResourceManager, typeof(Resources)),
                messageFormat: new LocalizableResourceString(nameof(Resources.MvcReturnBadHttpCodeMessageFormat), Resources.ResourceManager, typeof(Resources)),
                category: "Mvc",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.MvcReturnBadHttpCodeDescription), Resources.ResourceManager, typeof(Resources)));

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MvcReturnBadHttpCodeRule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">The <see cref="AnalysisContext"/> context used to register actions.</param>
        public sealed override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMvcReturnBadHttpCode, SyntaxKind.CatchClause);
        }

        /// <summary>
        /// Analyzes bad request returns inside a catch clause in an Mvc Controller (found by naming convention).
        /// </summary>
        /// <param name="context">The <see cref="SyntaxNode"/> context given to the Analyzer.</param>
        private void AnalyzeMvcReturnBadHttpCode(SyntaxNodeAnalysisContext context)
        {
            // 1. Skip everything that's not public.
            var methodDec = context.Node.FirstAncestorOrSelf<SyntaxNode>(n => n.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
            if (methodDec == null || methodDec.Modifiers.ToImmutableList().All(m => (string)m.Value != "public"))
            {
                return;
            }

            // 2. Skip if the class declaration doesn't inherit from something that contains the Controller convention in the name.
            var classDec = context.Node.FirstAncestorOrSelf<SyntaxNode>(n => n.Kind() == SyntaxKind.ClassDeclaration) as ClassDeclarationSyntax;
            if (classDec == null || !classDec.BaseList.Types.Any(t => t.Type.ToString().Contains(ControllerName)))
            {
                return;
            }

            // 3. Skip if there are no return statements inside the catch block.
            var returns = ((CatchClauseSyntax)context.Node).Block.Statements.ToImmutableList()
                                                            .Where(s => s.Kind() == SyntaxKind.ReturnStatement).Cast<ReturnStatementSyntax>()
                                                            .ToList();
            if (!returns.Any())
            {
                return;
            }

            // 4. Check if the return statements are invocations.
            var invocations = returns.Select(r => r.Expression)
                                     .Where(e => e.Kind() == SyntaxKind.InvocationExpression)
                                     .Cast<InvocationExpressionSyntax>()
                                     .ToList();
            if (!invocations.Any())
            {
                return;
            }

            // 5. Check if any of the invocations are for the BadRequest identifier.
            var badRequests = invocations.Select(i => i.Expression).Cast<IdentifierNameSyntax>().Where(i => (string)i.Identifier.Value == "BadRequest").ToList();
            if (!badRequests.Any())
            {
                return;
            }

            // 6. Issue errors for each return BadRequest found.
            foreach (var identifier in badRequests)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        MvcReturnBadHttpCodeRule,
                        identifier.FirstAncestorOrSelf<SyntaxNode>(n => n.Kind() == SyntaxKind.ReturnStatement).GetLocation()
                        ));
            }
        }
    }
}
