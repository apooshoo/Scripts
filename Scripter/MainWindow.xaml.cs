using Microsoft.Win32;
using Scripter.Models;
using Scripter.Services;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scripter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FolderSelectionOption[] _folderSelectionOptions { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            Setup();
            DataContext = this;
            WriteLogToUi("Initialised.");
        }

        private void Setup()
        {
            _folderSelectionOptions = FolderSelectionService.GetDefaultOptions();
            FolderSelectionComboBox.ItemsSource = _folderSelectionOptions;
            FolderSelectionComboBox.SelectedIndex = 0;

            SetupCollectionSources();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Start listening to messages
            ConcurrentQueue<string> log = new();
            CancellationTokenSource logListenerCancelTokenSource = new();
            Task logListenerTask = StartLogging(log, logListenerCancelTokenSource);

            // Start operations
            await PerformWork(log);

            // Stop listening to messages
            logListenerCancelTokenSource.Cancel();
            logListenerCancelTokenSource.Dispose();
            await logListenerTask;

            // Wrap up - clear messages if any (there should be none)
            Thread.Sleep(200);
            WriteRemainingLogs(log);
            WriteLogToUi("Operation complete. Test count: " + _whileLoopCountTest);
        }

        private Task PerformWork(ConcurrentQueue<string> log)
        {
            var trimIsChecked = TrimCheckBox.IsChecked;
            var trimLeft = TrimLeft.Text;
            var trimRight = TrimRight.Text; 

            var convertIsChecked = ConvertCheckBox.IsChecked;

            string[] folders = FolderSelectionService.GetFoldersToProcess(
                FolderPathTextBox.Text, 
                FolderSelectionComboBox.SelectedItem as FolderSelectionOption);

            return Task.Run(() =>
            {
                ScriptService.Trim(folders, trimIsChecked, trimLeft, trimRight, log);
                ScriptService.Convert(folders, convertIsChecked, log);
            });
        }

        private void FolderPathTextBox_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select folder",
                InitialDirectory = "C:\\Users\\jonat\\Downloads",
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Multiselect = false,
            };

            dialog.ShowDialog();
            var result = dialog.FolderNames.FirstOrDefault();
            if (!string.IsNullOrEmpty(result))
            {
                FolderPathTextBox.Text = result;
                OnFolderPathChanged();
            }
        }

        private void FolderPathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            OnFolderPathChanged();
        }

        private void FolderSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFolderPathChanged();
        }

        private void OnFolderPathChanged()
        {
            if (!string.IsNullOrEmpty(FolderPathTextBox.Text))
            {
                TryPopulateCollections(FolderPathTextBox.Text);
            }
        }

        private void Trim_PreviewTextInput_ParseNumber(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]") && e.Text.Length <= 2; 
        }

        private void TextBox_GotFocus_SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox!.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }
    }
}