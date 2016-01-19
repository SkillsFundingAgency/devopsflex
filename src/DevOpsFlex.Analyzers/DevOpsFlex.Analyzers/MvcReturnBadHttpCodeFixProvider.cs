namespace DevOpsFlex.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// Provides a CodeFix for the <see cref="MvcReturnBadHttpCodeAnalyzer"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MvcReturnBadHttpCodeFixProvider)), Shared]
    public class MvcReturnBadHttpCodeFixProvider : CodeFixProvider
    {
        internal const string CapturedExceptionIdentifier = "requestEx";

        /// <summary>
        /// A list of diagnostic IDs that this provider can provider fixes for.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MvcReturnBadHttpCodeAnalyzer.DiagnosticId);

        /// <summary>
        /// Computes one or more fixes for the specified <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext"/>.
        /// </summary>
        /// <param name="context">
        /// A <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext"/> containing context information about the diagnostics to fix.
        /// The context must only contain diagnostics with an <see cref="P:Microsoft.CodeAnalysis.Diagnostic.Id"/> included in the <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider.FixableDiagnosticIds"/> for the current provider.
        /// </param>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Throw instead of returning BadRequest",
                        createChangedDocument: token => ThrowInsteadOfReturnBadRequest(context.Document, node, token),
                        equivalenceKey: nameof(ThrowInsteadOfReturnBadRequest)),
                    diagnostic);
            }
        }

        /// <summary>
        /// Gets an optional <see cref="T:Microsoft.CodeAnalysis.CodeFixes.FixAllProvider"/> that can fix all/multiple occurrences of diagnostics fixed by this code fix provider.
        /// Return null if the provider doesn't support fix all/multiple occurrences.
        /// Otherwise, you can return any of the well known fix all providers from <see cref="T:Microsoft.CodeAnalysis.CodeFixes.WellKnownFixAllProviders"/> or implement your own fix all provider.
        /// </summary>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Refactors the return BadRequest into a proper throw and does some minor refactorings alongside that the throw requires.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> that we are applying the fix in.</param>
        /// <param name="node">The <see cref="ReturnStatementSyntax"/> node that fired the error.</param>
        /// <param name="token">The async <see cref="CancellationToken"/>.</param>
        /// <returns>The newly refactored <see cref="Document"/>.</returns>
        private async Task<Document> ThrowInsteadOfReturnBadRequest(Document document, SyntaxNode node, CancellationToken token)
        {
            var editor = await DocumentEditor.CreateAsync(document, token);
            var catchNode = (CatchClauseSyntax) node.FirstAncestorOrSelf<SyntaxNode>(n => n.Kind() == SyntaxKind.CatchClause);
            var exceptionIdentifier = catchNode.Declaration?.Identifier.ValueText ?? CapturedExceptionIdentifier;

            // 1. Capture the first argument of the BadRequest call if it's a valid one, otherwise populate with a "PUT SOMETHING MEANINGFUL HERE" string literal
            var badRequestArg = (((ReturnStatementSyntax) node).Expression as InvocationExpressionSyntax)?.ArgumentList.Arguments.FirstOrDefault() ??
                                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(
                                        SyntaxFactory.TriviaList(),
                                        @"""PUT SOMETHING MEANINGFUL HERE""",
                                        "PUT SOMETHING MEANINGFUL HERE",
                                        SyntaxFactory.TriviaList())));

            // 2. Convert the return BadRequest to a throw.
            var argsList = new SeparatedSyntaxList<ArgumentSyntax>();
            argsList = argsList.Add(
                SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(
                        SyntaxFactory.TriviaList(),
                        @"400",
                        400,
                        SyntaxFactory.TriviaList()))));
            argsList = argsList.Add(badRequestArg);
            argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(exceptionIdentifier)));

            var throwStatement = SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName("System.Web.HttpException"), SyntaxFactory.ArgumentList(argsList), null));
            var newCatchNode = catchNode.ReplaceNode(node, throwStatement);

            // 3. Check if the exception is being captured already. If it's not, start capturing it.
            var declaration = catchNode.Declaration;

            if (declaration?.Identifier.Value == null)
            {
                if (declaration == null)
                {
                    // 3a. This handles catch all clauses [catch { }]
                    var capturedExceptionDeclaration = SyntaxFactory.CatchDeclaration(
                        SyntaxFactory.ParseTypeName("System.Exception"),
                        SyntaxFactory.Identifier(SyntaxFactory.ParseLeadingTrivia(" "), CapturedExceptionIdentifier, SyntaxFactory.TriviaList()));

                    newCatchNode = newCatchNode.WithDeclaration(capturedExceptionDeclaration)
                                               // if we don't rebuild the catch keyword we get the previous catch clause trailing trivia between the catch keyword and declaration (usually a '/n')
                                               .WithCatchKeyword(SyntaxFactory.Token(newCatchNode.CatchKeyword.LeadingTrivia, SyntaxKind.CatchKeyword, SyntaxFactory.TriviaList()));
                }
                else
                {
                    // 3b. This handles catch without capture clauses [catch (Exception) { }]
                    var capturedExceptionDeclaration = SyntaxFactory.CatchDeclaration(
                        declaration.Type,
                        SyntaxFactory.Identifier(declaration.Type.GetLeadingTrivia(), CapturedExceptionIdentifier, declaration.Type.GetTrailingTrivia()));

                    newCatchNode = newCatchNode.WithDeclaration(capturedExceptionDeclaration);
                }
            }

            editor.ReplaceNode(catchNode, newCatchNode);

            // 4. Return the changed document
            return editor.GetChangedDocument();
        }
    }
}
