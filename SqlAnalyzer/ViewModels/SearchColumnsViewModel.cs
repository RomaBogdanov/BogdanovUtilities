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
        private ICollectionView columnsView;
        private ICollectionView columnFilter1;
        private object selectedItemColumnFilter1;
        private ICollectionView columnFilter2;
        private ObservableCollection<Log> samples;
        private object selectedItemColumnFilter2;

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
                ColumnsView.GroupDescriptions.Clear();
                ColumnsView = CollectionViewSource.GetDefaultView(Model.Columns);
                ColumnsView.GroupDescriptions.Add(new PropertyGroupDescription("COLUMN_NAME"));


                var a = from col in Model.Columns
                        group col by col.COLUMN_NAME into g
                        select new { Name = g.Key, Count = g.Count() };
                ColumnFilter1 = CollectionViewSource.GetDefaultView(a);

                OnPropertyChanged();
            }
        }

        public ICollectionView ColumnsView
        {
            get => columnsView;
            set
            {
                columnsView = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ColumnFilter1
        {
            get => columnFilter1;
            set
            {
                columnFilter1 = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItemColumnFilter1
        {
            get => selectedItemColumnFilter1;
            set
            {
                selectedItemColumnFilter1 = value;
                dynamic t = value;
                var a = from col in Model.Columns
                        where col.COLUMN_NAME == t.Name
                        select new { DB = col.TABLE_CATALOG, Table = col.TABLE_NAME, Tp = col.DATA_TYPE, Col = col.COLUMN_NAME };
                ColumnFilter2 = CollectionViewSource.GetDefaultView(a);

                OnPropertyChanged();
            }
        }

        public ICollectionView ColumnFilter2
        {
            get => columnFilter2;
            set
            {
                columnFilter2 = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItemColumnFilter2
        {
            get => selectedItemColumnFilter2;
            set
            {
                selectedItemColumnFilter2 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
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

        public ICommand UpdataCommand { get; set; }

        public ICommand ColumnsCountCommand { get; set; }

        public ICommand ColumnSampleInTableCommand { get; set; }
        public ICommand AllColumnSamplesInTablesCommand { get; set; }

        public SearchColumnsViewModel()
        {
            Samples = new ObservableCollection<Log>();

            //Model = new SearchColumnsTest1Model();
            Model = new SearchColumnsServerModel();
            SearchingColumnsCommand = new RelayCommand(
                p => SearchingColumns());

            UpdataCommand = new RelayCommand(p => Updata());
            ColumnsCountCommand = new RelayCommand(p => ColumnsCount());
            ColumnSampleInTableCommand = new RelayCommand(p => ColumnSampleInTable());
            AllColumnSamplesInTablesCommand = new RelayCommand(p => AllColumnSamplesInTables());

            ColumnsView = CollectionViewSource.GetDefaultView(Columns);
            ColumnsView.GroupDescriptions.Add(new PropertyGroupDescription("COLUMN_NAME"));

        }

        private void SearchingColumns()
        {
            Model.SeachColumns();
            ColumnsCount();
        }

        private void AllColumnSamplesInTables()
        {
            foreach (dynamic d in ColumnFilter2)
            {
                string command = $"Select top 1 {d.Col} FROM [{d.DB}].[dbo].[{d.Table}]";

                using (var sc = new SqlConnection(ConnectionString))
                {
                    sc.Open();
                    using (var query = new SqlCommand(command, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                Samples.Add(new Log
                                {
                                    Value = a.ToString(),
                                    DB = d.DB,
                                    Table = d.Table,
                                    Col = d.Col
                                });
                            }
                        }
                    }
                }
            }
        }

        private void ColumnSampleInTable()
        {
            dynamic d = SelectedItemColumnFilter2;
            string command = $"Select top 1 {d.Col} FROM [{d.DB}].[dbo].[{d.Table}]";

            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                using (var query = new SqlCommand(command, sc))
                {
                    using (var res = query.ExecuteReader())
                    {
                        while (res.Read())
                        {
                            var a = res[0];
                            Samples.Add(new Log
                            {
                                Value = a.ToString(),
                                DB = d.DB,
                                Table = d.Table,
                                Col = d.Col
                            });
                        }
                    }
                }
            }
        }

        private void ColumnsCount()
        {
            var a = from col in Model.Columns
                    group col by col.COLUMN_NAME into g
                    orderby g.Count() descending
                    select new { Name = g.Key, Count = g.Count() };
            ColumnFilter1 = CollectionViewSource.GetDefaultView(a);

        }

        private void Updata()
        {
            ColumnsView = CollectionViewSource.GetDefaultView(Columns);
            ColumnsView.GroupDescriptions.Add(new PropertyGroupDescription("COLUMN_NAME"));
            //ColumnsView.SortDescriptions.Add(new SortDescription("ItemCount", ListSortDirection.Descending));
        }
    }

    public class Log
    {
        public string Value { get; set; }
        public string DB { get; set; }
        public string Table { get; set; }
        public string Col { get; set; }
    }
}
