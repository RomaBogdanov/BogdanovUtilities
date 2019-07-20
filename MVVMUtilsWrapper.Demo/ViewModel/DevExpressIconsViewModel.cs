using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Data;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    public class DevExpressIconsViewModel : NotifyPropertyChanged
    {
        ObservableCollection<IconPath> iconPaths;
        string folderForSearch = "";
        string filter = "";
        ICollectionView filterMass;

         public ObservableCollection<IconPath> IconPaths
        {
            get { return iconPaths; }
            set
            {
                iconPaths = value;
                OnPropertyChanged();
            }
        }

        public string FolderForSearch
        {
            get { return folderForSearch; }
            set
            {
                folderForSearch = value;
                IconPaths.Clear();
                string dict = $@"..\..\..\DevExpressIcons\{folderForSearch}";
                string[] dictionars = Directory.GetDirectories(dict);
                foreach (var d in dictionars)
                {
                    string[] files = System.IO.Directory.GetFiles(d);
                    foreach (var file in files)
                    {
                        IconPaths.Add(new IconPath
                        {
                            FullPath = Path.GetFullPath(file),
                            ShortPath = $"{d.Split('\\').Last()}\\{Path.GetFileName(file)}"
                        });
                    }
                }

                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                filterMass.Filter = p =>
                {
                    if (((IconPath)p).ShortPath.ToLower().Contains(filter.ToLower()))
                    {
                        return true;
                    }
                    else return false;
                };
                OnPropertyChanged();
            }
        }

        public DevExpressIconsViewModel()
        {
            string dict = @"..\..\..\DevExpressIcons\Images";
            //string dict = @"C:\Program Files (x86)\DevExpress 17.2\Components\Sources\DevExpress.Images\DevAV\";
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
            filterMass = CollectionViewSource.GetDefaultView(iconPaths);
        }
    }

    public class IconPath
    {
        public string FullPath { get; set; }

        public string ShortPath { get; set; }
    }
}
