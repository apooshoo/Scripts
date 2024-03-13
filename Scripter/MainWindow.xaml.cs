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
            var log = new ConcurrentQueue<string>();
            var cancelTokenSource = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                WriteLogsToUiPeriodically(log, cancelTokenSource.Token);
            }, cancelTokenSource.Token);

            // Start operations
            Trim(folders, log);

            // Stop listening to messages
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
            task.Wait();

            // Wrap up - clear messages if any (there should be none)
            Thread.Sleep(200);
            WriteRemainingLogs(log);
            WriteLogToUi("Operation complete. Test count: " + _whileLoopCountTest);
        }

        private void Trim(string[] folders, ConcurrentQueue<string> log)
        {
            if (TrimCheckBox.IsChecked.GetValueOrDefault() 
                && int.TryParse(TrimLeft.Text, out var trimLeft) 
                && int.TryParse(TrimRight.Text, out var trimRight)
                && trimLeft + trimRight > 0)
            {
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight, log);
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