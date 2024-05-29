﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
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
            IncrementalValuesProvider<((ClassDeclarationSyntax, List<string> Names, List<string> NamesSpaces), bool reportInterfaceImplemented)> provider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (s, _) => s is ClassDeclarationSyntax,
                    (ctx, _) => GetClassDeclarationForSourceGen(ctx))
                .Where(t => t.reportInterfaceImplemented)
                .Select((t, _) => t);

            // Generate the source code.
            context.RegisterSourceOutput(provider, GenerateCode);
        }

        private static ((ClassDeclarationSyntax, List<string> Names, List<string> NamesSpaces), bool reportInterfaceImplemented) GetClassDeclarationForSourceGen(GeneratorSyntaxContext context) {
            // Cast the node to StructDeclarationSyntax
            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            List<string> names = new();
            List<string> namesSpaces = new();
            
            foreach (AttributeSyntax attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeList => attributeList.Attributes)) {
                // Get the symbol information for the attribute
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

                // Check if the attribute is [UnmanagedSetting]
                if (attributeSymbol.ContainingType.ToDisplayString() != "Commons.Architectures.ManagedSetting") continue;
                // Add the factory name
                names.Add(classDeclarationSyntax.Identifier.Text);

                NamespaceDeclarationSyntax namespaceDeclaration = classDeclarationSyntax.Parent as NamespaceDeclarationSyntax;
                namesSpaces.Add(namespaceDeclaration?.Name.ToString() ?? "UnknownNamespace");

                return ((classDeclarationSyntax, names, namesSpaces), true);
            }

            // Return the struct and indicate the attribute was not found
            return ((classDeclarationSyntax, names, namesSpaces), false);
        }

        public static void GenerateCode(SourceProductionContext context, ((ClassDeclarationSyntax classDeclaration, List<string> Names, List<string> NamesSpaces) data, bool _) input) {
            List<string> names = input.data.Names;
            List<string> namesSpaces = input.data.NamesSpaces;
            
            // Go through all filtered class declarations.
            MemoryStream sourceStream = new();
            StreamWriter sourceStreamWriter = new(sourceStream, Encoding.UTF8);
            IndentedTextWriter codeWriter = new(sourceStreamWriter);
            
            codeWriter.WriteLine("// <auto-generated/>");
            codeWriter.WriteLine("using Commons.Services;");
            
            foreach (string namesSpace in namesSpaces) {
                codeWriter.WriteLine($"using {namesSpace};");
            }

            codeWriter.WriteLine("namespace Commons.Architectures {");
            codeWriter.Indent++;

            codeWriter.WriteLine("public static class ManagedSettings {");
            codeWriter.Indent++;

            foreach (string service in names) {
                codeWriter.WriteLine($"public static {service} {service} =>  ServiceLocator.Current.Get<{service}>();");
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