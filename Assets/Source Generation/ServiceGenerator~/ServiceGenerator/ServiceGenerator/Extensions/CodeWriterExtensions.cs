using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ServiceGenerator.Extensions;


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