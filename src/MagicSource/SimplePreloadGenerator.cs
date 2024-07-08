using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicSource
{

    //public class FromConfig
    //{
    //    private string _key;

    //    public FromConfig(string key)
    //    {
    //        _key = key;
    //    }
    //}

    //[Generator]
    public class SimplePreloadGenerator// : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}

            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;
            List<string> preloadKeys = new List<string>();
            foreach (var syntaxTree in syntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var descendantNodes = root.DescendantNodes();
                var declarations = descendantNodes.OfType<FieldDeclarationSyntax>();



                foreach (var declaration in declarations)
                {
                    var fieldType = declaration.Declaration.Type.ToString();

                    if (fieldType == "FromConfig")
                    {

                        if (declaration is FieldDeclarationSyntax fieldDeclaration)
                        {
                            preloadKeys = fieldDeclaration.Declaration.Variables
                                .Select(variable => variable.Initializer?.Value)
                                .OfType<LiteralExpressionSyntax>()
                                .Select(x => x.Token.ValueText)
                                .Distinct()
                                .ToList();
                        }


                    }
                }


            }

            context.AddSource("PreloadKeys.g.cs",
                SourceText.From(@"
namespace MagicSource
{
// generated
    public class PreloadKeys
    {
        static PreloadKeys()
        {
            Abstractions.TenantUtil.Instance.SetPreloadKeys(Keys);
        }

        public static List<string> Keys = new List<string>()
        {
            """ + string.Join("\",\n                    \"", preloadKeys) + @"""
        };
    }
}
",
                Encoding.UTF8));

            //var test = GetFromApi();
        }

        private string GetFromApi()
        {
            try
            {


                HttpClient client = new HttpClient();

                var task = client.GetStringAsync("https://localhost:5001/api/keys");
                task.Wait();



                return task.Result;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }


        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}