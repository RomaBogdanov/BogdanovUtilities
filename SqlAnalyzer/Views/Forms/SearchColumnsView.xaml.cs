using SqlAnalyzer.ViewModels;
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

namespace SqlAnalyzer.Views.Forms
{
    /// <summary>
    /// Логика взаимодействия для SearchColumns.xaml
    /// </summary>
    public partial class SearchColumnsView : Window
    {
        public SearchColumnsView()
        {
            InitializeComponent();
            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lstViewGroup.ItemsSource);
            //PropertyGroupDescription groupDescription = new PropertyGroupDescription("COLUMN_NAME");
            //view.GroupDescriptions.Add(groupDescription);
        }

    }
}
