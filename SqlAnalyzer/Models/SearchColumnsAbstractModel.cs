using BogdanovUtilitisLib.MVVMUtilsWrapper;
using SqlAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAnalyzer.Models
{
    public abstract class SearchColumnsAbstractModel : NotifyPropertyChanged
    {
        private string connectionString;
        private ObservableCollection<Column> columns = new ObservableCollection<Column>();
        private bool? isSearchingNow;

        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Column> Columns
        {
            get => columns;
            set
            {
                columns = value;
                OnPropertyChanged();
            }
        }

        public bool? IsSearchingNow 
        { 
            get => isSearchingNow;
            set
            {
                isSearchingNow = value;
                OnPropertyChanged();
            }
        }

        public abstract void SeachColumns();
    }
}
