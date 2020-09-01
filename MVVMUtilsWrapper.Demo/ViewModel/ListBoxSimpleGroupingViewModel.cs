using BogdanovUtilitisLib.MVVMUtilsWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    public class ListBoxSimpleGroupingViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<User> users;

        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public ListBoxSimpleGroupingViewModel()
        {
            Users = new ObservableCollection<User>();
            Users.Add(new User { Name = "John Doe", Age = 42, Sex = SexType.Male });
            Users.Add(new User { Name = "Jane Doe", Age = 39, Sex = SexType.Female });
            Users.Add(new User { Name = "Sammy Doe", Age = 13, Sex = SexType.Male });
        }

    }


    public enum SexType { Male, Female };

    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Mail { get; set; }

        public SexType Sex { get; set; }
    }
}
