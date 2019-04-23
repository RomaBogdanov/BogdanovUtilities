using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    public class DevExpressIconsViewModel : NotifyPropertyChanged
    {
        ObservableCollection<IconPath> iconPaths;

         public ObservableCollection<IconPath> IconPaths
        {
            get { return iconPaths; }
            set
            {
                iconPaths = value;
                OnPropertyChanged();
            }
        }

        public DevExpressIconsViewModel()
        {
            string dict = @"..\..\..\DevExpressIcons\Images";
            //string dict = @"C:\Program Files (x86)\DevExpress 17.2\Components\Sources\DevExpress.Images\Images\";
            //string dict = @"C:\Program Files (x86)\DevExpress 17.2\Components\Sources\DevExpress.Images\GrayScaleImages\";
            string[] dictionars = Directory.GetDirectories(dict);
            iconPaths = new ObservableCollection<IconPath>();
            foreach (var d in dictionars)
            {
                string[] files = System.IO.Directory.GetFiles(d);
                foreach (var file in files)
                {
                    iconPaths.Add(new IconPath
                    {
                        FullPath = Path.GetFullPath(file),
                        ShortPath = Path.GetFileName(file)
                    });

                }
            }
        }
    }

    public class IconPath
    {
        public string FullPath { get; set; }

        public string ShortPath { get; set; }
    }
}
