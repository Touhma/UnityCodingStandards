﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ServiceGenerator;

[Generator]
public class EnumsTagsComponentsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // Create a provider to filter enums annotated with the [GenerateComponentTags] attribute
        IncrementalValuesProvider<(EnumDeclarationSyntax enumDeclaration, string Name, string Namespace)> provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is EnumDeclarationSyntax,
                (ctx, _) => GetEnumDeclarationForSourceGen(ctx))
            .Where(t => t.foundAttribute)
            .Select((t, _) => t.data);

        // Combine all results into a single collection
        IncrementalValueProvider<ImmutableArray<(EnumDeclarationSyntax enumDeclaration, string Name, string Namespace)>> combinedProvider = provider.Collect();

        // Generate the source code.
        context.RegisterSourceOutput(combinedProvider, GenerateCode);
    }

    private static ((EnumDeclarationSyntax enumDeclaration, string Name, string Namespace) data, bool foundAttribute) GetEnumDeclarationForSourceGen(GeneratorSyntaxContext context) {
        EnumDeclarationSyntax enumDeclarationSyntax = (EnumDeclarationSyntax)context.Node;

        // Iterate over attributes in the enum
        foreach (AttributeSyntax attributeSyntax in enumDeclarationSyntax.AttributeLists.SelectMany(attributeList => attributeList.Attributes)) {
            if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

            if (attributeSymbol.ContainingType.ToDisplayString() != "Commons.Architectures.GenerateComponentTags") continue;

            string name = enumDeclarationSyntax.Identifier.Text;
            string namespaceName = (enumDeclarationSyntax.Parent as NamespaceDeclarationSyntax)?.Name.ToString() ?? "UnknownNamespace";

            return ((enumDeclarationSyntax, name, namespaceName), true);
        }

        return ((enumDeclarationSyntax, string.Empty, string.Empty), false);
    }


    private static void GenerateCode(SourceProductionContext context, ImmutableArray<(EnumDeclarationSyntax enumDeclaration, string Name, string Namespace)> enums) {
        if (enums.IsDefaultOrEmpty) return;

        // Filter valid entries
        List<(EnumDeclarationSyntax enumDeclaration, string Name, string Namespace)> validEnums = enums.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

        if (!validEnums.Any()) return;

        using MemoryStream sourceStream = new();
        using StreamWriter sourceStreamWriter = new(sourceStream, Encoding.UTF8);
        using IndentedTextWriter codeWriter = new(sourceStreamWriter);

        codeWriter.WriteLine("// <auto-generated/>");
        codeWriter.WriteLine("using Unity.Entities;");

        foreach ((EnumDeclarationSyntax enumDeclaration, string Name, string Namespace) enumData in validEnums) {
            
            codeWriter.WriteLine($"// Auto-generated code for enum {enumData.Name}");
            codeWriter.WriteLine($"namespace {enumData.Namespace}");
            codeWriter.WriteLine("{");
            codeWriter.Indent++;

            foreach (EnumMemberDeclarationSyntax member in enumData.enumDeclaration.Members) {
                codeWriter.WriteLine($" public struct {enumData.Name}{member.Identifier.Text}Tag : IComponentData {{}}");
            }
            
            codeWriter.Indent--;
            codeWriter.WriteLine("}");
        }

        sourceStreamWriter.Flush();
        

        context.AddSource($"EnumsTagsComponents.g.cs", SourceText.From(sourceStream, Encoding.UTF8, canBeEmbedded: true));
    }
}