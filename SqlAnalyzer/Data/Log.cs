namespace SqlAnalyzer.Data
{
    /// <summary>
    /// Запись по исследованию строки.
    /// </summary>
    public class Log
    {
        public string Value { get; set; }
        /// <summary>
        /// Количество записей в колонке.
        /// </summary>
        public long CountRecsInColumn { get; set; }
        /// <summary>
        /// Количество уникальных записей в колонке.
        /// </summary>
        public long CountUniqueRecsInColumn { get; set; }
        public string DB { get; set; }
        public string Table { get; set; }
        public string Col { get; set; }

        public bool IsError { get; set; }

        public string ErrorMsg { get; set; }
    }
}
