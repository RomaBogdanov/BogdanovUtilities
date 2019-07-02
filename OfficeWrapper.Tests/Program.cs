using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OfficeWrapper.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            DataTable dataTable = new DataTable("Тестовая");
            dataTable.Columns.Add("Колонка1");
            dataTable.Columns.Add("Колонка2");
            dataTable.Rows.Add(new object[] { "1", "2" });
            spreadsheet.CreateSpreadsheet(dataTable, "Test.xlsx");
        }
    }
}
