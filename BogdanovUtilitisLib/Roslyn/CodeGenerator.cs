using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Пространство имён для работы с исходным кодом через Roslyn-компилятор.
/// </summary>
/// <remarks>
/// Основные задачи работы с исходным кодом:
/// 1. Анализ существующего кода;
/// 2. Создание нового кода.
/// </remarks>
namespace BogdanovUtilitisLib.Roslyn
{
    /// <summary>
    /// Класс для генерации кода.
    /// </summary>
    /// <remarks>
    /// Реализовать:
    /// 1. Создание метода;
    /// 1.1. Создание метода с параметрами входа;
    /// 1.2. Создание метода с атрибутами;
    /// 2. Создание конструктора;
    /// 3. Создание выражения (примеры: M = P();, var A = B;, C = D;);
    /// 4. Вызов метода(пример: Meth1(););
    /// 5. Вставка выражения или вызова метода в любое место конструктора или метода;
    /// 6. Создание try...catch...
    /// 7. Создание полей в классе;
    /// 8. Создание класса;
    /// </remarks>
    public class CodeGenerator
    {

        /// <summary>
        /// Создание метода
        /// </summary>
        /// <param name="methodName">Название будущего метода</param>
        /// <param name="accessStatus">Видимость будущего метода</param>
        /// <param name="outputType">Формат вывода будущего метода, 
        /// если пустой или null, то void</param>
        public MethodDeclarationSyntax CreateMethod(string methodName,
            AccessStatuses accessStatus = AccessStatuses.Public,
            string outputType = "void")
        {
            if (string.IsNullOrEmpty(outputType))
            {
                outputType = "void";
            }
            MethodDeclarationSyntax expr = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseName(outputType),
                methodName)
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(AccessStatus.AccessType(accessStatus))))
                .WithBody(
                    SyntaxFactory.Block());

            return expr.NormalizeWhitespace();
        }

        /// <summary>
        /// Процедура добавления параметра в метод
        /// </summary>
        /// <param name="method">Метод, в который надо добавить параметр</param>
        /// <param name="paramName">Наименование параметра</param>
        /// <param name="paramType">Тип параметра</param>
        /// <returns></returns>
        public MethodDeclarationSyntax AddParameterToMethod(
            MethodDeclarationSyntax method, string paramName, string paramType)
        {
            method = method.AddParameterListParameters(SyntaxFactory.Parameter(
                SyntaxFactory.Identifier(paramName))
                .WithType(SyntaxFactory.ParseTypeName(paramType))).NormalizeWhitespace();
            return method;
        }

        public MethodDeclarationSyntax CreateMethod2(string methodName,
    AccessStatuses accessStatus = AccessStatuses.Public,
    string outputType = "void")
        {
            if (string.IsNullOrEmpty(outputType))
            {
                outputType = "void";
            }
            MethodDeclarationSyntax expr = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseName(outputType),
                methodName)
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(AccessStatus.AccessType(accessStatus))))
                .WithBody(
                    SyntaxFactory.Block());
            //SyntaxFactory.AttributeList()
            /*SeparatedSyntaxList<AttributeSyntax> spr = new SeparatedSyntaxList<AttributeSyntax>();
            spr.Add(SyntaxFactory.Attribute(SyntaxFactory.ParseName("string b")));
            var b = SyntaxFactory.AttributeList(spr);
            expr = expr.AddAttributeLists(b);*/

            expr = expr.AddParameterListParameters(new ParameterSyntax[]
            {
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("a")).WithType(SyntaxFactory.ParseTypeName("string")),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("b")).WithType(SyntaxFactory.ParseTypeName("string"))
            });
            //SyntaxFactory.Parameter(SyntaxFactory.ParseToken("string a"));

            //ParameterSyntax parameterSyntax
            //AttributeListSyntax atr =
            /*expr.AddAttributeLists(new AttributeListSyntax[]
            {

            });*/
            //SyntaxFactory.Parameter(SyntaxFactory.Token())
            //SyntaxFactory.Token(default(syntaxl))
            return expr.NormalizeWhitespace();
        }
    }

    /// <summary>
    /// Список статусов доступа
    /// </summary>
    public enum AccessStatuses
    {
        Public,
        Private,
        Protected
    }

    /// <summary>
    /// Статус доступа
    /// </summary>
    static class AccessStatus
    {

        public static SyntaxKind AccessType(AccessStatuses accessStatus)
        {
            SyntaxKind syntaxKind;
            switch (accessStatus)
            {
                case AccessStatuses.Public:
                    syntaxKind = SyntaxKind.PublicKeyword;
                    break;
                case AccessStatuses.Private:
                    syntaxKind = SyntaxKind.PrivateKeyword;
                    break;
                case AccessStatuses.Protected:
                    syntaxKind = SyntaxKind.ProtectedKeyword;
                    break;
                default:
                    throw new Exception("Данный статус не соответсвует статусам доступа");
            }
            return syntaxKind;
        }
    }
}
