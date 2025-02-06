using Scripter.Models;
using Scripter.Models.Ui;
using Scripter.Services;
using System.Windows;
using System.Windows.Controls;

namespace Scripter
{
    public partial class MainWindow
    {
        public FolderSelectionOption[] _folderSelectionOptions { get; private set; } = UiSelectionOptions.DefaultFolderSelectionOptions;
        public ReseedSelectionOption[] _reseedSelectionOptions { get; private set; } = UiSelectionOptions.DefaultReseedSelectionOptions;
        public List<FileSelection> _selectedFiles { get; set; } = new();
        public List<FolderSelection> _selectedFolders { get; set; } = new();

        private void InitialiseFolderSelectionOptions()
        {
            FolderSelectionComboBox.ItemsSource = _folderSelectionOptions;
            FolderSelectionComboBox.SelectedIndex = 0;
        }

        private void InitialiseReseedSelectionOptions()
        {
            ReseedSelectionComboBox.ItemsSource = _reseedSelectionOptions;
            ReseedSelectionComboBox.SelectedIndex = 0;
        }

        private void InitialisePreviewDataSources()
        {
            PreviewFiles.ItemsSource = _selectedFiles;
            PreviewFiles.Visibility = Visibility.Collapsed;

            PreviewFolders.ItemsSource = _selectedFolders;
            PreviewFolders.Visibility = Visibility.Collapsed;
        }

        private void TryPopulatePreviews(string folderPath)
        {
            ClearPreviews();

            TryPopulatePreviews(folderPath, (FolderSelectionOption)FolderSelectionComboBox.SelectedItem);

            OnPreviewChanged(PreviewFiles);
            OnPreviewChanged(PreviewFolders);
        }

        private void TryPopulatePreviews(string folderPath, FolderSelectionOption folderSelectionOption)
        {
            switch (folderSelectionOption.Enum)
            {
                case FolderSelectionEnum.Folder:
                    var files = FileSelectionService.GetSelectedFiles(folderPath);
                    _selectedFiles.AddRange(files);
                    break;
                case FolderSelectionEnum.SubFolders:
                    var folders = FileSelectionService.GetSelectedFolders(folderPath, folderSelectionOption);
                    _selectedFolders.AddRange(folders);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(folderSelectionOption));
            }
        }

        private void OnPreviewChanged(ListView listView)
        {
            listView.Items.Refresh();
            listView.Visibility = listView.HasItems ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearPreviews()
        {
            _selectedFiles.Clear();
            _selectedFolders.Clear();
        }
    }
}
