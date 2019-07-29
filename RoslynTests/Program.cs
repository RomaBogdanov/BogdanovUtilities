using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoslynTests
{
    class Program
    {
        static void Main(string[] args)
        {
            BogdanovUtilitisLib.Roslyn.CodeGenerator codeGenerator = new BogdanovUtilitisLib.Roslyn.CodeGenerator();

            SyntaxFactorySamples samples = new SyntaxFactorySamples();
            SyntaxTreeAnalyzeSamples stas = new SyntaxTreeAnalyzeSamples();
            while (true)
            {
                Console.WriteLine("1 - посмотреть пример создания метода");
                Console.WriteLine("2 - посмотреть пример создания выражения присваивания");
                Console.WriteLine("3 - посмотреть пример создания кода вызова процедуры");
                Console.WriteLine("11 - посмотреть пример создания выражения " +
                "вызова метода");
                Console.WriteLine("12 - посмотреть пример создания выражения " +
                    "присваивания");
                Console.WriteLine("13 - посмотреть пример объединения выражений " +
                    "в один блок");
                Console.WriteLine("14 - посмотреть пример создания метода");
                Console.WriteLine("15 - посмотреть пример создания метода и " +
                    "добавления постфактум элементов в его тело");
                Console.WriteLine("17 - посмотреть пример вставки объединения " +
                    "выражений в метод(конструктор)");
                Console.WriteLine("21 - посмотреть пример нахождения конструктора");
                Console.WriteLine("22 - посмотреть пример вставки объединения " +
                    "выражений в метод(конструктор)");
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
                    case ("11"):
                        samples.CreatingMethodExpression();
                        break;
                    case ("12"):
                        samples.CreatingAssignmentExpression();
                        break;
                    case ("13"):
                        samples.UnionExpressions();
                        break;
                    case ("14"):
                        samples.CreateMethod();
                        break;
                    case ("15"):
                        samples.CreateMethodAndAddBody();
                        break;
                    case ("21"):
                        stas.SearchConstructor();
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
    }
}
