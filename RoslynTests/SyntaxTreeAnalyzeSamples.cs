using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynTests
{
    /// <summary>
    /// Примеры анализа кода
    /// </summary>
    class SyntaxTreeAnalyzeSamples
    {
        SyntaxTree tree;

        public SyntaxTreeAnalyzeSamples()
        {
            string path = @"C:\BogdanovR\sintez\SintezOSPClient-2019-01-11-14-29-45\Sintez\SintezOSPClient\Main.cs";
            string txt = System.IO.File.ReadAllText(path);
            SourceText st = SourceText.From(txt);
            tree = CSharpSyntaxTree.ParseText(st);
        }

        /// <summary>
        /// Анализ конструктора
        /// </summary>
        public void SearchConstructor()
        {
            SyntaxNode root = tree.GetRoot();
            var nodes = root.DescendantNodes().Where(p => p.Kind() == SyntaxKind.ConstructorDeclaration);
            foreach (var item in nodes)
            {
                Console.WriteLine(item.GetText());
            }
        }

        /// <summary>
        /// Добавляем в конструктор выражения
        /// </summary>
        public void AddingExpressionsToConstructor()
        {
            SyntaxNode root = tree.GetRoot();
            var nodes = root.DescendantNodes().Where(p => p.Kind() == SyntaxKind.ConstructorDeclaration);
            foreach (var item in nodes)
            {
                Console.WriteLine(item.GetText());
                var st = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("TRRR")));
                var a = item as ConstructorDeclarationSyntax;
                var b = a.AddBodyStatements(st).NormalizeWhitespace();
                var expr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("L"),
                SyntaxFactory.Token(SyntaxKind.PlusEqualsToken),
                SyntaxFactory.IdentifierName("T")))
                    .WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                b = b.AddBodyStatements(expr).NormalizeWhitespace();
                Console.WriteLine(b.GetText());

                root = root.ReplaceNode(a, b).NormalizeWhitespace();

                Console.WriteLine(root.GetText());
            }
        }

    }
}
