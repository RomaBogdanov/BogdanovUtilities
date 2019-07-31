using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace RoslynTests
{
    class Program
    {
        static void Main(string[] args)
        {
            BogdanovUtilitisLib.Roslyn.CodeGenerator codeGenerator =
                new BogdanovUtilitisLib.Roslyn.CodeGenerator();
            BogdanovUtilitisLib.Roslyn.CodeAnalyzer codeAnalyzer =
                new BogdanovUtilitisLib.Roslyn.CodeAnalyzer(
                    @"C:\BogdanovR\MyReps\BogdanovUtilities\RoslynTests\TestFile.cs");
            //@"C:\BogdanovR\MyReps\BogdanovUtilities\BogdanovUtilitisLib\Roslyn\CodeAnalyzer.cs");
            //@"C:\BogdanovR\MyReps\BogdanovUtilities\BogdanovUtilitisLib\Roslyn\CodeGenerator.cs");

            SyntaxFactorySamples samples = new SyntaxFactorySamples();
            SyntaxTreeAnalyzeSamples stas = new SyntaxTreeAnalyzeSamples();
            while (true)
            {
                Console.WriteLine("1 - посмотреть пример создания метода");
                Console.WriteLine("2 - посмотреть пример создания выражения присваивания");
                Console.WriteLine("3 - посмотреть пример создания кода вызова процедуры");
                Console.WriteLine("4 - посмотреть пример создания кода внутри метода");
                Console.WriteLine("5 - посмотреть пример создания ссылки на пространство имён");

                Console.WriteLine("11 - анализ кода на наличие конструкторов");
                Console.WriteLine("12 - анализ кода на наличие методов");
                Console.WriteLine("13 - добавляем в конец метода код");
                Console.WriteLine("14 - добавляем в начало метода код");
                Console.WriteLine("15 - добавляем код перед return, если он есть, если нет, в конец метода");
                Console.WriteLine("16 - анализ кода на наличие пространств имён");
                Console.WriteLine("21 - сделать тестовую генерацию файла с методами обложенными логами");
                /*Console.WriteLine("21 - посмотреть пример нахождения конструктора");
                Console.WriteLine("22 - посмотреть пример вставки объединения " +
                    "выражений в метод(конструктор)");*/
                string input = Console.ReadLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                switch (input)
                {
                    case ("1"):
                        Console.Write("Введите название метода: ");
                        string methName = Console.ReadLine();
                        Console.Write("Введите тип вывода в методе(если " +
                            "пустой, то void): ");
                        string methOutput = Console.ReadLine();
                        var meth = codeGenerator.CreateMethod(methName,
                            BogdanovUtilitisLib.Roslyn.AccessStatuses.Private,
                            methOutput);
                        bool IsFinishParams = false;
                        while (!IsFinishParams)
                        {
                            Console.Write("Введите наимерование праметра:");
                            string parName = Console.ReadLine();
                            Console.Write("Введите тип праметра:");
                            string parType = Console.ReadLine();
                            if (string.IsNullOrEmpty(parName) || string.IsNullOrEmpty(parType))
                            {
                                IsFinishParams = true;
                            }
                            else
                            {
                                meth = codeGenerator.AddParameterToMethod(meth, parName, parType);
                            }
                        }
                        Console.WriteLine(meth.GetText());
                        break;
                    case ("2"):
                        Console.Write("Введите левую часть присваивания:");
                        string left = Console.ReadLine();
                        Console.Write("Введите выражение присваивания(=, +=, -+, по-умолчанию =):");
                        string express = Console.ReadLine();
                        BogdanovUtilitisLib.Roslyn.ExpressionTypes expressionType;
                        if (express == "=" || string.IsNullOrEmpty(express))
                        {
                            expressionType = BogdanovUtilitisLib.Roslyn.ExpressionTypes.Equal;
                        }
                        else if (express == "+=")
                        {
                            expressionType = BogdanovUtilitisLib.Roslyn.ExpressionTypes.PlusEqual;
                        }
                        else if (express == "-=")
                        {
                            expressionType = BogdanovUtilitisLib.Roslyn.ExpressionTypes.MinusEqual;
                        }
                        else
                        {
                            Console.WriteLine("Неправильно введено выражение присваивания");
                            goto Finish;
                        }
                        Console.Write("Введите правую часть присваивания:");
                        string right = Console.ReadLine();
                        Console.WriteLine(codeGenerator.CreatingAssignmentExpression(left, expressionType, right));
                        break;
                    case ("3"):
                        Console.Write("Введите наименование метода:");
                        string mth = Console.ReadLine();
                        List<string> pars = new List<string>();
                        bool isFinishParse = false;
                        while (!isFinishParse)
                        {
                            Console.Write("Введите параметр: ");
                            string s = Console.ReadLine();
                            if (string.IsNullOrEmpty(s))
                            {
                                isFinishParse = true;
                            }
                            else
                            {
                                pars.Add(s);
                            }
                        }
                        Console.WriteLine(codeGenerator.CreatingCallProcedureExpression(mth, pars).GetText());
                        break;
                    case ("4"):
                        var m = codeGenerator.CreateMethod("Meth");
                        m = codeGenerator.AddParameterToMethod(m, "a", "string");
                        m = codeGenerator.AddExpressionToMethodsBody(m,
                            codeGenerator.CreatingAssignmentExpression("A",
                            BogdanovUtilitisLib.Roslyn.ExpressionTypes.PlusEqual, "B"));
                        m = codeGenerator.AddExpressionToMethodsBody(m,
                            codeGenerator.CreatingCallProcedureExpression("A",
                            new List<string> { "a1", "a2" }));
                        Console.WriteLine(m.GetText());
                        break;
                    case ("5"):
                        Console.Write("Введите пространство имён:");
                        var m1 = codeGenerator.CreatingUsingDirective($"{Console.ReadLine()}");
                        Console.WriteLine(m1.GetText());
                        break;
                    case ("11"):
                        List<Microsoft.CodeAnalysis.SyntaxNode> constructs = codeAnalyzer.SearchConstructors();
                        foreach (var item in constructs)
                        {
                            Console.WriteLine(item.GetText());
                            Console.WriteLine();
                        }
                        break;
                    case ("12"):
                        List<Microsoft.CodeAnalysis.SyntaxNode> methods = codeAnalyzer.SearchMethods();
                        foreach (var item in methods)
                        {
                            Console.WriteLine(item.GetText());
                            Console.WriteLine();
                        }
                        break;
                    case ("13"):
                        List<SyntaxNode> methods2 = codeAnalyzer.SearchMethods();
                        SyntaxNode root = codeAnalyzer.SyntaxTree.GetRoot();
                        root = root.ReplaceNodes(methods2, (x, y) =>
                        {
                            y = codeGenerator.AddExpressionToMethodsBody(x as MethodDeclarationSyntax,
                                codeGenerator.CreatingCallProcedureExpression("Logger.Debug", null));
                            return x.ReplaceNode(x, y);
                        }).NormalizeWhitespace();
                        Console.WriteLine(root.GetText());
                        break;
                    case ("14"):
                        List<SyntaxNode> methods3 = codeAnalyzer.SearchMethods();
                        SyntaxNode root2 = codeAnalyzer.SyntaxTree.GetRoot();
                        IEnumerable<SyntaxNode> nodes = root2.DescendantNodes().Where(
                            p => p.Kind() == SyntaxKind.MethodDeclaration);
                        foreach (var item in nodes)
                        {
                            var t = codeGenerator.AddExpressionToStartMethodsBody(item as MethodDeclarationSyntax,
                                codeGenerator.CreatingCallProcedureExpression("Logger.Debug", null));
                            Console.WriteLine(t.GetText());
                        }
                        break;
                    case ("15"):
                        IEnumerable<SyntaxNode> nodes2 = codeAnalyzer.SyntaxTree.GetRoot()
                            .DescendantNodes().Where(
                            p => p.Kind() == SyntaxKind.MethodDeclaration);
                        foreach (var item in nodes2)
                        {
                            var t = codeGenerator.AddExpressionToFinishOrBeforeReturnMethodsBody(item as MethodDeclarationSyntax,
                                codeGenerator.CreatingCallProcedureExpression("Logger.Debug", null));
                            Console.WriteLine(t.GetText());
                        }
                        break;
                    case ("16"):
                        var a = codeAnalyzer.SearchLinkedNamespaces();
                        foreach (var item in a)
                        {
                            Console.WriteLine(item.GetText());
                        }
                        break;
                    case ("21"):
                        GenerateFileWithLogs(codeAnalyzer, codeGenerator);
                        break;
                    case ("22"):
                        stas.AddingExpressionsToConstructor();
                        break;
                    default:
                        break;
                }
            Finish:
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        static void GenerateFileWithLogs(BogdanovUtilitisLib.Roslyn.CodeAnalyzer analyzer,
            BogdanovUtilitisLib.Roslyn.CodeGenerator generator)
        {
            var root = analyzer.SyntaxTree.GetRoot();
            // вставляем в методы логи
            var methods = analyzer.SearchMethods();
            var expression1 = generator.CreatingCallProcedureExpression(
                "Logger.Debug", new List<string> { "\"Начало метода\"" });
            var expression2 = generator.CreatingCallProcedureExpression(
                "Logger.Debug", new List<string> { "\"Окончание метода\"" });

            Func<SyntaxNode, SyntaxNode, SyntaxNode> func = (x, y) =>
             {
                 y = generator.AddExpressionToStartMethodsBody(
                    x as MethodDeclarationSyntax, expression1);
                 y = generator.AddExpressionToFinishOrBeforeReturnMethodsBody(
                     y as MethodDeclarationSyntax, expression2);
                 return y;
             };

            root = root.ReplaceNodes(methods, func).NormalizeWhitespace();
            // вставляем пространство имён
            var usdir = generator.CreatingUsingDirective($"Logging");
            var usdirs = analyzer.SearchLinkedNamespaces(root);

            root = root.InsertNodesAfter(usdirs[usdirs.Count - 1],
                new List<SyntaxNode> { usdir }).NormalizeWhitespace();

            Console.WriteLine(root.GetText());
            System.IO.File.WriteAllText("Test.cs", root.GetText().ToString(), Encoding.UTF8);
        }
    }
}
