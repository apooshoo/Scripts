using Scripter.Models;
using Scripter.Services;
using System.Windows;
using System.Windows.Controls;

namespace Scripter
{
    public partial class MainWindow
    {
        public List<FileSelection> _selectedFiles { get; set; } = new();
        public List<FolderSelection> _selectedFolders { get; set; } = new();

        private void SetupCollectionSources()
        {
            PreviewFiles.ItemsSource = _selectedFiles;
            PreviewFiles.Visibility = Visibility.Collapsed;

            PreviewFolders.ItemsSource = _selectedFolders;
            PreviewFolders.Visibility = Visibility.Collapsed;
        }

        private void TryPopulateCollections(string folderPath)
        {
            ClearCollections();

            TryPopulateCollections(folderPath, (FolderSelectionOption)FolderSelectionComboBox.SelectedItem);

            OnCollectionChanged(PreviewFiles);
            OnCollectionChanged(PreviewFolders);
        }

        private void TryPopulateCollections(string folderPath, FolderSelectionOption folderSelectionOption)
        {
            switch (folderSelectionOption.Enum)
            {
                case FolderSelectionEnum.Folder:
                    var files = FileService.GetFiles(folderPath)
                        .Select(x => new FileSelection(x));
                    _selectedFiles.AddRange(files);
                    break;
                case FolderSelectionEnum.SubFolders:
                    var folders = FolderSelectionService.GetFoldersToProcess(folderPath, folderSelectionOption)
                        .Select(x => new FolderSelection(x));
                    _selectedFolders.AddRange(folders);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(folderSelectionOption));
            }
        }

        private void OnCollectionChanged(ListView listView)
        {
            listView.Items.Refresh();
            listView.Visibility = listView.HasItems ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearCollections()
        {
            _selectedFiles.Clear();
            _selectedFolders.Clear();
        }
    }
}
