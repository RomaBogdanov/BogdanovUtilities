using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Description;
using BogdanovUtilitisLib.MVVMUtilsWrapper;
using BogdanovUtilitisLib.LogsWrapper;
using System.Windows.Documents;

namespace BogdanovCodeAnalyzer.ViewModel
{
    class AnalyzeCodeViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<string> sTRS;

        public ObservableCollection<string> STRS
        {
            get => sTRS;
            set => sTRS = value;
        }

        public AnalyzeCodeViewModel()
        {
            STRS = new ObservableCollection<string>();
            STRS.Add("-------------dasfasd");
            STRS.Add("dsfasdffasd");

        }

    }
}
