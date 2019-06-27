using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsWrapper.Consoles.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();
            TypesAnalyzer typesAnalyzer = new TypesAnalyzer();
            typesAnalyzer.TransitionBeetweenTypes(typeof(Type));
            
            //Console.WriteLine(typesAnalyzer.ShowClassesHierarchy());
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Введите:");
                Console.WriteLine("0 - выйти из программы");
                Console.WriteLine("1 - посмотреть возможные переходы между типами в типе Type");
                Console.WriteLine("2 - посмотреть полную иерархию класса");
                Console.WriteLine("3 - посмотреть через какие методы можно достичь одним типом других");
                Console.WriteLine("4 - посмотреть, из каких типов можно попасть в этот за один проход");
                Console.WriteLine("5 - посмотреть основные статистические выкладки по типам");
                Console.WriteLine("6 - показать иерархическую связь между двумя классами");
                Console.WriteLine("7 - показать все возможные пути от одного типа к другому");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        Console.WriteLine(typesAnalyzer.ShowInfoAboutClassesAccessible());
                        break;
                    case "2":
                        Console.WriteLine(typesAnalyzer.ShowClassesHierarchy());
                        break;
                    case "3":
                        Console.WriteLine(typesAnalyzer.TypeOneLevelAccessible("Type"));
                        break;
                    case "4":
                        Console.WriteLine(typesAnalyzer.OneLevelAccessors("Type"));
                        break;
                    case "5":
                        Console.WriteLine(typesAnalyzer.StatisticSyntaxFactory());
                        break;
                    case "6":
                        Console.WriteLine(typesAnalyzer.ShowHierarchyRelateBetweenTypes("String", "Type"));
                        break;
                    case "7":
                        Console.WriteLine(typesAnalyzer.AccessabilityFromTypeToType("String", "Type"));
                        break;
                    default:
                        break;
                }
            }
            
            //Console.ReadLine();
        }
    }
}
