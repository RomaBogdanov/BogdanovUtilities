using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynTests
{
    /// <summary>
    /// Класс примеров генерации кода с помощью SyntaxFactory
    /// </summary>
    class SyntaxFactorySamples
    {

        /// <summary>
        /// Создание выражения, в котором вызывается метод
        /// </summary>
        public ExpressionStatementSyntax CreatingMethodExpression()
        {
            var expr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("D")))
            .WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            expr = expr.NormalizeWhitespace();
            Console.WriteLine(expr.GetText());
            return expr;
        }

        /// <summary>
        /// Создание выражения присваивания
        /// </summary>
        public ExpressionStatementSyntax CreatingAssignmentExpression()
        {
            var expr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("L"),
                SyntaxFactory.Token(SyntaxKind.PlusEqualsToken),
                SyntaxFactory.IdentifierName("T")))
            .WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            expr = expr.NormalizeWhitespace();
            Console.WriteLine(expr);
            return expr;
        }

        /// <summary>
        /// Создание объединения выражений
        /// </summary>
        public void UnionExpressions()
        {
            List<StatementSyntax> l = new List<StatementSyntax>
            {
                CreatingMethodExpression(),
                CreatingAssignmentExpression()
            };


            var expr = SyntaxFactory.Block(SyntaxFactory.List<StatementSyntax>(l));

            expr = expr.NormalizeWhitespace();
            Console.WriteLine(expr.GetText());
        }

        /// <summary>
        /// Создание метода
        /// </summary>
        public void CreateMethod()
        {
            var expr = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                "LLL")
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    SyntaxFactory.Block());
            expr = expr.NormalizeWhitespace();
            Console.WriteLine(expr.GetText());
        }

        /// <summary>
        /// Добавление метода и постфактум добавление выражений в тело метода
        /// </summary>
        public void CreateMethodAndAddBody()
        {
            var expr = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                "LLL")
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    SyntaxFactory.Block());
            expr = expr.NormalizeWhitespace();
            Console.WriteLine(expr.GetText());

            expr = expr.AddBodyStatements(CreatingMethodExpression(),
                CreatingAssignmentExpression()).NormalizeWhitespace();
            Console.WriteLine(expr);

            var st = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("TRRR")));
            expr = expr.AddBodyStatements(st).NormalizeWhitespace();
            Console.WriteLine(expr);
        }

        public void CreateConstructor()
        { }

        public void Test1()
        {
        }

        public void Test2()
        {
            List<string> methods = new List<string>();
            Type type = typeof(SyntaxFactory);
            string pattern = "ExpressionStatement";
            foreach (var item in type.GetMethods())
            {
                if (item.IsPublic)
                {
                    string s = $"{item.ReturnType.Name} {item.Name}";
                    //Console.WriteLine();
                    //methods.Add($"{item.ReturnType.Name} {item.Name}");
                    //methods.Add($"{item.ToString()}");
                    var a = item.GetParameters();
                    foreach (var item2 in a)
                    {
                        s += $"\t{item2.ParameterType.Name} ";
                        //Console.WriteLine($"\t{item2.ParameterType.Name} {item2.Name}");
                    }
                    methods.Add(s);
                    //Console.WriteLine(s);
                }
            }
            foreach (var item in methods)
            {
                if (Regex.IsMatch(item, pattern))
                {
                    Console.WriteLine(item);
                }
            }
        }

    }
}
