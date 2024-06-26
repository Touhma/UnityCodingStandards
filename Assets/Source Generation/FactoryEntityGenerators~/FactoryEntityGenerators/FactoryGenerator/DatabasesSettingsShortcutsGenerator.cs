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
    public class DatabaseSettingsShortcutsGenerator : IIncrementalGenerator {
        public void Initialize(IncrementalGeneratorInitializationContext context) {
            // Create a provider to filter structs annotated with the [EntityFactory] attribute
            IncrementalValuesProvider<(StructDeclarationSyntax structDeclaration, string Name, string Namespace)> provider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (s, _) => s is StructDeclarationSyntax,
                    (ctx, _) => GetStructDeclarationForSourceGen(ctx))
                .Where(t => t.foundAttribute)
                .Select((t, _) => t.data);

            // Combine all results into a single collection
            IncrementalValueProvider<ImmutableArray<(StructDeclarationSyntax structDeclaration, string Name, string Namespace)>> combinedProvider = provider.Collect();

            // Generate the source code.
            context.RegisterSourceOutput(combinedProvider, GenerateCode);
        }

        private static ((StructDeclarationSyntax structDeclaration, string Name, string Namespace) data, bool foundAttribute) GetStructDeclarationForSourceGen(GeneratorSyntaxContext context) {
            StructDeclarationSyntax structDeclarationSyntax = (StructDeclarationSyntax)context.Node;

            // Iterate over attributes in the struct
            foreach (AttributeSyntax attributeSyntax in structDeclarationSyntax.AttributeLists.SelectMany(attributeList => attributeList.Attributes)) {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Commons.Architectures.GameDatabase") continue;

                string name = structDeclarationSyntax.Identifier.Text;
                string namespaceName = (structDeclarationSyntax.Parent as NamespaceDeclarationSyntax)?.Name.ToString() ?? "UnknownNamespace";

                return ((structDeclarationSyntax, name, namespaceName), true);
            }

            return ((structDeclarationSyntax, string.Empty, string.Empty), false);
        }
        
        private static void GenerateCode(SourceProductionContext context, ImmutableArray<(StructDeclarationSyntax structDeclaration, string Name, string Namespace)> structs) {
            if (structs.IsDefaultOrEmpty) return;

            // Filter valid entries
            List<(StructDeclarationSyntax structDeclaration, string Name, string Namespace)> validStructs = structs.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

            if (!validStructs.Any()) return;

            // Go through all filtered class declarations.
            MemoryStream sourceStream = new();
            StreamWriter sourceStreamWriter = new(sourceStream, Encoding.UTF8);
            IndentedTextWriter codeWriter = new(sourceStreamWriter);

            codeWriter.WriteLine("// <auto-generated/>");
            codeWriter.WriteLine("using Unity.Burst;");

            foreach (string namesSpace in validStructs.Select(s => s.Namespace).Distinct()) {
                codeWriter.WriteLine($"using {namesSpace};");
            }

            codeWriter.WriteLine("namespace Commons.Architectures {");
            codeWriter.Indent++;

            codeWriter.WriteLine("public static partial class DBStatics {");
            codeWriter.Indent++;
          

            foreach ((StructDeclarationSyntax structDeclaration, string Name, string Namespace) vStruct in validStructs) {
                codeWriter.WriteLine($"public abstract class {vStruct.Name}StaticFieldKey {{}}");
                codeWriter.WriteLine($"public static readonly SharedStatic<{vStruct.Name}> {vStruct.Name} = SharedStatic<{vStruct.Name}>.GetOrCreate<{vStruct.Name}, {vStruct.Name}StaticFieldKey>();");
            }

            codeWriter.Indent--;
            codeWriter.WriteLine("}");


            codeWriter.WriteLine("public static partial class DB {");
            codeWriter.Indent++;

            foreach ((StructDeclarationSyntax structDeclaration, string Name, string Namespace) vStruct in validStructs) {
                codeWriter.WriteLine($"public static {vStruct.Name} {vStruct.Name} => DBStatics.{vStruct.Name}.Data;");
                codeWriter.WriteLine($"public static ref {vStruct.Name} {vStruct.Name}Ref => ref DBStatics.{vStruct.Name}.Data;");
            }

            codeWriter.Indent--;
            codeWriter.WriteLine("}");
            codeWriter.Indent--;
            codeWriter.WriteLine("}");

            sourceStreamWriter.Flush();

            context.AddSource($"DbShortcuts.g.cs", SourceText.From(sourceStream, Encoding.UTF8, canBeEmbedded: true));
        }
    }
}