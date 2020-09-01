using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAnalyzer.Models
{
    public class SearchColumnsTest1Model : SearchColumnsAbstractModel
    {
        public override void SeachColumns()
        {
            Columns = new System.Collections.ObjectModel.ObservableCollection<Data.Column>();
            Columns.Add(new Data.Column
            {
                TABLE_CATALOG = "TestCatalog",
                TABLE_NAME = "TestTable",
                COLUMN_NAME = "TestColumn",
                DATA_TYPE = "char"
            });
        }
    }
}
