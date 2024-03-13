using Scripter.Models;
using Scripter.Services;
using Scripts;
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
        public MainWindow()
        {
            InitializeComponent();
            Setup();
            DataContext = this;
            WriteLogToUi("Initialised.");
        }

        private void Setup()
        {
            FolderSelectionComboBox.ItemsSource = FolderSelectionService.GetDefaultOptions();
            FolderSelectionComboBox.SelectedIndex = 0;
        }

        private void FolderSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //debug
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = PathTextBox.Text;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string[] folders = FolderSelectionService.GetFoldersToProcess(
                path, FolderSelectionComboBox.SelectedItem as FolderSelectionOption);

            // Start listening to messages
            ConcurrentQueue<string> log = new();
            CancellationTokenSource logListenerCancelTokenSource = new();
            Task logListenerTask = StartLogging(log, logListenerCancelTokenSource);

            // Start operations
            await PerformWork(folders, log);

            // Stop listening to messages
            logListenerCancelTokenSource.Cancel();
            logListenerCancelTokenSource.Dispose();
            await logListenerTask;

            // Wrap up - clear messages if any (there should be none)
            Thread.Sleep(200);
            WriteRemainingLogs(log);
            WriteLogToUi("Operation complete. Test count: " + _whileLoopCountTest);
        }

        private Task PerformWork(string[] folders, ConcurrentQueue<string> log)
        {
            var trimIsChecked = TrimCheckBox.IsChecked;
            var trimLeft = TrimLeft.Text;
            var trimRight = TrimRight.Text; 

            var convertIsChecked = ConvertCheckBox.IsChecked;

            return Task.Run(() =>
            {
                Trim(folders, trimIsChecked, trimLeft, trimRight, log);
                Convert(folders, convertIsChecked, log);
            });
        }

        private void Trim(string[] folders, bool? isChecked, string trimLeftStr, string trimRightStr, 
            ConcurrentQueue<string> log)
        {
            if (isChecked.GetValueOrDefault() 
                && int.TryParse(trimLeftStr, out var trimLeft) 
                && int.TryParse(trimRightStr, out var trimRight)
                && trimLeft + trimRight > 0)
            {
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight, log);
                }
            }
        }

        private void Convert(string[] folders, bool? isChecked, ConcurrentQueue<string> log)
        {
            if (isChecked.GetValueOrDefault())
            {
                foreach (var folder in folders)
                {
                    FileConverter.ConvertWebps(folder, log);
                }
            }
        }

        private void Trim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]") && e.Text.Length <= 2; 
        }

        private void Trim_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }
    }
}