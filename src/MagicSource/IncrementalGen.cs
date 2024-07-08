using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicSource
{
    [Generator]
    public class IncrementalGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var fieldDecs = context.SyntaxProvider.CreateSyntaxProvider(
                (s, _) => IsSyntaxTargetForGeneration(s),
                (ctx, _) => GetSemanticTargetForGeneration(ctx));

            IncrementalValueProvider<(Compilation, ImmutableArray<LiteralExpressionSyntax>)> compilationAndClasses
                = context.CompilationProvider.Combine(fieldDecs.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (ctx, source) => Execute(source.Item1, source.Item2, ctx));
        }

        static void Execute(
            Compilation compilation,
            ImmutableArray<LiteralExpressionSyntax> expressions,
            SourceProductionContext context)
        {
            if (expressions.IsDefaultOrEmpty)
            {
                return;
            }

            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }

            var dist = expressions.Select(x => x.Token.ValueText).Distinct();
            context.AddSource("PreloadKeys.g.cs",
                SourceText.From(@"
namespace ConfigDemo
{
    public partial class PreloadKeys
    {
        static PreloadKeys()
        {
            Abstractions.TenantUtil.Instance.SetPreloadKeys(Keys);
        }


        public static List<string> Keys = new List<string>()
        {
            """ + string.Join("\",\n                    \"", dist) + @"""
        };
    }
}
",
                Encoding.UTF8));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is FieldDeclarationSyntax m && m.Declaration.Type.ToString() == "FromConfig";

        static LiteralExpressionSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            FieldDeclarationSyntax field = (FieldDeclarationSyntax)context.Node;
            return field.Declaration.Variables.Select(x => x.Initializer?.Value)
                .OfType<LiteralExpressionSyntax>()
                .FirstOrDefault();
        }
    }
}
