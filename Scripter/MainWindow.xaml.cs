using Scripts;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            FolderSelectionComboBox.ItemsSource = new FolderSelectionOption[]
            {
                new FolderSelectionOption { Enum = FolderSelectionEnum.Folder, Text = "Selected folder"},
                new FolderSelectionOption { Enum = FolderSelectionEnum.SubFolders, Text = "Subfolders of selected folder"},
            };
            FolderSelectionComboBox.SelectedIndex = 0;
        }

        private void FolderSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //debug
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var log = new ConcurrentQueue<string>();

            var path = PathTextBox.Text;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string[] folders = GetFoldersToProcess(path);

            var cancelTokenSource = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                WriteLogsToUiPeriodically(log, cancelTokenSource.Token);
            }, cancelTokenSource.Token);

            Trim(folders, log);
            cancelTokenSource.Cancel();
            task.Wait();
            cancelTokenSource.Dispose();

            Thread.Sleep(300);
            WriteRemainingLogs(log);
            WriteLogToUi("Operation complete. Test count: " + _whileLoopCountTest);
        }

        private int _whileLoopCountTest = 0;
        private void WriteLogsToUiPeriodically(ConcurrentQueue<string> log, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _whileLoopCountTest++;
                TryWriteLogToUI(log);
                Thread.Sleep(50);
            }
        }

        private void TryWriteLogToUI(ConcurrentQueue<string> log)
        {
            if (log.TryDequeue(out var message))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    WriteLogToUi(message);
                }
            }
        }

        private void WriteRemainingLogs(ConcurrentQueue<string> log)
        {
            while (!log.IsEmpty)
            {
                TryWriteLogToUI(log);
            }
        }

        private void WriteLogToUi(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Output.AppendText(message);
                Output.AppendText(Environment.NewLine);
            }));
        }

        private void Trim(string[] folders, ConcurrentQueue<string> log)
        {
            if (TrimCheckBox.IsChecked.GetValueOrDefault())
            {
                var trimLeft = Convert.ToInt32(TrimLeft.Text);
                var trimRight = Convert.ToInt32(TrimRight.Text);
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight, log);
                }
            }
        }

        private string[] GetFoldersToProcess(string path)
        {
            var folderSelection = FolderSelectionComboBox.SelectedItem as FolderSelectionOption;
            return GetFoldersToProcess(path, folderSelection);
        }

        private static string[] GetFoldersToProcess(string path, FolderSelectionOption? folderSelection)
        {
            if (folderSelection == null)
            {
                return Array.Empty<string>();
            }
            else if (folderSelection.Enum == FolderSelectionEnum.Folder)
            {
                return [path];
            }
            else
            {
                return Directory.GetDirectories(path).ToArray();
            }
        }

        private void Trim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]");
        }
    }

    public class FolderSelectionOption
    {
        public FolderSelectionEnum Enum { get; set; }
        public string Text { get; set; }
    }

    public enum FolderSelectionEnum
    {
        Folder,
        SubFolders
    }
}