using BogdanovUtilitisLib.MVVMUtilsWrapper;
using SqlAnalyzer.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SqlAnalyzer.Models
{
    public abstract class SearchColumnsAbstractModel : NotifyPropertyChanged
    {
        private string connectionString;
        private ObservableCollection<Column> columns =
            new ObservableCollection<Column>();
        private bool? isSearchingNow;
        private ICollectionView columnsCountCollection;
        private RepeatingColumn selectedColumn;
        private ICollectionView columnDetails;
        private Column selectedItemColumnDetails;
        private ObservableCollection<object> uniqueValuesInColumn =
            new ObservableCollection<object>();

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
                ColumnsCount();
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

        /// <summary>
        /// Отображаемая на GUI коллекция колонок сервера БД с количеством
        /// раз повторений этого названия на сервере БД.
        /// </summary>
        public ICollectionView ColumnsCountCollection
        {
            get => columnsCountCollection;
            set
            {
                columnsCountCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранная для получения более детальной информации колонка.
        /// </summary>
        public RepeatingColumn SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                DetailingSelectedColumn(value);

                OnPropertyChanged();
            }
        }

        public void DetailingSelectedColumn(RepeatingColumn value)
        {
            var a = from col in Columns
                    where col.COLUMN_NAME == value.Name
                    select col;

            ColumnDetails = CollectionViewSource.GetDefaultView(
                a);
        }

        /// <summary>
        /// Детализированый список по колонке.
        /// </summary>
        public ICollectionView ColumnDetails
        {
            get => columnDetails;
            set
            {
                columnDetails = value;
                OnPropertyChanged();
            }
        }

        public Column SelectedItemColumnDetails
        {
            get => selectedItemColumnDetails;
            set
            {
                selectedItemColumnDetails = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<object> UniqueValuesInColumn 
        { 
            get => uniqueValuesInColumn;
            set
            {
                uniqueValuesInColumn = value;
                OnPropertyChanged();
            }
        }

        public abstract void SearchColumns();

        public abstract void DetailedInfoAboutColumn(Column d);

        public abstract void SearchUniqueValuesInColumn();

        /// <summary>
        /// Подсчёт количества колонок с одинаковым названием.
        /// </summary>
        protected void ColumnsCount()
        {
            var a = from col in Columns
                    group col by col.COLUMN_NAME into g
                    orderby g.Count() descending
                    select new RepeatingColumn(g.Key, g.Count());
            ColumnsCountCollection = CollectionViewSource.GetDefaultView(a);
        }

    }
}
