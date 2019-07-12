using System.Collections.Generic;

namespace LogsWrapper
{
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
}
