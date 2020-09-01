using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SearchColumnsViewModel()
        {
            //Model = new SearchColumnsTest1Model();
            Model = new SearchColumnsServerModel();
            SearchingColumnsCommand = new RelayCommand(
                p => Model.SeachColumns());
        }
    }
}
