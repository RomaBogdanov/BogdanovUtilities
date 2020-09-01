using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BogdanovUtilitisLib.MVVMUtilsWrapper;
using SqlAnalyzer.Views.Forms;

namespace SqlAnalyzer.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        public ICommand SearchColumnsCommand { get; set; }

        public MainWindowViewModel()
        {
            SearchColumnsCommand = new RelayCommand(p =>
            {
                SearchColumnsView searchColumnsView = new SearchColumnsView();
                searchColumnsView.Show();
            });
        }
    }
}
