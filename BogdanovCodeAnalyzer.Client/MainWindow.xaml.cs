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
        Callback callback;

        bool isPermitToLogs = false;

        public MainWindow()
        {
            InitializeComponent();
            //serviceBaseContractClient = new ServiceBaseContractClient();
            callback = new Callback();
            callback.OnStartLogs += Callback_OnStartLogs;
            callback.OnStopLogs += Callback_OnStopLogs;
            serviceBaseContract = new ServiceBaseContractClient(
                new System.ServiceModel.InstanceContext(callback));
            //BogdanovCodeAnalyzer.Client.ServiceReferenceCodeAnalyzer.IServiceBaseContract serviceBase = new BogdanovCodeAnalyzer.Client.ServiceReferenceCodeAnalyzer.ServiceBaseContractClient();
            if (isPermitToLogs)
            {
                serviceBaseContract.Log("Тестовое сообщенище", "ТЭГ", "ТОТ МЕТОД", "ФАЙЛ");
            }

        }

        private void Callback_OnStopLogs()
        {
            isPermitToLogs = false;
        }

        private void Callback_OnStartLogs()
        {
            isPermitToLogs = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (isPermitToLogs)
            {
                serviceBaseContract.Log("Тест", "DEBUG", "", "");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            serviceBaseContract.Connect();
        }
    }

    class Callback : BogdanovCodeAnalyzer.Contracts.ContractServiceReference.IServiceBaseContractCallback
    {
        public event Action OnStartLogs;
        public event Action OnStopLogs;

        public void StartLogs()
        {
            OnStartLogs?.Invoke();
        }

        public void StopLogs()
        {
            OnStopLogs?.Invoke();
        }
    }
}
