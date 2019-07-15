using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Proxier.Exceptions;

namespace Proxier.Managers
{
    /// <summary>
    ///     Creates assemblies from code string.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class CodeManager
    {
        /// <summary>
        ///     Generates an assembly from code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="references">Assembly references</param>
        /// <returns></returns>
        public static Task<Assembly> GenerateAssembly(string code, IEnumerable<MetadataReference> references = null)
        {
            return Task.Run(() =>
            {
                using (var mem = new MemoryStream())
                {
                    var compilation = CreateCompilation(code, references);
                    var compilationResult = compilation.Emit(mem);

                    if (compilationResult.Success)
                    {
                        mem.Seek(0, SeekOrigin.Begin);
                        return Assembly.Load(mem.ToArray());
                    }

                    foreach (var codeIssue in compilationResult.Diagnostics)
                        throw new CompilationError(codeIssue,
                            $"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()}, Location: {codeIssue.Location.GetLineSpan()}, Severity: {codeIssue.Severity}");

                    return null;
                }
            });
        }

        private static CSharpCompilation CreateCompilation(string code,
            IEnumerable<MetadataReference> references = null)
        {
            var compilation = CSharpCompilation.Create(Nanoid.Nanoid.Generate())
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references ?? AppDomain.CurrentDomain.GetAssemblies()
                                   .Where(i => !i.IsDynamic && !string.IsNullOrEmpty(i.Location))
                                   .Distinct().Select(i => MetadataReference.CreateFromFile(i.Location))
                                   .OrderBy(i => i.FilePath)
                                   .ToList())
                .AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(code));
            return compilation;
        }
    }
}