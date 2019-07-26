using System;
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
                Console.WriteLine("2 - посмотреть пример создания метода");
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
                        Console.WriteLine(codeGenerator.CreateMethod(methName, 
                            BogdanovUtilitisLib.Roslyn.AccessStatuses.Private,
                            methOutput).GetText());
                        break;
                    case ("2"):
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
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}
