using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BogdanovUtilitisLib.MVVMUtilsWrapper
{
    /// <summary>
    /// Базовый класс для классов ViewModel с общим функционалом
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Для передачи данных от модел до вьюмодел.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}
