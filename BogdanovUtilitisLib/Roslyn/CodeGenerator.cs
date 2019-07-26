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
            string outputType = "void")//SyntaxKind accessStatus = SyntaxKind.PublicKeyword)
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
            //Console.WriteLine(expr.GetText());
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
