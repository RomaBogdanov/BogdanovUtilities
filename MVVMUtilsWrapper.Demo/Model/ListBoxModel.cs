using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMUtilsWrapper.Demo.Data;

namespace MVVMUtilsWrapper.Demo.Model
{
    public class ListBoxModel
    {
        public ObservableCollection<People> Peoples
        {
            get; set;
        }

        public ListBoxModel()
        {
            Peoples = new ObservableCollection<People>
            {
                new People { Name= "Вася"},
                new People { Name="Петя"}
            };
        }
    }
}
