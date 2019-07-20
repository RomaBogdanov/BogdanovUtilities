﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMUtilsWrapper.Demo.View;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        public ICommand OpenDragAndDropCommand { get; set; }

        public ICommand OpenListBoxCommand { get; set; }

        public ICommand OpenAllDevExpressIcons { get; set; }

        public ICommand RelateWithUCCommand { get; set; }

        public MainWindowViewModel()
        {
            OpenDragAndDropCommand = new RelayCommand(obj =>
            {
                DragAndDropView dragAndDropView = new DragAndDropView();
                dragAndDropView.Show();
            });
            OpenListBoxCommand = new RelayCommand(obj =>
            {
                ListBoxView listBoxView = new ListBoxView();
                ((ListBoxViewModel)listBoxView.DataContext).Model = new Model.ListBoxModel();
                listBoxView.Show();
            });
            OpenAllDevExpressIcons = new RelayCommand(obj =>
            {
                DevExpressIconsView devExpressIconsView = new DevExpressIconsView();
                devExpressIconsView.Show();
            });
            RelateWithUCCommand = new RelayCommand(obj =>
            {
                RelateWithUserControlsView relateWithUserControlsView = new RelateWithUserControlsView();
                relateWithUserControlsView.Show();
            });
        }
    }
}
