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
using System.Windows.Controls.Primitives;
using System.IO;
//using SintezLibrary;

namespace SintezWpfUiLib.View
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            //Logger.Debug("Начали создавать контрол ChatView");
            try
            {

            InitializeComponent();
            lbxMessages.Items.MoveCurrentToLast();
            lbxMessages.ScrollIntoView(lbxMessages.Items.CurrentItem);

            }
            catch (Exception err)
            {
              //  Logger.Log.Error(err.Message, err);
            }
            //Logger.Debug("Создали контрол ChatView");
        }

        private void lbxMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollingToLastElement();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScrollingToLastElement();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                ScrollingToLastElement();
            }
            if ((e.Key == Key.Up || e.Key == Key.Down) && popAccounts.IsOpen)
            {
                lbxAccountsWriting.Focus();
            }
            if ((e.Key == Key.Up || e.Key == Key.Down) && popIssues.IsOpen)
            {
                lbxIssuesWriting.Focus();
            }
            if ((e.Key == Key.Up || e.Key == Key.Down) && popDesigns.IsOpen)
            {
                lbxDesignsWriting.Focus();
            }
        }

        private void ScrollingToLastElement()
        {
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    lbxMessages.Items.MoveCurrentToLast();
                    lbxMessages.ScrollIntoView(lbxMessages.Items.CurrentItem);
                });
            });
            //tbxEnterMessage.Focus();
        }

        private void lbxAccountsWriting_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Down && ((ListBox)sender).SelectedIndex <
                ((ListBox)sender).Items.Count - 1)
            {
                ((ListBox)sender).SelectedIndex++;
                var listBoxItem = (ListBoxItem)((ListBox)sender)
                    .ItemContainerGenerator.ContainerFromItem(
                    ((ListBox)sender).SelectedItem);
                listBoxItem.Focus();
            }
            else if (e.Key == Key.Up && ((ListBox)sender).SelectedIndex > 0)
            {
                ((ListBox)sender).SelectedIndex--;
                var listBoxItem = (ListBoxItem)((ListBox)sender)
                    .ItemContainerGenerator.ContainerFromItem(
                    ((ListBox)sender).SelectedItem);
                listBoxItem.Focus();
            }
            if (e.Key == Key.Tab)
            {
                tbxEnterMessage.Focus();
                ((Popup)((Border)((ListBox)sender).Parent).Parent).IsOpen = false;
            }
            if (e.Key == Key.Enter)
            {
                if (((ListBox)sender).SelectedItem is SintezUserRec)
                {
                    SintezUserRec rec = ((ListBox)sender).SelectedItem as SintezUserRec;
                    string txt = DoingShortString(rec.fullname);
                    tbxEnterMessage.Text += $"\"{txt}({rec.baseid})\"";
                    ClosePopup(sender);
                }
                else if (((ListBox)sender).SelectedItem is GoalRec)
                {
                    GoalRec rec = ((ListBox)sender).SelectedItem as GoalRec;
                    string txt = DoingShortString(rec.goal);
                    tbxEnterMessage.Text += $"\"{txt}({rec.idx})\"";
                    ClosePopup(sender);
                }
                else if (((ListBox)sender).SelectedItem is DocumentRec)
                {
                    DocumentRec rec = ((ListBox)sender).SelectedItem as DocumentRec;
                    string txt = DoingShortString(rec.Production);
                    tbxEnterMessage.Text += $"\"{txt}({rec.idx}|{rec.connection.connector_id})\"";
                    ClosePopup(sender);
                }
            }*/
        }

        /// <summary>
        /// Укоротить строку при необходимости
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        private string DoingShortString(string rec)
        {
            string txt;
            if (rec.Length > 20)
            {
                txt = rec.Substring(0, 20) + "...";
            }
            else
            {
                txt = rec;
            }

            return txt;
        }

        /// <summary>
        /// Закрывает всплывающее окно и переводит фокус на поле ввода сообщения.
        /// </summary>
        /// <param name="sender"></param>
        private void ClosePopup(object sender)
        {
            tbxEnterMessage.Focus();
            ((Popup)((Border)((ListBox)sender).Parent).Parent).IsOpen = false;
        }

        /// <summary>
        /// Активируем функционал Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxEnterMessage_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Отрабатываем перетаскивание данных в сообщение по Drag&Drop. Если 1 файл,т
        /// то просто перетаскиваем, если несколько, отправляем несколькими сообщениями.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxEnterMessage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = e.Data.GetData(DataFormats.FileDrop) as string[];
                dynamic dyn = this.DataContext;
                dyn.LoadListFiles(filenames);
            }
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            {
                // Из Outlook тянем
                try
                {
                    Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor");
                    byte[] fileGroupDescriptor = new byte[513];
                    theStream.Read(fileGroupDescriptor, 0, 512);
                    StringBuilder fileName = new StringBuilder("");
                    int i = 76;
                    while (!(fileGroupDescriptor[i] == 0))
                    {
                        int val = fileGroupDescriptor[i];
                        char c = (char)val;
                        fileName.Append(c);
                        System.Math.Min(System.Threading.Interlocked.Increment(ref i), i - 1);
                    }

                    theStream.Close();
                    
                    string myTempFile = System.IO.Path.GetTempPath() + fileName.ToString();//ETConfig.localset.tmp_path + fileName.ToString();
                    MemoryStream ms = (MemoryStream)e.Data.GetData("FileContents");
                    byte[] FileBytes = new byte[Convert.ToInt32(ms.Length) + 1];
                    // read the raw data into our variable
                    ms.Position = 0;
                    ms.Read(FileBytes, 0, System.Convert.ToInt32(ms.Length));
                    ms.Close();
                    if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(myTempFile))
                    {
                        try
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(myTempFile);
                        }
                        catch (Exception ex)
                        {
                            Microsoft.VisualBasic.Interaction.MsgBox("Ошибка удаления файла " + myTempFile);
                            //return false;
                        }
                    }

                    // save the raw data into our temp file
                    FileStream fs = new FileStream(myTempFile, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Write(FileBytes, 0, FileBytes.Length - 1);
                    fs.Close();
                    // Make sure we have a actual file and also if we do make sure we erase it when done
                    if (!File.Exists(myTempFile))
                        return;
                    string[] filenames = new string[] { myTempFile };
                    dynamic dyn = this.DataContext;
                    dyn.LoadListFiles(filenames);
                    //return AddFile(myTempFile, false);
                }
                catch (Exception ex)
                {
                    Microsoft.VisualBasic.Interaction.MsgBox("Could not copy file from memory. Please save the file to your hard drive first and then retry your drag and drop.");
                   
                }
            }
        }

    }
}
