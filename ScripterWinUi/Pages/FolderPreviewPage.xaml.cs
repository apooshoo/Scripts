using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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
    private readonly AppStateService _appState = AppStateService.Instance;

    public FolderPreviewPage()
    {
        InitializeComponent();
        InitializeControls();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        LoadAppState();
    }
    
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        SaveAppState();
        base.OnNavigatedFrom(e);
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
        ClearPreviews();

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

    private void SaveAppState()
    {
        _appState.SelectedFolderPath = FolderPathTextBox.Text;
        _appState.SelectedFolderOption = (FolderSelectionOption?)FolderSelectionComboBox.SelectedItem;
        
        _appState.SelectedFiles.Clear();
        _appState.SelectedFolders.Clear();
        foreach (var file in SelectedFiles)
        {
            _appState.SelectedFiles.Add(file);
        }
        foreach (var folder in SelectedFolders)
        {
            _appState.SelectedFolders.Add(folder);
        }
    }

    private void LoadAppState()
    {
        FolderPathTextBox.Text = _appState.SelectedFolderPath;
        FolderSelectionComboBox.SelectedItem = _appState.SelectedFolderOption ?? _folderSelectionOptions.First();
        
        SelectedFiles.Clear();
        SelectedFolders.Clear();
        foreach (var file in _appState.SelectedFiles)
        {
            SelectedFiles.Add(file);
        }
        foreach (var folder in _appState.SelectedFolders)
        {
            SelectedFolders.Add(folder);
        }
        UpdatePreviewVisibility();
    }
}
