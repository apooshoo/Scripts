using Scripts;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = PathTextBox.Text;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var folderSelection = FolderSelectionComboBox.SelectedItem as FolderSelectionOption;
            List<string> folders = GetFoldersToProcess(path, folderSelection);

            if (TrimCheckBox.IsChecked.GetValueOrDefault())
            {
                var trimLeft = Convert.ToInt32(TrimLeft.Text);
                var trimRight = Convert.ToInt32(TrimRight.Text);
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight);
                }
            }
        }

        private static List<string> GetFoldersToProcess(string path, FolderSelectionOption? folderSelection)
        {
            if (folderSelection == null)
            {
                return new List<string>();
            }
            else if (folderSelection.Enum == FolderSelectionEnum.Folder)
            {
                return new List<string> { path };
            }
            else
            {
                return Directory.GetDirectories(path).ToList();
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