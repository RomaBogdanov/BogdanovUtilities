using System.Collections.Generic;

namespace SqlAnalyzer.Data
{
    /// <summary>
    /// Содержит информацию о количестве повторений названия колонки на сервере БД.
    /// </summary>
    public class RepeatingColumn
    {

        public string Name { get; }
        public int Count { get; }

        public RepeatingColumn(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override bool Equals(object obj)
        {
            return obj is RepeatingColumn other &&
                   Name == other.Name &&
                   Count == other.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = 34153098;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }
    }
}
