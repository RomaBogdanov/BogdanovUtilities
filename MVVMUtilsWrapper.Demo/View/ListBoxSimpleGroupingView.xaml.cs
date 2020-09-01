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
    /// Логика взаимодействия для ListBoxSimpleGroupingView.xaml
    /// </summary>
    public partial class ListBoxSimpleGroupingView : Window
    {
        public ListBoxSimpleGroupingView()
        {
            InitializeComponent();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lstBox.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Sex");
            view.GroupDescriptions.Add(groupDescription);

        }
    }
}
