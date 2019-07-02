using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using GemBox;
using GemBox.Spreadsheet;

namespace OfficeWrapper
{
    /// <summary>
    /// Обёртка для excell файлов.
    /// </summary>
    public class Spreadsheet
    {
        public Spreadsheet()
        {
            GemBox.Spreadsheet.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        }

        /// <summary>
        /// Процедура создания таблицы в excel из объекта DataTable
        /// </summary>
        /// <param name="dataTable">Объект данных для создания таблицы</param>
        /// <param name="path">Путь к создаваемому файлу.</param>
        public void CreateSpreadsheet(DataTable dataTable, string path)
        {
            var workbook = new ExcelFile();
            string name = dataTable.TableName == "" ? "Неизвестно" : dataTable.TableName;
            var worksheet = workbook.Worksheets.Add(name);
            worksheet.InsertDataTable(dataTable);
            workbook.Save(path);
        }
    }
}
