using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using BogdanovUtilitisLib.MVVMUtilsWrapper;
using SqlAnalyzer.Data;
using SqlAnalyzer.Models;

namespace SqlAnalyzer.ViewModels
{
    public class SearchColumnsViewModel : NotifyPropertyChanged
    {
        private SearchColumnsAbstractModel model;
        private ICollectionView columnsCountCollection;
        private object selectedColumn;
        private ICollectionView columnDetails;
        private ObservableCollection<Log> samples;
        private object selectedItemColumnDetails;

        public SearchColumnsAbstractModel Model
        {
            get => model;
            set
            {
                model = value;
                model.PropertyChanged += OnModelPropertyChanged;
            }
        }

        public string ConnectionString
        {
            get { return Model?.ConnectionString; }
            set
            {
                Model.ConnectionString = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Column> Columns
        {
            get { return Model?.Columns; }
            set
            {
                Model.Columns = value;

                var a = from col in Model.Columns
                        group col by col.COLUMN_NAME into g
                        select new { Name = g.Key, Count = g.Count() };
                ColumnsCountCollection = CollectionViewSource.GetDefaultView(a);

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
        public object SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                dynamic t = value;
                var a = from col in Model.Columns
                        where col.COLUMN_NAME == t.Name
                        select new { DB = col.TABLE_CATALOG, Table = col.TABLE_NAME, Tp = col.DATA_TYPE, Col = col.COLUMN_NAME };
                ColumnDetails = CollectionViewSource.GetDefaultView(a);

                OnPropertyChanged();
            }
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

        public object SelectedItemColumnDetails
        {
            get => selectedItemColumnDetails;
            set
            {
                selectedItemColumnDetails = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Свойство указывает на то, что сейчас идёт поиск в базах данных
        /// данных по колонкам.
        /// </summary>
        public bool? IsSearchingNow
        {
            get => Model?.IsSearchingNow;
            set
            {
                Model.IsSearchingNow = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Log> Samples
        {
            get => samples;
            set
            {
                samples = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchingColumnsCommand { get; set; }

        public ICommand ColumnSampleInTableCommand { get; set; }
        public ICommand AllColumnSamplesInTablesCommand { get; set; }

        public SearchColumnsViewModel()
        {
            Samples = new ObservableCollection<Log>();

            //Model = new SearchColumnsTest1Model();
            Model = new SearchColumnsServerModel();
            SearchingColumnsCommand = new RelayCommand(
                p => SearchingColumns());

            ColumnSampleInTableCommand = new RelayCommand(p => ColumnSampleInTable());
            AllColumnSamplesInTablesCommand = new RelayCommand(p => AllColumnSamplesInTables());
        }

        private void SearchingColumns()
        {
            Model.SeachColumns();
            ColumnsCount();
        }

        private void AllColumnSamplesInTables()
        {
            foreach (dynamic d in ColumnDetails)
            {
                DetailedInfoAboutColumn(d);
            }
        }

        /// <summary>
        /// Сбор детализированой информации о конкретной колонке.
        /// TODO: убрать динамик, заменить на чёткую типизацию.
        /// </summary>
        /// <param name="d"></param>
        private void DetailedInfoAboutColumn(dynamic d)
        {
            Log log = new Log
            {
                DB = d.DB,
                Table = d.Table,
                Col = d.Col
            };

            string command1 = $"Select top 1 [{d.Col}] FROM [{d.DB}].[dbo].[{d.Table}]";
            string command2 = $"SELECT COUNT([{d.Col}]) FROM[{d.DB}].[dbo].[{d.Table}]";
            string command3 = $"select COUNT([{d.Col}]) from " +
                $"(SELECT distinct [{d.Col}] FROM[{d.DB}].[dbo].[{d.Table}]) as T";

            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                try
                {

                    using (var query = new SqlCommand(command1, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                log.Value = a.ToString();

                            }
                        }
                    }
                    using (var query = new SqlCommand(command2, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                log.CountRecsInColumn = Convert.ToInt64(a);
                            }
                        }
                    }
                    using (var query = new SqlCommand(command3, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                log.CountUniqueRecsInColumn = Convert.ToInt64(a);
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    log.IsError = true;
                    log.ErrorMsg = err.Message;
                }
            }

            Samples.Add(log);
        }

        private void ColumnSampleInTable()
        {
            dynamic d = SelectedItemColumnDetails;
            DetailedInfoAboutColumn(d);
        }

        private void ColumnsCount()
        {
            var a = from col in Model.Columns
                    group col by col.COLUMN_NAME into g
                    orderby g.Count() descending
                    select new { Name = g.Key, Count = g.Count() };
            ColumnsCountCollection = CollectionViewSource.GetDefaultView(a);
        }
    }
}
