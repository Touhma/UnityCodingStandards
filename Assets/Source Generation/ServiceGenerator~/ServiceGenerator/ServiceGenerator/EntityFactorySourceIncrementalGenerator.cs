using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ServiceGenerator;

/// <summary>
/// A sample source generator that creates a custom report based on class properties. The target class should be annotated with the 'Generators.ReportAttribute' attribute.
/// When using the source code as a baseline, an incremental source generator is preferable because it reduces the performance overhead.
/// </summary>
[Generator]
public class EntityFactorySourceIncrementalGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // Filter structs annotated with the [EntityFactory] attribute. Only filtered Syntax Nodes can trigger code generation.
        IncrementalValuesProvider<((StructDeclarationSyntax, List<string> componentTypes), bool reportAttributeFound)> provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is StructDeclarationSyntax,
                (ctx, _) => GetStructDeclarationForSourceGen(ctx))
            .Where(t => t.reportAttributeFound)
            .Select((t, _) => t);

        // Generate the source code.
        context.RegisterSourceOutput(provider, GenerateCode);
    }

    private static ((StructDeclarationSyntax, List<string> componentTypes), bool reportAttributeFound) GetStructDeclarationForSourceGen(GeneratorSyntaxContext context) {
        StructDeclarationSyntax structDeclarationSyntax = (StructDeclarationSyntax)context.Node;
        List<string> componentTypes = new();

        foreach (AttributeSyntax attributeSyntax in structDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes)) {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

            string attributeName = attributeSymbol.ContainingType.ToDisplayString();

            if (attributeName != "Commons.Architectures.EntityFactory") continue;
            // Check for GenWith attribute and extract component types
            foreach (AttributeSyntax attr in structDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)) {
                if (context.SemanticModel.GetSymbolInfo(attr).Symbol is not IMethodSymbol attrSymbol) continue;

                string attrName = attrSymbol.ContainingType.ToDisplayString();

                if (attrName != "Commons.Architectures.GenWithAttribute") continue;

                SeparatedSyntaxList<AttributeArgumentSyntax> args = attr.ArgumentList!.Arguments;
                componentTypes.AddRange(args.Select(arg => arg.Expression).OfType<TypeOfExpressionSyntax>().Select(typeOfExpr => typeOfExpr.Type).Select(type => type.ToString()));
            }

            return ((structDeclarationSyntax, componentTypes), true);
        }

        return ((structDeclarationSyntax, componentTypes), false);
    }

    public static void GenerateCode(SourceProductionContext context, ((StructDeclarationSyntax structDeclaration, List<string> componentTypes) data, bool _) input) {
        StructDeclarationSyntax structDeclarationSyntax = input.data.structDeclaration;
        List<string> componentTypes = input.data.componentTypes;

        string factoryName = structDeclarationSyntax.Identifier.Text;
        NamespaceDeclarationSyntax namespaceDeclaration = structDeclarationSyntax.Parent as NamespaceDeclarationSyntax;
        string namespaceName = namespaceDeclaration?.Name.ToString() ?? "UnknownNamespace";

        StringBuilder sourceBuilder = new($$"""
                                            using System;
                                            using Players.Components;
                                            using Unity.Collections;
                                            using Unity.Entities;
                                            using Players.Factories;
                                            using Unity.Burst;
                                            
                                            namespace {{namespaceName}} {
                                                public partial struct {{factoryName}} : IDisposable {
                                                    public EntityArchetype Archetype;
                                                    public EntityQuery Query;
                                            
                                                    public void Setup(ref SystemState state) {
                                                        NativeList<ComponentType> componentTypes = new(Allocator.Temp) {

                                            """);

        foreach (string component in componentTypes) {
            sourceBuilder.AppendLine($"                ComponentType.ReadWrite<{component}>(),");
        }

        sourceBuilder.AppendLine($$"""
                                               };
                                   
                                               Archetype = state.EntityManager.CreateArchetype(componentTypes.AsArray()); // Caching the Archetype
                                               Query = new EntityQueryBuilder(Allocator.Temp).WithAll(ref componentTypes).Build(ref state); // Caching the Query matching the archetype
                                   
                                               componentTypes.Dispose();
                                           }
                                   
                                           public Entity CreateEntity(ref SystemState state) => state.EntityManager.CreateEntity(Archetype); // Use this method to instantiate something with the same archetype
                                   
                                           public void Dispose() {
                                               Query.Dispose();
                                           }
                                       }
                                   }
                                   """);

        context.AddSource($"{factoryName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
}

static class CodeWriterExtensions {
    public static void AppendFullTypeName(this TextWriter codeWriter, ClassDeclarationSyntax classDeclarationSyntax) {
        int ancestorCount = 0;
        SyntaxNode parent = classDeclarationSyntax.Parent;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax) {
            ancestorCount++;
            parent = parent.Parent;
        }

        parent = classDeclarationSyntax.Parent;

        string[] names = new string[ancestorCount];
        int currentAncestor = ancestorCount - 1;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax) {
            switch (parent) {
                case BaseTypeDeclarationSyntax parentClass:
                    names[currentAncestor] = parentClass.Identifier.Text;
                    break;
                case BaseNamespaceDeclarationSyntax parentNamespace:
                    names[currentAncestor] = parentNamespace.Name.ToString();
                    break;
            }

            currentAncestor--;
            parent = parent.Parent;
        }

        codeWriter.Write("global::");
        foreach (string name in names) {
            codeWriter.Write(name);
            codeWriter.Write('.');
        }

        codeWriter.Write(classDeclarationSyntax.Identifier.Text);
    }

    public static void AppendFullTypeName(this TextWriter codeWriter, StructDeclarationSyntax structDeclarationSyntax) {
        int ancestorCount = 0;
        SyntaxNode parent = structDeclarationSyntax.Parent;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax) {
            ancestorCount++;
            parent = parent.Parent;
        }

        parent = structDeclarationSyntax.Parent;

        string[] names = new string[ancestorCount];
        int currentAncestor = ancestorCount - 1;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax) {
            switch (parent) {
                case BaseTypeDeclarationSyntax parentClass:
                    names[currentAncestor] = parentClass.Identifier.Text;
                    break;
                case BaseNamespaceDeclarationSyntax parentNamespace:
                    names[currentAncestor] = parentNamespace.Name.ToString();
                    break;
            }

            currentAncestor--;
            parent = parent.Parent;
        }

        codeWriter.Write("global::");
        foreach (string name in names) {
            codeWriter.Write(name);
            codeWriter.Write('.');
        }

        codeWriter.Write(structDeclarationSyntax.Identifier.Text);
    }
}