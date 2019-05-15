using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using SintezLibrary;
using SintezWpfUiLib.Model;

namespace SintezWpfUiLib.ViewModel
{
    public class ChatViewModel : NotifyPropertyChanged
    {
        string writingMessage = "";
        string loadingFileName = "";
        string filterChatsList = "";
        ICollectionView filter;
        ICollectionView filterAccountsEntering;
        ICollectionView filterIssuesEntering;
        ICollectionView filterDesignsEntering;
        SintezUserRec currentGroup;
        Visibility isAddingGroup;
        Visibility isSearchingGroup;
        ChatModel model;
        SintezUserRec toAccount;
        GoalRec currentGoal;
        DocumentRec currentDoc;

        bool isAccountsEnteringPopupOpen = false;
        bool isIssuesEnteringPopupOpen = false;
        bool isDesignsEnteringPopupOpen = false;
        bool isAttentionMessage = false;

        public Visibility IsVisibleLeftPart
        {
            get { return Model != null ? Model.IsVisibleLeftPart : Visibility.Visible; }
            set
            {
                Model.IsVisibleLeftPart = value;
                OnPropertyChanged();
            }
        }

        public ChatModel Model
        {
            get { return model; }
            set
            {
                Logger.Debug("Присоединяем к контролу чата новый Model");
                model = value;
                Messages = model.Messages;
                Accounts = model.Accounts;
                Issues = model.Issues;
                Designes = model.Designes;
                Manager = model.Manager;
                IsVisibleLeftPart = model.IsVisibleLeftPart;

                // сделал для того, чтобы фильтрации не влияли на конечный список аккаунтов.
                List<SintezUserRec> forAccList1 = new List<SintezUserRec>(Accounts);
                List<SintezUserRec> forAccList2 = new List<SintezUserRec>(Accounts);
                Filter = CollectionViewSource.GetDefaultView(forAccList1);

                FilterAccountsEntering = CollectionViewSource.GetDefaultView(forAccList2);
                FilterIssuesEntering = CollectionViewSource.GetDefaultView(Issues);
                FilterDesignsEntering = CollectionViewSource.GetDefaultView(Designes);
                Logger.Debug("Присоединили к контролу чата новый Model");
            }
        }

