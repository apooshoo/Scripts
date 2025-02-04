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
        public MainWindow()
        {
            InitializeComponent();
            InitialiseFolderSelectionOptions();
            InitialisePreviewDataSources();
            DataContext = this;
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            WriteLogToUi("Starting...");

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
            OnFolderPathChanged();
        }

        private Task PerformWork(ConcurrentQueue<string> log)
        {
            var shouldTrim = TrimCheckBox.IsChecked.GetValueOrDefault();
            var trimLeft = TrimLeft.Text;
            var trimRight = TrimRight.Text;

            var shouldNormalise = NormaliseCheckBox.IsChecked.GetValueOrDefault();

            var shouldReseed = ReseedCheckBox.IsChecked.GetValueOrDefault();
            var reseedValue = ReseedValue.Text;

            var shouldConvert = ConvertCheckBox.IsChecked.GetValueOrDefault();

            var folders = FileSelectionService.GetSelectedFolders(FolderPathTextBox.Text,
                (FolderSelectionOption)FolderSelectionComboBox.SelectedItem);

            return Task.Run(() =>
            {
                if (shouldTrim) ScriptService.Trim(folders, trimLeft, trimRight, log);
                if (shouldNormalise) ScriptService.Normalise(folders);
                if (shouldReseed) ScriptService.Reseed(folders, reseedValue);
                if (shouldConvert) ScriptService.Convert(folders, log);
            });
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

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }
    }
}