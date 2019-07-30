using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace BogdanovUtilitisLib.Roslyn
{
    /// <summary>
    /// Класс для анализа исходного кода
    /// </summary>
    /// <remarks>
    /// Реализовать:
    /// 1. Вычленение в исходном коде прикреплённых пространств имён
    /// 2. Вычленение всех классов
    /// 3. Вычленение внутри класса методов
    /// 3.1. Получение тела метода;
    /// 3.1.1. Получение начала метода;
    /// 3.1.2. Получение окончания метода;
    /// 3.1.3. Получение всех try..catch..;
    /// 3.1.4. Получение всех return;
    /// 3.2. Получение списка входных параметров;
    /// 4. Вычленение внутри класса свойств;
    /// 5. Вычленение внутри класса полей;
    /// v-------6. Вычленение внутри файла конструкторов;
    /// </remarks>
    public class CodeAnalyzer
    {
        SyntaxTree syntaxTree;

        public SyntaxTree SyntaxTree
        {
            get { return syntaxTree; }
            set
            {
                syntaxTree = value;
            }
        }

        /// <summary>
        /// Создаёт анализатор кода файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public CodeAnalyzer(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            SourceText st = SourceText.From(text);
            syntaxTree = CSharpSyntaxTree.ParseText(st);
        }

        public List<SyntaxNode> SearchConstructors()
        {
            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.DescendantNodes().Where(
                p => p.Kind() == SyntaxKind.ConstructorDeclaration);

            return nodes.ToList();
        }

        public List<SyntaxNode> SearchMethods()
        {
            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.DescendantNodes().Where(
                p => p.Kind() == SyntaxKind.MethodDeclaration);

            return nodes.ToList();
        }
    }
}
