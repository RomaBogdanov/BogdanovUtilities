﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using r = BogdanovUtilitisLib.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace BogdanovCodeAnalyzer.ViewModel
{
    /// <summary>
    /// Предлагает логику для работы контролов по переработке кода.
    /// </summary>
    public class CoverCodeViewModel : NotifyPropertyChanged
    {
        const string inDevelop = "Функционал находится в разработке";
        const string pathToAutoSave = "SettingsUpdataInform.txt";
        private string pathToFiles = @"C:\BogdanovR\Experiments\Sintez\";
        private string addedNamespace = "SintezLibrary";
        private bool isLogsToMethods = true;
        private bool isLogsToCatches = true;
        private ObservableCollection<string> notIgnoredPaths = new ObservableCollection<string>();
        private ObservableCollection<string> ignoredPaths = new ObservableCollection<string>();
        private string informText = "";
        private bool isEnableUpdataCommand = true;
        private string selectedNotIgnoredFile;
        private string selectedIgnoredFile;

        /// <summary>
        /// Путь по которому ищутся файлы.
        /// </summary>
        public string PathToFiles
        {
            get => pathToFiles;
            set
            {
                pathToFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Добавляемое в файлы пространство имён
        /// </summary>
        public string AddedNamespace
        {
            get => addedNamespace;
            set
            {
                addedNamespace = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Указывает, надо ли покрывать логами методы
        /// </summary>
        public bool IsLogsToMethods
        {
            get => isLogsToMethods;
            set
            {
                isLogsToMethods = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Указывает, надо ли покрывать логама catch
        /// </summary>
        public bool IsLogsToCatches
        {
            get => isLogsToCatches;
            set
            {
                isLogsToCatches = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список файлов для обновления
        /// </summary>
        public ObservableCollection<string> NotIgnoredPaths
        {
            get => notIgnoredPaths;
            set
            {
                notIgnoredPaths = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список необновляемых файлов
        /// </summary>
        public ObservableCollection<string> IgnoredPaths
        {
            get => ignoredPaths;
            set
            {
                ignoredPaths = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Информационный текст, выводимый на контрол
        /// </summary>
        public string InformText
        {
            get => informText;
            set
            {
                informText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Определяет возможность для пользователя запустить процедуру аптейта.
        /// </summary>
        public bool IsEnableUpdataCommand
        {
            get => isEnableUpdataCommand;
            set
            {
                isEnableUpdataCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный файл из списка обрабатываемых
        /// </summary>
        public string SelectedNotIgnoredFile
        {
            get => selectedNotIgnoredFile;
            set
            {
                selectedNotIgnoredFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный файл из списка игнорируемых
        /// </summary>
        public string SelectedIgnoredFile
        {
            get => selectedIgnoredFile;
            set
            {
                selectedIgnoredFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда для открытия настроек
        /// </summary>
        public ICommand OpenSettingsCommand { get; set; }
        /// <summary>
        /// Команда для сохранения настроек
        /// </summary>
        public ICommand SaveSettingsCommand { get; set; }
        /// <summary>
        /// Команда для получения списка всех файлов под обновление
        /// </summary>
        public ICommand GetFilesListCommand { get; set; }
        /// <summary>
        /// Команда для обновления файлов
        /// </summary>
        public ICommand UpdateCodeCommand { get; set; }
        /// <summary>
        /// Команда для переноса файла в список игнорируемых
        /// </summary>
        public ICommand IgnoreFileCommand { get; set; }
        /// <summary>
        /// Команда для переноса файла в список под обновление
        /// </summary>
        public ICommand NotIgnoreFileCommand { get; set; }

        public CoverCodeViewModel()
        {
            OpenSettingsCommand = new RelayCommand(obj => OpenSettings());
            SaveSettingsCommand = new RelayCommand(obj => SaveSettings());
            GetFilesListCommand = new RelayCommand(obj => GetFilesList());
            UpdateCodeCommand = new RelayCommand(obj => UpdateCode());
            IgnoreFileCommand = new RelayCommand(obj => IgnoreFile());
            NotIgnoreFileCommand = new RelayCommand(obj => NotIgnoreFile());

            OpenSettings();
        }

        private void OpenSettings()
        {
            IgnoredPaths.Clear();
            if (!System.IO.File.Exists(pathToAutoSave)) return;
            var a = System.IO.File.ReadAllLines(pathToAutoSave);
            foreach (var item in a)
            {
                IgnoredPaths.Add(item);
            }
        }

        private void SaveSettings()
        {
            System.IO.File.WriteAllLines(pathToAutoSave, IgnoredPaths);
        }

        /// <summary>
        /// Получаем список файлов, в которых надо будет провести обновления.
        /// </summary>
        private void GetFilesList()
        {
            NotIgnoredPaths = new ObservableCollection<string>(
                System.IO.Directory.EnumerateFiles(PathToFiles, "*.cs",
                System.IO.SearchOption.AllDirectories)
                .Where(p => !p.ToUpper().Contains("DESIGNER.CS"))
                .Except(IgnoredPaths));
        }

        /// <summary>
        /// Обновляет выбранные файлы в соответствии с выбранными настройками.
        /// </summary>
        private void UpdateCode()
        {
            InformText += $"Запуск обновления файлов{Environment.NewLine}...{Environment.NewLine}";
            IsEnableUpdataCommand = false;
            Task.Run(() =>
            {
                r.CodeGenerator codeGenerator = new r.CodeGenerator();
                r.CodeAnalyzer codeAnalyzer;
                foreach (var item in NotIgnoredPaths)
                {
                    //TextInTextBlock += item + Environment.NewLine;
                    codeAnalyzer = new r.CodeAnalyzer(item);
                    var root = codeAnalyzer.SyntaxTree.GetRoot();

                    // вставляем логи в начало и конец методов
                    if (IsLogsToMethods)
                    {
                        var methods = codeAnalyzer.SearchMethods();
                        var expression1 = codeGenerator.CreatingCallProcedureExpression(
                            "Logger.Debug", new List<string> { "\"Начало метода\"" });
                        var expression2 = codeGenerator.CreatingCallProcedureExpression(
                            "Logger.Debug", new List<string> { "\"Окончание метода\"" });

                        Func<SyntaxNode, SyntaxNode, SyntaxNode> func = (x, y) =>
                        {
                            y = codeGenerator.AddExpressionToStartMethodsBody(
                               x as MethodDeclarationSyntax, expression1);
                            y = codeGenerator.AddExpressionToFinishOrBeforeReturnMethodsBody(
                                y as MethodDeclarationSyntax, expression2);
                            return y;
                        };
                        root = root.ReplaceNodes(methods, func);
                    }

                    // вставляем логи в catch
                    if (IsLogsToCatches)
                    {
                        var catchs = codeAnalyzer.SearchCatches(root);
                        Func<SyntaxNode, SyntaxNode, SyntaxNode> func1 = (x, y) =>
                        {
                            y = codeGenerator.AddExpressionToCatchConstructionMethodsBody(
                                x as CatchClauseSyntax, "Logger.Error", "err", "Exception");
                            return y;
                        };
                        root = root.ReplaceNodes(catchs, func1);
                    }

                    // вставляем пространство имён
                    string nameNamespace = AddedNamespace;
                    UsingDirectiveSyntax usdir = codeGenerator.CreatingUsingDirective(nameNamespace);
                    List<SyntaxNode> usdirs = codeAnalyzer.SearchLinkedNamespaces(root);

                    if (usdirs.FirstOrDefault(p => ((p as UsingDirectiveSyntax).Name
                        .GetText().ToString() == nameNamespace)) == null && usdirs.Count > 0)
                    {
                        root = root.InsertNodesAfter(usdirs[usdirs.Count - 1],
                            new List<SyntaxNode> { usdir });
                    }
                    System.IO.File.WriteAllText(item, root.NormalizeWhitespace().GetText().ToString(), Encoding.UTF8);
                }
                InformText += $"Окончание обновления файлов{Environment.NewLine}";
                IsEnableUpdataCommand = true;
            });
        }

        private void IgnoreFile()
        {
            if (string.IsNullOrEmpty(SelectedNotIgnoredFile))
            {
                return;
            }
            IgnoredPaths.Add(SelectedNotIgnoredFile);
            NotIgnoredPaths.Remove(SelectedNotIgnoredFile);
        }

        private void NotIgnoreFile()
        {
            if (string.IsNullOrEmpty(SelectedIgnoredFile))
            {
                return;
            }
            NotIgnoredPaths.Add(SelectedIgnoredFile);
            IgnoredPaths.Remove(SelectedIgnoredFile);
        }
    }
}
