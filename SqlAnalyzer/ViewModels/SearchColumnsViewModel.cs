using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                        select new { DB = col.TABLE_CATALOG, Table = col.TABLE_NAME, Tp = col.DATA_TYPE };
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

        public ICommand SearchingColumnsCommand { get; set; }

        public ICommand UpdataCommand { get; set; }

        public ICommand ColumnsCountCommand { get; set; }

        public SearchColumnsViewModel()
        {
            //Model = new SearchColumnsTest1Model();
            Model = new SearchColumnsServerModel();
            SearchingColumnsCommand = new RelayCommand(
                p => Model.SeachColumns());
            UpdataCommand = new RelayCommand(p => Updata());
            ColumnsCountCommand = new RelayCommand(p => ColumnsCount());

            ColumnsView = CollectionViewSource.GetDefaultView(Columns);
            ColumnsView.GroupDescriptions.Add(new PropertyGroupDescription("COLUMN_NAME"));

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
}
