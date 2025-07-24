using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScripterWinUi.Models;
using ScripterWinUi.Models.Ui;
using ScripterWinUi.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace ScripterWinUi.Pages;

public sealed partial class FolderPreviewPage : Page
{
    public ObservableCollection<FileSelection> SelectedFiles { get; } = new();
    public ObservableCollection<FolderSelection> SelectedFolders { get; } = new();
    
    private FolderSelectionOption[] _folderSelectionOptions = UiSelectionOptions.DefaultFolderSelectionOptions;

    public FolderPreviewPage()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Initialize folder selection options
        FolderSelectionComboBox.ItemsSource = _folderSelectionOptions;
        FolderSelectionComboBox.SelectedIndex = 0;

        // Bind preview lists to ObservableCollections
        PreviewFiles.ItemsSource = SelectedFiles;
        PreviewFolders.ItemsSource = SelectedFolders;

        // Initially hide preview sections
        FilesPreviewSection.Visibility = Visibility.Collapsed;
        FoldersPreviewSection.Visibility = Visibility.Collapsed;
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");

            // Get the current window handle for the picker
            var window = MainWindow.Current;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                FolderPathTextBox.Text = folder.Path;
                await OnFolderPathChangedAsync(folder.Path);
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error selecting folder: {ex.Message}";
        }
    }

    private async void FolderPathTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        await OnFolderPathChangedAsync(FolderPathTextBox.Text);
    }

    private async void FolderSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await OnFolderPathChangedAsync(FolderPathTextBox.Text);
    }

    private async Task OnFolderPathChangedAsync(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            ClearPreviews();
            return;
        }

        try
        {
            StatusTextBlock.Text = "Loading preview...";
            
            // Clear previous results
            ClearPreviews();

            var selectedOption = (FolderSelectionOption)FolderSelectionComboBox.SelectedItem;
            if (selectedOption == null) return;

            // WinUI 3 Best Practice: Use Task.Run for file system operations to avoid blocking UI
            await Task.Run(() =>
            {
                try
                {
                    switch (selectedOption.Enum)
                    {
                        case FolderSelectionEnum.Folder:
                            var files = FileSelectionService.GetSelectedFiles(folderPath);
                            
                            // WinUI 3 Best Practice: Update ObservableCollection on UI thread
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                foreach (var file in files)
                                {
                                    SelectedFiles.Add(file);
                                }
                                UpdatePreviewVisibility();
                            });
                            break;

                        case FolderSelectionEnum.SubFolders:
                            var folders = FileSelectionService.GetSelectedFolders(folderPath, selectedOption);
                            
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                foreach (var folder in folders)
                                {
                                    SelectedFolders.Add(folder);
                                }
                                UpdatePreviewVisibility();
                            });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        StatusTextBlock.Text = $"Error loading preview: {ex.Message}";
                    });
                }
            });

            StatusTextBlock.Text = "Preview loaded successfully";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdatePreviewVisibility()
    {
        FilesPreviewSection.Visibility = SelectedFiles.Any() ? Visibility.Visible : Visibility.Collapsed;
        FoldersPreviewSection.Visibility = SelectedFolders.Any() ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ClearPreviews()
    {
        SelectedFiles.Clear();
        SelectedFolders.Clear();
        UpdatePreviewVisibility();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.Current?.GoBack();
    }

    private void Forward_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.Current?.GoForward();
    }
}
