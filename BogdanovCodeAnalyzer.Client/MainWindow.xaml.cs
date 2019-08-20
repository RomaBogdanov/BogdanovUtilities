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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BogdanovCodeAnalyzer.Contracts.ContractServiceReference;

namespace BogdanovCodeAnalyzer.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IServiceBaseContract serviceBaseContract;
        public MainWindow()
        {
            InitializeComponent();
            //serviceBaseContractClient = new ServiceBaseContractClient();
            serviceBaseContract = new ServiceBaseContractClient(
                new System.ServiceModel.InstanceContext(new Callback()));
            //BogdanovCodeAnalyzer.Client.ServiceReferenceCodeAnalyzer.IServiceBaseContract serviceBase = new BogdanovCodeAnalyzer.Client.ServiceReferenceCodeAnalyzer.ServiceBaseContractClient();
            serviceBaseContract.Log("Тестовое сообщенище", "ТЭГ", "ТОТ МЕТОД", "ФАЙЛ");
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            serviceBaseContract.Log("Тест", "DEBUG", "", "");
        }
    }

    class Callback : BogdanovCodeAnalyzer.Contracts.ContractServiceReference.IServiceBaseContractCallback
    {
        public void StartLogs()
        {
            throw new NotImplementedException();
        }

        public void StopLogs()
        {
            throw new NotImplementedException();
        }
    }
}
