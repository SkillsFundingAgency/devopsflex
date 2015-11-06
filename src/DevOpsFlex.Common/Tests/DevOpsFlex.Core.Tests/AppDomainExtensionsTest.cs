namespace DevOpsFlex.Core.Tests
{
    using System;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the extension class for the <see cref="AppDomain"/>
    /// in DevOpsFlex.Core.
    /// </summary>
    [TestClass]
    public class AppDomainExtensionsTest
    {
        /// <summary>
        /// Tests that the RedirectAssembly is redirecting properly by generating one assembly and two versions
        /// of a referenced assembly. The reference is for the older version and what gets loaded into the <see cref="AppDomain"/>
        /// is the newer version.
        /// </summary>
        [TestMethod, TestCategory("Unit"), TestCategory("Roslyn")]
        public void Test_RedirectAssembly_ProperlyRedirects()
        {
            var assemblyName = $"{nameof(Test_RedirectAssembly_ProperlyRedirects).Replace("_", "")}";

            using (var oldMs = new MemoryStream())
            using (var newMs = new MemoryStream())
            using (var targetMs = new MemoryStream())
            {
                var referenceAsmOld = CSharpSyntaxTree.ParseText($@"
                    using System.Reflection;
                    [assembly: AssemblyTitle(""{assemblyName}Reference"")]
                    [assembly: AssemblyVersion(""1.0.0.0"")]
                    [assembly: AssemblyFileVersion(""1.0.0.0"")]

                    namespace {nameof(Test_RedirectAssembly_ProperlyRedirects)}Reference
                    {{
                        public class {nameof(Test_RedirectAssembly_ProperlyRedirects)}ReferenceClass
                        {{
                        }}
                    }}");

                var compilationOld = CSharpCompilation.Create(
                    $"{assemblyName}Reference",
                    new[] {referenceAsmOld},
                    new[]
                    {
                        MetadataReference.CreateFromFile(typeof (object).Assembly.Location)
                    },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                var compilationResult = compilationOld.Emit(oldMs);
                Assert.IsTrue(compilationResult.Success, $"OldReference assembly generation failed, inspect {nameof(compilationResult)}.Diagnostics");
                oldMs.Seek(0, SeekOrigin.Begin);

                var referenceAsmNew = CSharpSyntaxTree.ParseText($@"
                    using System.Reflection;
                    [assembly: AssemblyTitle(""{assemblyName}Reference"")]
                    [assembly: AssemblyVersion(""2.0.0.0"")]
                    [assembly: AssemblyFileVersion(""2.0.0.0"")]

                    namespace {nameof(Test_RedirectAssembly_ProperlyRedirects)}Reference
                    {{
                        public class {nameof(Test_RedirectAssembly_ProperlyRedirects)}ReferenceClass
                        {{
                        }}
                    }}");

                var compilationNew = CSharpCompilation.Create(
                    $"{assemblyName}Reference",
                    new[] {referenceAsmNew},
                    new[]
                    {
                        MetadataReference.CreateFromFile(typeof (object).Assembly.Location)
                    },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                compilationResult = compilationNew.Emit(newMs);
                Assert.IsTrue(compilationResult.Success, $"NewReference assembly generation failed, inspect {nameof(compilationResult)}.Diagnostics");
                newMs.Seek(0, SeekOrigin.Begin);

                var syntaxTree = CSharpSyntaxTree.ParseText($@"
                    using System;
                    using {nameof(Test_RedirectAssembly_ProperlyRedirects)}Reference;

                    namespace {nameof(Test_RedirectAssembly_ProperlyRedirects)}
                    {{
                        public class {nameof(Test_RedirectAssembly_ProperlyRedirects)}Class
                        {{
                            private {nameof(Test_RedirectAssembly_ProperlyRedirects)}ReferenceClass foo = new {nameof(Test_RedirectAssembly_ProperlyRedirects)}ReferenceClass();

                            public void DoNothing()
                            {{
                                Console.WriteLine(""Did Nothing!"");
                            }}
                        }}
                    }}");

                var compilationTarget = CSharpCompilation.Create(
                    $"{assemblyName}",
                    new[] {syntaxTree},
                    new[]
                    {
                        MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                        MetadataReference.CreateFromStream(oldMs, MetadataReferenceProperties.Assembly)
                    },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                compilationResult = compilationTarget.Emit(targetMs);
                Assert.IsTrue(compilationResult.Success, $"Target test referenced assembly generation failed, inspect {nameof(compilationResult)}.Diagnostics");
                targetMs.Seek(0, SeekOrigin.Begin);

                AppDomain.CurrentDomain.Load(newMs.ToArray());
                AppDomain.CurrentDomain.RedirectAssembly($"{assemblyName}Reference", new Version(2, 0, 0, 0), null);

                var asm = AppDomain.CurrentDomain.Load(targetMs.ToArray());
                var instance = asm.CreateInstance($"{nameof(Test_RedirectAssembly_ProperlyRedirects)}.{nameof(Test_RedirectAssembly_ProperlyRedirects)}Class");

                Assert.IsNotNull(instance);
            }
        }
    }
}
