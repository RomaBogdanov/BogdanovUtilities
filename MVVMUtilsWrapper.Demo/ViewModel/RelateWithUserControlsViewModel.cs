using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMUtilsWrapper.Demo.Data;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    class RelateWithUserControlsViewModel : NotifyPropertyChanged
    {
        People current;

        public ObservableCollection<People> Peoples
        {
            get; set;
        }

        public People Current
        {
            get { return current; }
            set
            {
                current = value;
            }
        }

        public ICommand PressCommand { get; set; }

        public RelateWithUserControlsViewModel()
        {
            PressCommand = new RelayCommand(v => { System.Windows.MessageBox.Show("Запустил!!!"); });

            Current = new People { Name = "Вася", Surname = "Пупкин" };
            Peoples = new ObservableCollection<People>
            {
                new People{ Name="Петя", Surname= "Иванов"},
                new People{ Name="Саша", Surname="Кириленко"}
            };
        }
    }

}
