using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отображаемая на GUI коллекция колонок сервера БД с количеством
        /// раз повторений этого названия на сервере БД.
        /// </summary>
        public ICollectionView ColumnsCountCollection
        {
            get => Model?.ColumnsCountCollection;
            set
            {
                Model.ColumnsCountCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранная для получения более детальной информации колонка.
        /// </summary>
        public RepeatingColumn SelectedColumn
        {
            get => Model?.SelectedColumn;
            set
            {
                Model.SelectedColumn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Детализированый список по колонке.
        /// </summary>
        public ICollectionView ColumnDetails
        {
            get => Model?.ColumnDetails;
            set
            {
                Model.ColumnDetails = value;
                OnPropertyChanged();
            }
        }

        public Column SelectedItemColumnDetails
        {
            get => Model?.SelectedItemColumnDetails;
            set
            {
                Model.SelectedItemColumnDetails = value;
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

        public ObservableCollection<object> UniqueValuesInColumn
        {
            get => Model?.UniqueValuesInColumn;
            set
            {
                Model.UniqueValuesInColumn = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchingColumnsCommand { get; set; }
        public ICommand ColumnSampleInTableCommand { get; set; }
        public ICommand AllColumnSamplesInTablesCommand { get; set; }
        public ICommand SearchUniqueValuesInColumnCommand { get; set; }

        public SearchColumnsViewModel()
        {
            //Model = new SearchColumnsTest1Model();
            Model = new SearchColumnsServerModel();
            SearchingColumnsCommand = new RelayCommand(
                p => SearchingColumns());

            ColumnSampleInTableCommand = new RelayCommand(p => ColumnSampleInTable());
            AllColumnSamplesInTablesCommand = new RelayCommand(p => AllColumnSamplesInTables());
            SearchUniqueValuesInColumnCommand = new RelayCommand(p => SearchUniqueValuesInColumn());
        }

        private void SearchUniqueValuesInColumn()
        {
            Model?.SearchUniqueValuesInColumn();
        }

        /// <summary>
        /// Нахождение всех колонок.
        /// </summary>
        private void SearchingColumns()
        {
            Model.SearchColumns();
        }

        private void ColumnSampleInTable()
        {
            Model.DetailedInfoAboutColumn(SelectedItemColumnDetails);

            //TODO: код помойный, надо думать, как напрямую изменять свойства в коллекции.
            // или вызывать при полной обработке.
            Model.DetailingSelectedColumn(SelectedColumn);
        }

        private void AllColumnSamplesInTables()
        {
            foreach (var d in ColumnDetails)
            {
                Model.DetailedInfoAboutColumn(d as Column);
            }

            //TODO: код помойный, надо думать, как напрямую изменять свойства в коллекции.
            // или вызывать при полной обработке.
            Model.DetailingSelectedColumn(SelectedColumn);
        }
    }
}
