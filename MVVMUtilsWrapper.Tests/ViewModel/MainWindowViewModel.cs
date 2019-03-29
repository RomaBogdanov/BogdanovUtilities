using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMUtilsWrapper.Tests.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        string txtField;

        public string TxtField
        {
            get { return txtField; }
            set
            {
                txtField = value;
                OnPropertyChanged();
            }
        }

        public ICommand Command { get; set; }

        public MainWindowViewModel()
        {
            Command = new RelayCommand(CommandRealize());
        }

        private Action<object> CommandRealize()
        {
            return x =>
            {
                TxtField = "Всё получилось!!!";
            };
        }
    }
}