        /// <summary>
        /// Тот, от имени кого заходят в чат
        /// </summary>
        public SintezUserRec Manager
        {
            get
            {
                return Model?.Manager;
            }
            set
            {
                Model.Manager = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список сообщений
        /// </summary>
        public ObservableCollection<ChatMsgRec> Messages
        {
            get
            {
                return Model?.Messages;
            }
            set
            {
                Model.Messages = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Аккаунт, на который передаётся сообщение
        /// </summary>
        public SintezUserRec ToAccount
        {
            get { return toAccount; }
            set
            {
                if (value != null)
                {
                    toAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Список аккаунтов
        /// </summary>
        public ObservableCollection<SintezUserRec> Accounts
        {
            get { return Model?.Accounts; }
            set
            {
                Model.Accounts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список задач
        /// </summary>
        public ObservableCollection<GoalRec> Issues
        {
            get { return Model?.Issues; }
            set
            {
                Model.Issues = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список задач
        /// </summary>
        public ObservableCollection<DocumentRec> Designes
        {
            get { return Model?.Designes; }
            set
            {
                Model.Designes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Привязаный к отправляемому сообщению макет.
        /// </summary>
        public DocumentRec AttachedDesign
        {
            get { return Model?.CurrentDoc; }
            set
            {
                if (Model != null)
                {
                    Model.CurrentDoc = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility IsAddingGroup
        {
            get { return isAddingGroup; }
            set
            {
                isAddingGroup = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsSearchingGroup
        {
            get { return isSearchingGroup; }
            set
            {
                isSearchingGroup = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отфильтрованый список групп
        /// </summary>
        public ICollectionView Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Состояние открытости выпадающего окна для поиска аккаунтов
        /// </summary>
        public bool IsAccountsEnteringPopupOpen
        {
            get { return isAccountsEnteringPopupOpen; }
            set
            {
                isAccountsEnteringPopupOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Состояние открытости выпадающего окна для поиска задач
        /// </summary>
        public bool IsIssuesEnteringPopupOpen
        {
            get { return isIssuesEnteringPopupOpen; }
            set
            {
                isIssuesEnteringPopupOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Состояние открытости выпадающего окна для поиска макетов
        /// </summary>
        public bool IsDesignsEnteringPopupOpen
        {
            get { return isDesignsEnteringPopupOpen; }
            set
            {
                isDesignsEnteringPopupOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отфильтрованый список вводимых аккаунтов.
        /// </summary>
        public ICollectionView FilterAccountsEntering
        {
            get { return filterAccountsEntering; }
            set
            {
                filterAccountsEntering = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отфильтрованый список вводимых задач.
        /// </summary>
        public ICollectionView FilterIssuesEntering
        {
            get { return filterIssuesEntering; }
            set
            {
                filterIssuesEntering = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отфильтрованый список вводимых дизайнов
        /// </summary>
        public ICollectionView FilterDesignsEntering
        {
            get { return filterDesignsEntering; }
            set
            {
                filterDesignsEntering = value;
                OnPropertyChanged();
            }
        }

        string Fl { get; set; } = "";

        /// <summary>
        /// Сообщение, которое записывается.
        /// </summary>
        public string WritingMessage
        {
            get { return writingMessage; }
            set
            {
                writingMessage = value;
                if (writingMessage != "")
                {
                    SmartAnalyzeWritingText();
                }
                else
                {
                    IsAccountsEnteringPopupOpen = false;
                    IsIssuesEnteringPopupOpen = false;
                    IsDesignsEnteringPopupOpen = false;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к загружаемому файлу
        /// </summary>
        public string LoadingFile { get; set; } = "";

        /// <summary>
        /// Имя загружаемого файла
        /// </summary>
        public string LoadingFileName
        {
            get { return loadingFileName; }
            set
            {
                loadingFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фильтр, по которому происходит фильтрация списка групп
        /// </summary>
        public string FilterChatsList
        {
            get { return filterChatsList; }
            set
            {
                filterChatsList = value;

                OnPropertyChanged();
                Filter.Filter = p =>
                {
                    if (((SintezUserRec)p).login.ToLower().Contains(
                        filterChatsList.ToLower()))
                    {
                        return true;
                    }
                    else return false;
                };
            }
        }

        /// <summary>
        /// Текущий выбранный аккаунт в списке аккаунтов.
        /// </summary>
        public SintezUserRec CurrentGroup
        {
            get { return currentGroup; }
            set
            {
                currentGroup = value;
                if (Model != null && currentGroup != null)
                {
                    Model.Talker = currentGroup;
                    Model.MessagesRelatedWithAccount(currentGroup);
                    Messages = Model.Messages;
                    ToAccount = currentGroup;
                    currentGroup.CountNotReadedMessages = 0;
                    CurrentGoal = null;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущая выбранная задача
        /// </summary>
        public GoalRec CurrentGoal
        {
            get { return currentGoal; }
            set
            {
                currentGoal = value;
                if (Model != null && currentGoal != null)
                {
                    Model.Talker = null;
                    Model.MessagesRelatedWithIssue(currentGoal);
                    Messages = Model.Messages;
                    CurrentGroup = null;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий выбранный макет
        /// </summary>
        public DocumentRec CurrentDoc
        {
            get { return currentDoc; }
            set
            {
                currentDoc = value;
                if (Model != null)
                {
                    Model.Talker = null;
                    Model.MessagesRelatedWithDoc(currentDoc);
                    Messages = Model.Messages;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Показывает важное ли это сообщение. True - важное,
        /// false - обычное.
        /// </summary>
        public bool IsAttentionMessage
        {
            get { return isAttentionMessage; }
            set
            {
                isAttentionMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда "отправить сообщение"
        /// </summary>
        public ICommand SendCommand { get; set; }

        /// <summary>
        /// Команда "загрузить файл"
        /// </summary>
        public ICommand LoadCommand { get; set; }

        /// <summary>
        /// Команда "загрузить файл(ы)" через Ctrl+V
        /// </summary>
        public ICommand InsertFilesCommand { get; set; }

        /// <summary>
        /// Команда "скачать файл"
        /// </summary>
        public ICommand DownloadFileCommand { get; set; }

        /// <summary>
        /// Команда "просмотреть файл"
        /// </summary>
        public ICommand ShowFileCommand { get; set; }

        /// <summary>
        /// Команда "удалить загруженный файл"
        /// </summary>
        public ICommand DeleteFileCommand { get; set; }

        /// <summary>
        /// Команда "удалить привязанный макет"
        /// </summary>
        public ICommand DeleteDesignCommand { get; set; }

        /// <summary>
        /// Команда "Нажатие на кнопку привязанного макета"
        /// </summary>
        public ICommand AttachedDesignPressCommand { get; set; }

        /// <summary>
        /// Команда добавления чата
        /// </summary>
        public ICommand AddGroupCommand { get; set; }

        /// <summary>
        /// Команда подтверждения добавления чата
        /// </summary>
        public ICommand ConfirmAddGroupCommand { get; set; }

        /// <summary>
        /// Команда отмены добавления чата
        /// </summary>
        public ICommand CancelAddGroupCommand { get; set; }

        /// <summary>
        /// Команда удаления чата
        /// </summary>
        public ICommand DeleteGroupCommand { get; set; }

        /// <summary>
        /// Команда формирования ответного сообщения
        /// </summary>
        public ICommand AnswerCommand { get; set; }

        public ChatViewModel()
        {
            Logger.Debug("Создаём ViewModel чата");
            //ChatViewModelSingleton.ViewModel = this;
            //Model = new ChatModel();
            SendCommand = new RelayCommand(Send());
            LoadCommand = new RelayCommand(LoadFile());
            InsertFilesCommand = new RelayCommand(InsertFiles());
            DeleteFileCommand = new RelayCommand(DeleteFile());
            DeleteDesignCommand = new RelayCommand(p => { AttachedDesign = null; });
            AddGroupCommand = new RelayCommand(AddGroup());
            ConfirmAddGroupCommand = new RelayCommand(ConfirmAddGroup());
            CancelAddGroupCommand = new RelayCommand(CancelAddGroup());
            DeleteGroupCommand = new RelayCommand(DeleteGroup());
            AnswerCommand = new RelayCommand(Answer());
            AttachedDesignPressCommand = new RelayCommand(AttachedDesignPress());

            DownloadFileCommand = new RelayCommand(DownloadFile());
            ShowFileCommand = new RelayCommand(ShowFile());

            IsSearchingGroup = Visibility.Visible;
            isAddingGroup = Visibility.Collapsed;
            Logger.Debug("Закончили создание ViewModel чата");
        }

        private Action<object> AttachedDesignPress()
        {
            return obj =>
            {
                ChatMsgRec chatMsgRec = obj as ChatMsgRec;
                // TODO: здесь разместить код по нажатию кнопки макета.
            };
        }

        private Action<object> Answer()
        {
            return obj =>
            {
                ChatMsgRec rec = obj as ChatMsgRec;
                var a = Accounts.FirstOrDefault(p => p.idx == rec.from_id);
                if (a == null) return;
                ToAccount = a;
                if (rec.doc_id == 0) return;
                AttachedDesign = new DocumentRec(rec.doc_id,
                    DBConnector.DBConnectionsDictionary[rec.doc_connection_id]);
            };
        }

        /// <summary>
        /// Процедура передачи нескольких файлов, в частности, в результате
        /// Drag&Drop или Ctrl+V
        /// </summary>
        /// <param name="list"></param>
        public void LoadListFiles(System.Collections.IList list)
        {
            if (list.Count == 1)
            {
                LoadingFile = list[0].ToString();
                if (FileNotEmpty(LoadingFile))
                {
                    LoadingFileName = Path.GetFileName(LoadingFile);
                }
            }
            else if (list.Count > 1)
            {
                foreach (string filename in list)
                {
                    LoadingFile = filename;
                    if (FileNotEmpty(LoadingFile))
                    {
                        LoadingFileName = Path.GetFileName(LoadingFile);
                        SendCommand.Execute(null);
                    }
                    //LoadingFileName = System.IO.Path.GetFileName(filename);
                }
            }
        }

        private Action<object> AddGroup()
        {
            return obj =>
            {
                IsSearchingGroup = Visibility.Collapsed;
                IsAddingGroup = Visibility.Visible;
            };
        }

        private Action<object> ConfirmAddGroup()
        {
            return obj =>
            {
                IsSearchingGroup = Visibility.Visible;
                IsAddingGroup = Visibility.Collapsed;
            };
        }

        private Action<object> CancelAddGroup()
        {
            return obj =>
            {
                IsSearchingGroup = Visibility.Visible;
                IsAddingGroup = Visibility.Collapsed;
            };
        }

        private Action<object> Send()
        {
            return obj =>
            {
                if (Model?.SendMessage(ToAccount, WritingMessage, IsAttentionMessage, LoadingFile) == true)
                {
                    WritingMessage = "";
                    LoadingFile = "";
                    LoadingFileName = "";
                    IsAttentionMessage = false;
                    //AttachedDesign = null;
                }
            };
        }

        private Action<object> LoadFile()
        {
            return obj =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {

                    LoadingFile = openFileDialog.FileName;
                    if (FileNotEmpty(LoadingFile))
                    {
                        LoadingFileName = Path.GetFileName(LoadingFile);
                    }
                }
            };
        }

        private Action<object> InsertFiles()
        {
            return obj =>
            {
                if (Clipboard.ContainsFileDropList())
                {
                    StringCollection filelist = Clipboard.GetFileDropList();
                    LoadListFiles(filelist);
                }
            };
        }

        private Action<object> DeleteFile()
        {
            return obj =>
            {
                LoadingFile = "";
                LoadingFileName = "";
            };
        }

        private Action<object> DeleteGroup()
        {
            return obj =>
            {

            };
        }

        /// <summary>
        /// Анализ вводимого текста.
        /// </summary>
        private void SmartAnalyzeWritingText()
        {
            if (writingMessage.Last() == '$' && !IsAccountsEnteringPopupOpen)
            {
                IsAccountsEnteringPopupOpen = true;
                IsIssuesEnteringPopupOpen = false;
                IsDesignsEnteringPopupOpen = false;
            }
            else if (writingMessage.Last() == '$' && IsAccountsEnteringPopupOpen)
            {
                FilterAccountsEntering.Filter = p => true;
            }
            else if (IsAccountsEnteringPopupOpen)
            {
                Match match = Regex.Match(writingMessage, @"(?<=\$)\w*$");
                if (match.Value != "")
                {
                    Fl = match.Value;
                    FilterAccountsEntering.Filter = p =>
                    {
                        if (((SintezUserRec)p).login.ToLower().Contains(Fl.ToLower()))
                        {
                            return true;
                        }
                        else return false;
                    };
                }
                else
                {
                    FilterAccountsEntering.Filter = p => true;
                }
            }
            else if (writingMessage.Last() == '#' && !IsIssuesEnteringPopupOpen)
            {
                IsIssuesEnteringPopupOpen = true;
                IsAccountsEnteringPopupOpen = false;
                IsDesignsEnteringPopupOpen = false;
            }
            else if (writingMessage.Last() == '#' && IsIssuesEnteringPopupOpen)
            {
                FilterIssuesEntering.Filter = p => true;
            }
            else if (IsIssuesEnteringPopupOpen)
            {
                Match match = Regex.Match(writingMessage, @"(?<=\#)\w*$");
                if (match.Value != "")
                {
                    Fl = match.Value;
                    FilterIssuesEntering.Filter = p =>
                    {
                        if (((GoalRec)p).goal.ToLower().Contains(Fl.ToLower()))
                        {
                            return true;
                        }
                        else return false;
                    };
                }
                else
                {
                    FilterIssuesEntering.Filter = p => true;
                }
            }
            else if (writingMessage.Last() == '%' && !IsDesignsEnteringPopupOpen)
            {
                IsIssuesEnteringPopupOpen = false;
                IsAccountsEnteringPopupOpen = false;
                IsDesignsEnteringPopupOpen = true;
            }
            else if (writingMessage.Last() == '%' && IsDesignsEnteringPopupOpen)
            {
                FilterDesignsEntering.Filter = p => true;
            }
            else if (IsIssuesEnteringPopupOpen)
            {
                Match match = Regex.Match(writingMessage, @"(?<=\%)\w*$");
                if (match.Value != "")
                {
                    Fl = match.Value;
                    FilterDesignsEntering.Filter = p =>
                    {
                        if (((DocumentRec)p).Production.ToLower().Contains(Fl.ToLower()))
                        {
                            return true;
                        }
                        else return false;
                    };
                }
                else
                {
                    FilterDesignsEntering.Filter = p => true;
                }
            }
        }

        /// <summary>
        /// Проверка, что файл не пустой. Если файл пустой, то пользователю предлагается выбор,
        /// прикреплять его или нет.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true - файл надо прикреплять, false - файл не надо прикреплять, 
        /// он пустой.</returns>
        private bool FileNotEmpty(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Length == 0)
            {
                var warnBoxResult = MessageBox.Show(
                    "Данный файл не содержит никаких данных. Вы уверены, " +
                    "что хотите прикрепить его?", "Сообщение о пустом файле",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (warnBoxResult != MessageBoxResult.Yes)
                {
                    LoadingFile = "";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Процедура загрузки файла.
        /// </summary>
        /// <returns></returns>
        private Action<object> DownloadFile()
        {
            return obj =>
            {
                ChatMsgRec rec = obj as ChatMsgRec;
                rec.saveToFile();
            };
        }

        /// <summary>
        /// Процедура просмотра файла.
        /// </summary>
        /// <returns></returns>
        private Action<object> ShowFile()
        {
            return obj =>
            {
                ChatMsgRec rec = obj as ChatMsgRec;
                rec.openFile();
            };
        }
    }
}
