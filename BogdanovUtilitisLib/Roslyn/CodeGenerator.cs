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
    /// v------1. Создание метода;
    /// v------1.1. Создание метода с параметрами входа;
    /// 1.2. Создание метода с атрибутами;
    /// 1.3. Сделать return в методе, если возвращается не void;
    /// 2. Создание конструктора;
    /// v------3. Создание выражения (примеры: M = P();, var A = B;, C = D;);
    /// v------4. Вызов метода(пример: Meth1(););
    /// 5. Вставка выражения или вызова метода в любое место конструктора или метода;
    /// 6. Создание try...catch...
    /// 7. Создание полей в классе;
    /// 8. Создание класса;
    /// 9. Создание пространства имён, на которое ссылается файл.
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

        /// <summary>
        /// Процедура присваивания левой части правой
        /// </summary>
        /// <param name="leftPart">Левая часть выражения присваивания</param>
        /// <param name="expression">Выражение присваивания</param>
        /// <param name="rightPart">Правая часть выражения присваивания</param>
        /// <returns></returns>
        public ExpressionStatementSyntax CreatingAssignmentExpression(
            string leftPart, ExpressionTypes expression, string rightPart)
        {
            var expr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(leftPart),
                SyntaxFactory.Token(ExpressionType.Expression(expression)),
                SyntaxFactory.IdentifierName(rightPart))).WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();
            return expr;
        }

        /// <summary>
        /// Процедура вызова процедуры с параметрами
        /// </summary>
        /// <param name="procedure">Наименование процедуры</param>
        /// <param name="parameters">Параметры присваиваемые процедуре</param>
        /// <returns></returns>
        public ExpressionStatementSyntax CreatingCallProcedureExpression(string procedure, List<string> parameters)
        {
            string st = "";
            if (!(parameters == null || parameters.Count == 0))
            {
                st = string.Join(", ", parameters);
            }
            ArgumentListSyntax args = SyntaxFactory.ParseArgumentList($"({st})");
            var expr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName(procedure), args)).WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();
            return expr;
        }

        /// <summary>
        /// Добавление выражения в тело метода
        /// </summary>
        /// <param name="method">Метод</param>
        /// <param name="expression">Добавляемое выражение</param>
        /// <returns></returns>
        public MethodDeclarationSyntax AddExpressionToMethodsBody(MethodDeclarationSyntax method,
            ExpressionStatementSyntax expression)
        {
            method = method.AddBodyStatements(expression).NormalizeWhitespace();
            return method;
        }

        /// <summary>
        /// Добавление выражения в начало тела метода
        /// </summary>
        /// <param name="method"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public MethodDeclarationSyntax AddExpressionToStartMethodsBody(MethodDeclarationSyntax method,
            ExpressionStatementSyntax expression)
        {
            if (method.Body == null)
            {
                return method;
            }
            if (method.Body.ChildNodes().Count() == 0)
            {
                return method.AddBodyStatements(expression).NormalizeWhitespace();
            }
            BlockSyntax newBlock = method.Body.InsertNodesBefore(method.Body
                .ChildNodes().ElementAt(0),
                new List<SyntaxNode> { expression }).NormalizeWhitespace();
            method = method.ReplaceNode(method.Body, newBlock);
            return method;
        }

        /// <summary>
        /// Добавление выражения в конец метода или перед return
        /// </summary>
        /// <param name="method"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public MethodDeclarationSyntax AddExpressionToFinishOrBeforeReturnMethodsBody(MethodDeclarationSyntax method,
            ExpressionStatementSyntax expression)
        {
            if (method.Body == null)
            {
                return method;
            }
            IEnumerable<SyntaxNode> retStatements = method.Body.ChildNodes().Where(p =>
                p.Kind() == SyntaxKind.ReturnStatement);
            BlockSyntax newBlock = method.Body;
            if (retStatements.Count() == 0)
            {
                return AddExpressionToMethodsBody(method, expression);
            }
            else
            {
                foreach (var item in retStatements)
                {
                    newBlock = newBlock.InsertNodesBefore(item, new List<SyntaxNode> { expression });
                    //Console.WriteLine(item.GetText());
                }
            }
            method = method.ReplaceNode(method.Body, newBlock.NormalizeWhitespace());
            return method;
            //throw new Exception("Не реализовано");
        }

        /// <summary>
        /// Добавление выражения внутрь catch конструкции try..catch..
        /// </summary>
        /// <param name="method"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public MethodDeclarationSyntax AddExpressionToCatchConstructionMethodsBody(MethodDeclarationSyntax method,
            ExpressionStatementSyntax expression)
        {
            throw new Exception("Не реализовано");
        }

        /// <summary>
        /// Создаёт ссылку на пространства имён.
        /// </summary>
        /// <returns></returns>
        public UsingDirectiveSyntax CreatingUsingDirective(string name)
        {
            return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(name))
                .NormalizeWhitespace();
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

        private void Test1()
        { }
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
    /// Тип выражения
    /// </summary>
    public enum ExpressionTypes
    {
        Equal,
        PlusEqual,
        MinusEqual
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

    static class ExpressionType
    {
        public static SyntaxKind Expression(ExpressionTypes expressionType)
        {
            SyntaxKind syntaxKind;
            switch (expressionType)
            {
                case ExpressionTypes.Equal:
                    syntaxKind = SyntaxKind.EqualsToken;
                    break;
                case ExpressionTypes.PlusEqual:
                    syntaxKind = SyntaxKind.PlusEqualsToken;
                    break;
                case ExpressionTypes.MinusEqual:
                    syntaxKind = SyntaxKind.MinusEqualsToken;
                    break;
                default:
                    throw new Exception("Данный тип присваивания " +
                        "не соответствует обрабатываемым типам присваивания");
            }
            return syntaxKind;
        }
    }
}
