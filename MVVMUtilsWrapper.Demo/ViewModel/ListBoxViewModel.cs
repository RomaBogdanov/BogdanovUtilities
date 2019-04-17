using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMUtilsWrapper.Demo.Data;
using MVVMUtilsWrapper.Demo.Model;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    public class ListBoxViewModel : NotifyPropertyChanged
    {
        ListBoxModel model;

        public ListBoxModel Model
        {
            get { return model; }
            set
            {
                model = value;
                Peoples = model.Peoples;
            }
        }

        public ObservableCollection<People> Peoples
        {
            get { return Model?.Peoples; }
            set
            {
                if (Model != null)
                {
                    Model.Peoples = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand PeopleNameCommand { get; set; }

        public ListBoxViewModel()
        {
            PeopleNameCommand = new RelayCommand(obj =>
            {
                People people = obj as People;
                System.Windows.MessageBox.Show(people.Name);
            });
        }
    }
}
