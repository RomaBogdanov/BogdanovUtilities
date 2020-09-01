using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MVVMUtilsWrapper.Demo.View
{
    /// <summary>
    /// Логика взаимодействия для ListViewGrouping2View.xaml
    /// </summary>
    public partial class ListViewGrouping2View : Window
    {
        public ListViewGrouping2View()
        {
            InitializeComponent();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lstView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Sex");
            view.GroupDescriptions.Add(groupDescription);
        }
    }
}
