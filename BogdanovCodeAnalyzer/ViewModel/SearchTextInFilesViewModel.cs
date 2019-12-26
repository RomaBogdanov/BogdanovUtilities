using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BogdanovUtilitisLib.MVVMUtilsWrapper;
using System.Windows.Input;

namespace BogdanovCodeAnalyzer.ViewModel
{
    class SearchTextInFilesViewModel : NotifyPropertyChanged
    {
        private string searchingText;
        private string searchingPath;
        private ObservableCollection<string> searchedPaths;
        private ObservableCollection<string> allExtents;
        private ObservableCollection<string> filteredExtents;

        /// <summary>
        /// Искомый текст, который должен содержаться в файле.
        /// </summary>
        public string SearchingText
        {
            get => searchingText;
            set
            {
                searchingText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к списку файлов, в которых ищется текст.
        /// </summary>
        public string SearchingPath
        {
            get => searchingPath;
            set
            {
                searchingPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список файлов, в которых содержится текст.
        /// </summary>
        public ObservableCollection<string> SearchedPaths
        {
            get => searchedPaths;
            set
            {
                searchedPaths = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AllExtents
        {
            get => allExtents;
            set
            {
                allExtents = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> FilteredExtents
        {
            get => filteredExtents;
            set
            {
                filteredExtents = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Запускает поиск файлов, в которых содержится данная строка.
        /// </summary>
        public ICommand SearchFilesWithTextCommand { get; set; }

        public ICommand AllExtentionsCommand { get; set; }

        List<string> exts;

        public SearchTextInFilesViewModel()
        {
            SearchFilesWithTextCommand = new RelayCommand(obj => SearchFilesWithText());
            AllExtentionsCommand = new RelayCommand(obj => AllExtentions());
            SearchingPath = @"D:\Repository";

            exts = new List<string>()
            {
                "config",
                "htm",
                "csproj",
                "cs",
                "resx",
                "rtf",
                "txt",
                "settings",
                "xml",
                "html",
                "doc"
            };
        }

        /// <summary>
        /// Находит все файлы, содержащие текст.
        /// </summary>
        private void SearchFilesWithText()
        {
            
            string[] allFiles = System.IO.Directory.GetFiles(SearchingPath,
                "*.*", System.IO.SearchOption.AllDirectories).Where(
                p => exts.Contains(p.Split('.').Last())).ToArray();
            SearchedPaths = new ObservableCollection<string>();

            foreach (var item in allFiles)
            {
                try
                {
                    string str = System.IO.File.ReadAllText(item);
                    if (System.Text.RegularExpressions.Regex.IsMatch(str, SearchingText))
                    {
                        SearchedPaths.Add(item);
                    }
                }
                catch (Exception err)
                {
                    SearchedPaths.Add(err.Message);
                }
            }
        }

        /// <summary>
        /// Находит все расширения файлов
        /// </summary>
        private void AllExtentions()
        {
            string[] allFiles = System.IO.Directory.GetFiles(SearchingPath,
                "*", System.IO.SearchOption.AllDirectories);
            AllExtents = new ObservableCollection<string>(allFiles.Select(
                p => p.Split('.').Last()).Distinct());
        }

    }
}
