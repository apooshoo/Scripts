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
        public MainWindow()
        {
            InitializeComponent();
            Initialise();
            DataContext = this;
        }

        private void Initialise()
        {
            InitialisePreviewOptions();
            InitialisePreviewSources();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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
            WriteLogToUi("Operation complete. Test count: " + _whileLoopCountTest);
        }

        private Task PerformWork(ConcurrentQueue<string> log)
        {
            var shouldTrim = TrimCheckBox.IsChecked.GetValueOrDefault();
            var trimLeft = TrimLeft.Text;
            var trimRight = TrimRight.Text; 

            var shouldNormalise = NormaliseCheckBox.IsChecked.GetValueOrDefault();

            var shouldConvert = ConvertCheckBox.IsChecked.GetValueOrDefault();

            var folders = FolderSelectionService.GetFoldersToProcess(FolderPathTextBox.Text, 
                (FolderSelectionOption)FolderSelectionComboBox.SelectedItem);

            return Task.Run(() =>
            {
                if (shouldTrim) ScriptService.Trim(folders, trimLeft, trimRight, log);
                if (shouldNormalise) ScriptService.Normalise(folders);
                if (shouldConvert) ScriptService.Convert(folders, log);
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
                TryPopulatePreviews(FolderPathTextBox.Text);
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