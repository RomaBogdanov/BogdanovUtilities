using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.SqlAnalyzerLib.Model
{
    /// <summary>
    /// Содержит описание базы данных на сервере БД.
    /// </summary>
    public class DataBase
    {
        [Key]
        public string Name { get; set; }
    }
}
