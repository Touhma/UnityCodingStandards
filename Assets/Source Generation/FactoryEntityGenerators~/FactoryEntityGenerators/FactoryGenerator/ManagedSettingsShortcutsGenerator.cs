﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ServiceGenerator {
    [Generator]
    public class ManagedSettingsShortcutsGenerator : IIncrementalGenerator {
        public void Initialize(IncrementalGeneratorInitializationContext context) {
            // Create a provider to filter structs annotated with the [EntityFactory] attribute
            IncrementalValuesProvider<(ClassDeclarationSyntax classDeclaration, string Name, string Namespace)> provider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (s, _) => s is ClassDeclarationSyntax,
                    (ctx, _) => GetClassDeclarationForSourceGen(ctx))
                .Where(t => t.foundAttribute)
                .Select((t, _) => t.data);

            // Combine all results into a single collection
            IncrementalValueProvider<ImmutableArray<(ClassDeclarationSyntax classDeclaration, string Name, string Namespace)>> combinedProvider = provider.Collect();

            // Generate the source code.
            context.RegisterSourceOutput(combinedProvider, GenerateCode);
        }

        private static ((ClassDeclarationSyntax classDeclaration, string Name, string Namespace) data, bool foundAttribute) GetClassDeclarationForSourceGen(GeneratorSyntaxContext context) {
            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            // Iterate over attributes in the struct
            foreach (AttributeSyntax attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeList => attributeList.Attributes)) {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Commons.Architectures.ManagedSetting") continue;

                string name = classDeclarationSyntax.Identifier.Text;
                string namespaceName = (classDeclarationSyntax.Parent as NamespaceDeclarationSyntax)?.Name.ToString() ?? "UnknownNamespace";

                return ((classDeclarationSyntax, name, namespaceName), true);
            }

            return ((classDeclarationSyntax, string.Empty, string.Empty), false);
        }

        private static void GenerateCode(SourceProductionContext context, ImmutableArray<(ClassDeclarationSyntax classDeclaration, string Name, string Namespace)> classes) {
            if (classes.IsDefaultOrEmpty) return;

            // Filter valid entries
            List<(ClassDeclarationSyntax classDeclaration, string Name, string Namespace)> validClasses = classes.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

            if (!validClasses.Any()) return;

            // Go through all filtered class declarations.
            MemoryStream sourceStream = new();
            StreamWriter sourceStreamWriter = new(sourceStream, Encoding.UTF8);
            IndentedTextWriter codeWriter = new(sourceStreamWriter);

            codeWriter.WriteLine("// <auto-generated/>");
            codeWriter.WriteLine("using Commons.Services;");
            foreach (string namesSpace in validClasses.Select(s => s.Namespace).Distinct()) {
                codeWriter.WriteLine($"using {namesSpace};");
            }

            codeWriter.WriteLine("namespace Commons.Architectures {");
            codeWriter.Indent++;

            codeWriter.WriteLine("public static class ManagedSettings {");
            codeWriter.Indent++;

            foreach ((ClassDeclarationSyntax classDeclaration, string Name, string Namespace) vClass in validClasses) {
                codeWriter.WriteLine($"public static {vClass.Name} {vClass.Name} =>  ServiceLocator.Current.Get<{vClass.Name}>();");
            }

            codeWriter.Indent--;
            codeWriter.WriteLine("}");
            codeWriter.Indent--;
            codeWriter.WriteLine("}");

            sourceStreamWriter.Flush();

            context.AddSource($"ManagedSettingsShortcuts.g.cs", SourceText.From(sourceStream, Encoding.UTF8, canBeEmbedded: true));
        }
    }
}