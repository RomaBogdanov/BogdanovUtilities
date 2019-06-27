using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogsWrapper
{
    /// <summary>
    /// Класс анализатор работы с SyntaxFactory.
    /// </summary>
    /// <remarks>
    /// Задачи:
    /// 1. найти возможность взаимодействия между классами
    /// 2. найти наследования классов
    /// 3. 
    /// </remarks>
    public class TypesAnalyzer
    {

        /// <summary>
        /// первым идёт возвращаемый метод, вторым описание исследуемого типа.
        /// </summary>
        Dictionary<string, SearchingType> searchTypes =
                   new Dictionary<string, SearchingType>();

        public TypesAnalyzer()
        {
            //TransitionBeetweenTypes();
        }

        /// <summary>
        /// Ищет все возможные однонаправленные переходы между классами,
        /// которые можно осуществить c помощью типа.
        /// </summary>
        public void TransitionBeetweenTypes(Type searchingType)
        {
            // Находим список всех привязанных к типу классов
            Type type = searchingType;
            foreach (var method in type.GetMethods())
            {
                if (method.IsPublic)
                {
                    var methodReturnName = method.ReturnType.FullName ?? method.ReturnType.Name;
                    if (!searchTypes.Keys.Contains(methodReturnName))
                    {
                        searchTypes.Add(methodReturnName, new SearchingType
                        {
                            Name = method.ReturnType.Name,
                            FullName = methodReturnName,
                            AssemblyQualifier = method.ReturnType.AssemblyQualifiedName
                        });
                    }
                    ParameterInfo[] a = method.GetParameters();
                    foreach (var item2 in a)
                    {
                        string c = item2.ParameterType.FullName ?? item2.ParameterType.Name;
                        if (!searchTypes.Keys.Contains(c))
                        {
                            searchTypes.Add(c, new SearchingType
                            {
                                Name = item2.ParameterType.Name,
                                FullName = c,
                                AssemblyQualifier = item2.ParameterType.AssemblyQualifiedName
                            });
                        }
                        searchTypes[c].AvailableTypes
                            .Add(new RelateSearchingTypeString
                            {
                                SearchingType = searchTypes[methodReturnName],
                                RelatedString = method.Name
                            });
                        searchTypes[methodReturnName].Prototypes
                            .Add(new RelateSearchingTypeString
                            {
                                SearchingType = searchTypes[c],
                                RelatedString = method.Name
                            });
                        //(searchTypes[c], item.Name));
                    }
                }
            }
            // Находим иерархическое дерево между типами
            var p = searchTypes.Values.ToList();
            foreach (var st in p)
            {
                AddParentTypeToDictionary(st);
            }
        }

        /// <summary>
        /// Выводим информацию о доступности классов в словаре типов
        /// </summary>
        /// <remarks>
        /// Процедура показывает, из какого типа можно попасть в какой.
        /// </remarks>
        public string ShowInfoAboutClassesAccessible()
        {
            string output = "";
            foreach (var item in searchTypes)
            {
                output += item.Value.Name + Environment.NewLine;
                IEnumerable<SearchingType> types = item.Value.AvailableTypes
                    .Select(p => p.SearchingType).Distinct();
                foreach (var type in types)
                {
                    output += $"----------->{type.Name}" + Environment.NewLine;
                }
            }
            output += $"Количество всех типов: {searchTypes.Count}" + Environment.NewLine;
            return output;
        }

        /// <summary>
        /// Показывает полную иерархию классов
        /// </summary>
        /// <param name="classes"></param>
        public string ShowClassesHierarchy(params string[] classes)
        {
            string output = "";
            var root = searchTypes.Values.Where(p => p.Parent == null);
            foreach (var item in root)
            {
                if (classes.Contains(item.Name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    output += $"{item.Name}<---" + Environment.NewLine;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    output += item.Name + Environment.NewLine;
                }
                ShowChilds(item, classes: classes);
            }
            output += $"Количество всех типов: {root.Count()}" + Environment.NewLine;
            return output;
        }

        /// <summary>
        /// Просматриваем достижимость типом других типов за один переход и показывает,
        /// через какие методы это возможно.
        /// </summary>
        /// <param name="type"></param>
        public string TypeOneLevelAccessible(string type, bool IsFullName = false)
        {
            string output = "";
            SearchingType t = null;
            if (IsFullName)
            {
                t = searchTypes.Select(p => p.Value)
                    .FirstOrDefault(p => p.FullName == type);
            }
            else
            {
                t = searchTypes.Select(p => p.Value)
                    .FirstOrDefault(p => p.Name == type);
            }
            output += type + Environment.NewLine;
            foreach (var item in t.AvailableTypes.Distinct())
            {
                output += $"->[{item.RelatedString}]->{item.SearchingType.Name}"
                    + Environment.NewLine;
            }
            return output;
        }

        /// <summary>
        /// Просматриваем типы достигающие данный тип за один переход
        /// </summary>
        /// <param name="type"></param>
        public string OneLevelAccessors(string type, bool IsFullName = false)
        {
            string output = "";
            SearchingType t = null;
            if (IsFullName)
            {
                t = searchTypes.Select(p => p.Value)
                    .FirstOrDefault(p => p.FullName == type);
            }
            else
            {
                t = searchTypes.Select(p => p.Value)
                    .FirstOrDefault(p => p.Name == type);
            }
            foreach (var item in t.Prototypes.Distinct())
            {
                output += $"{item.SearchingType.Name}->[{item.RelatedString}]->"
                    + Environment.NewLine;
            }
            output += $"------------->{type}" + Environment.NewLine;
            return output;
        }

        /// <summary>
        /// Основные статистические выкладки по типам связанным с типом
        /// </summary>
        public string StatisticSyntaxFactory(bool IsFullName = false)
        {
            string output = "";
            var a = from i in searchTypes.Values
                    select new
                    {
                        tp = i,
                        input = i.Prototypes.Distinct().Count(),
                        output = i.AvailableTypes.Distinct().Count()
                    };
            output += "Входит\t Выходит\t Наименование  " + Environment.NewLine;
            foreach (var item in a.OrderBy(p => p.output))
            {
                if (IsFullName)
                {
                    output += $"{item.input}\t{item.output}\t{item.tp.FullName}" 
                        + Environment.NewLine;
                }
                else
                {
                    output += $"{item.input}\t{item.output}\t{item.tp.Name}" 
                        + Environment.NewLine;
                }
            }
            return output;
        }

        /// <summary>
        /// Показывает иерархическую связь между двумя классами
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        public string ShowHierarchyRelateBetweenTypes(string type1, string type2)
        {
            string output = "";
            List<SearchingType> searcheds1 = new List<SearchingType>();
            List<SearchingType> searcheds2 = new List<SearchingType>();

            string t1 = searchTypes.Select(p => p.Value)
                .FirstOrDefault(p => p.Name == type1).FullName;
            string t2 = searchTypes.Select(p => p.Value)
                .FirstOrDefault(p => p.Name == type2).FullName;
            LoadingList(searcheds1, searchTypes[t1]);
            LoadingList(searcheds2, searchTypes[t2]);
            foreach (var item in searcheds1.ToArray())
            {
                if (searcheds2.Contains(item))
                {
                    int i1 = searcheds1.IndexOf(item);
                    int i2 = searcheds2.IndexOf(item);
                    string s = "";
                    for (int i = i1; i >= 0; i--)
                    {
                        output += $"{s}{searcheds1[i].Name}" + Environment.NewLine;
                        s += "|<---";
                    }
                    output += "" + Environment.NewLine;
                    s = "|<---";
                    for (int i = i2 - 1; i >= 0; i--)
                    {
                        output += $"{s}{searcheds2[i].Name}" + Environment.NewLine;
                        s += "|<---";
                    }
                    break;
                }
            }
            return output;
        }

        /// <summary>
        /// Процедура поиска путей от одного типа к другому
        /// </summary>
        /// <param name="prototype">тип, из которого необходимо найти путь</param>
        /// <param name="image">тип, к которому необходимо найти путь</param>
        /// <param name="firstEntry">true - находим самый короткий путь,
        /// false - находим все доступные пути</param>
        public string AccessabilityFromTypeToType(string prototype, string image,
            bool firstEntry = false)
        {
            return AccessabilityFromTypeToType(prototype, image, firstEntry, null, "");
        }

        private string AccessabilityFromTypeToType(string prototype, string image,
            bool firstEntry, Dictionary<SearchingType, string> posiblePaths,
            string predicat)
        {
            string output = "";
            List<string> listOfAccessability = new List<string>();
            // Находим иерархическую цепочку класса, который пытается достичь другого
            List<SearchingType> ParentClasses = new List<SearchingType>();
            if (posiblePaths == null)
            {
                posiblePaths = new Dictionary<SearchingType, string>();
            }
            string t1 = searchTypes.Select(p => p.Value)
                .FirstOrDefault(p => p.Name == prototype).FullName;
            string t2 = searchTypes.Select(p => p.Value)
                .FirstOrDefault(p => p.Name == image).FullName;
            LoadingList(ParentClasses, searchTypes[t1]);

            foreach (var item in ParentClasses)
            {
                if (!posiblePaths.ContainsKey(item))
                {
                    posiblePaths.Add(item, predicat);
                }
            }
            // Перебираем привязанные процедуры на предмет достижимости, 
            // извлекаем все достижимые типы, если достигли данного типа - 
            // записываем в специальное хранилище
            if (firstEntry)
            {
                foreach (var item in ParentClasses)
                {
                    string path = predicat;
                    if (prototype == item.Name)
                    {
                        path += $"{prototype}";
                    }
                    else
                    {
                        path += $"{prototype}-^{item.Name}";
                    }
                    foreach (var item2 in item.AvailableTypes)
                    {
                        if (item2.SearchingType.Name == image)
                        {
                            listOfAccessability.Add($"{path}-->[{item2.RelatedString}]" +
                                $"-->{item2.SearchingType.Name}");

                            goto Output;
                        }
                    }
                }
            }

            foreach (var item in ParentClasses)
            {
                string path = predicat;
                if (prototype == item.Name)
                {
                    path += $"{prototype}";
                }
                else
                {
                    path += $"{prototype}-^{item.Name}";
                }
                if (!firstEntry)
                {
                    foreach (var item2 in item.AvailableTypes)
                    {
                        if (item2.SearchingType.Name == image)
                        {
                            listOfAccessability.Add($"{path}-->[{item2.RelatedString}]" +
                                $"-->{item2.SearchingType.Name}");

                        }
                    }
                }
                foreach (var item2 in item.AvailableTypes)
                {
                    if (!posiblePaths.ContainsKey(item2.SearchingType))
                    {
                        AccessabilityFromTypeToType(item2.SearchingType.Name, image,
                            firstEntry, posiblePaths,
                            $"{path}-->[{item2.RelatedString}]-->");//{item2.Item1.Name}");
                    }
                }
            }
        // Сравниваем новые достижимые типы
        Output:
            foreach (var item in listOfAccessability)
            {
                output += item + Environment.NewLine;
            }
            return output;
        }

        /// <summary>
        /// Загружает в список последовательность Родительских классов
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        private void LoadingList(List<SearchingType> list, SearchingType type)
        {
            list.Add(type);
            if (type.Parent != null)
            {
                LoadingList(list, type.Parent);
            }
        }

        /// <summary>
        /// Добавляем тип в словарь
        /// </summary>
        /// <param name="type"></param>
        private void AddParentTypeToDictionary(SearchingType type)
        {
            SearchingType parentType;
            Type pt = null;
            Type t = Type.GetType($"{type.AssemblyQualifier}");
            try
            {
                pt = t.BaseType;
            }
            catch (Exception err)
            {
                return;
            }
            if (pt == null)
            {
                return;
            }
            else
            {
                if (searchTypes.Keys.Contains(pt.FullName))
                {
                    type.Parent = searchTypes[pt.FullName];
                    searchTypes[pt.FullName].Childs.Add(type);
                }
                else
                {
                    searchTypes.Add(pt.FullName, new SearchingType
                    {
                        Name = pt.Name,
                        FullName = pt.FullName,
                        AssemblyQualifier = pt.AssemblyQualifiedName
                    });
                    type.Parent = searchTypes[pt.FullName];
                    searchTypes[pt.FullName].Childs.Add(type);
                    AddParentTypeToDictionary(searchTypes[pt.FullName]);
                }
            }
        }

        private string ShowChilds(SearchingType type, string offset = "|<----", params string[] classes)
        {
            string output = "";
            foreach (var item in type.Childs)
            {
                if (classes.Contains(item.Name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    output += $"{offset} {item.Name}<---" + Environment.NewLine;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    output += $"{offset} {item.Name}" + Environment.NewLine;
                }
                output += ShowChilds(item, "|     " + offset, classes);
            }
            return output;
        }

    }

    /// <summary>
    /// Описание изучаемого типа
    /// </summary>
    /// <remarks>
    /// Описание класса, интерфейса или ещё чего, требующего исследования.
    /// Содержит список доступных из типа типов;
    /// список типов, из которых можно добраться напряму к данному;
    /// родительские типы;
    /// наследуемые классы.
    /// </remarks>
    public class SearchingType
    {
        /// <summary>
        /// Наименование типа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное наименование типа с включением пространства имён.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Помогает понять, где находится сборка
        /// </summary>
        public string AssemblyQualifier { get; set; }

        /// <summary>
        /// Список доступных типов, т.е. типов, в которые можно попасть из 
        /// данного типа. Первым идёт тип, а вторым идёт метод, который помогает
        /// проброситься.
        /// </summary>
        public List<RelateSearchingTypeString> AvailableTypes { get; set; }
            = new List<RelateSearchingTypeString>();

        /// <summary>
        /// Список типов, из которых можно добраться напрямую.
        /// </summary>
        public List<RelateSearchingTypeString> Prototypes { get; set; }
            = new List<RelateSearchingTypeString>();

        /// <summary>
        /// Родительский тип данного типа
        /// </summary>
        public SearchingType Parent { get; set; }

        /// <summary>
        /// Наследуемые классы.
        /// </summary>
        public List<SearchingType> Childs { get; set; }
            = new List<SearchingType>();
    }

    public class RelateSearchingTypeString
    {
        public SearchingType SearchingType { get; set; }

        public string RelatedString { get; set; }
    }
}
