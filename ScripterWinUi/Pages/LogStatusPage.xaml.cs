using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ScripterWinUi.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ScripterWinUi.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LogStatusPage : Page
{
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly AppStateService _appState = AppStateService.Instance;
    private DateTime _operationStartTime;

    public LogStatusPage()
    {
        InitializeComponent();
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_appState.CanStartOperations())
        {
            AppendLog("Error: Cannot start operations. Please configure options and select a folder.");
            return;
        }

        try
        {
            // Setup for operation
            _cancellationTokenSource = new CancellationTokenSource();
            _operationStartTime = DateTime.Now;
            
            // Update UI state
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            OperationProgressBar.IsIndeterminate = true;
            ProgressTextBlock.Text = "Progress: Starting operations...";
            
            AppendLog("Starting operations...");
            AppendLog($"Target folder: {_appState.SelectedFolderPath}");
            AppendLog($"Operations: {GetSelectedOperationsText()}");
            AppendLog("");

            // Create progress reporter
            var progress = new Progress<string>(message =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    AppendLog(message);
                });
            });

            // Execute operations
            await ScriptOperationService.ExecuteOperationsAsync(
                _appState.SelectedFolderPath,
                _appState.SelectedFolderOption!,
                _appState.IsTrimEnabled, _appState.TrimLeft, _appState.TrimRight,
                _appState.IsNormalizeEnabled,
                _appState.IsReseedEnabled, _appState.ReseedStartValue, _appState.ReseedOrder,
                _appState.IsConvertEnabled,
                progress,
                _cancellationTokenSource.Token);

            var elapsed = DateTime.Now - _operationStartTime;
            AppendLog("");
            AppendLog($"All operations completed successfully in {elapsed.TotalSeconds:F1} seconds.");
            ProgressTextBlock.Text = "Progress: Completed successfully";
            OperationProgressBar.Value = 100;
        }
        catch (OperationCanceledException)
        {
            AppendLog("");
            AppendLog("Operations cancelled by user.");
            ProgressTextBlock.Text = "Progress: Cancelled";
        }
        catch (Exception ex)
        {
            AppendLog("");
            AppendLog($"Error: {ex.Message}");
            ProgressTextBlock.Text = "Progress: Error occurred";
        }
        finally
        {
            StartButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
            OperationProgressBar.IsIndeterminate = false;
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        AppendLog("Cancellation requested...");
        ProgressTextBlock.Text = "Progress: Cancelling...";
        CancelButton.IsEnabled = false;
    }

    private void ClearLogButton_Click(object sender, RoutedEventArgs e)
    {
        LogTextBlock.Text = "Log cleared.\n";
        OperationProgressBar.Value = 0;
        ProgressTextBlock.Text = "Progress: Ready";
    }

    private void AppendLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var logEntry = string.IsNullOrEmpty(message) ? "\n" : $"[{timestamp}] {message}\n";
        
        LogTextBlock.Text += logEntry;
        
        // Auto-scroll to bottom
        LogScrollViewer.ChangeView(null, LogScrollViewer.ScrollableHeight, null);
    }

    private string GetSelectedOperationsText()
    {
        var operations = new List<string>();
        
        if (_appState.IsTrimEnabled)
            operations.Add($"Trim (L:{_appState.TrimLeft}, R:{_appState.TrimRight})");
        if (_appState.IsNormalizeEnabled)
            operations.Add("Normalize");
        if (_appState.IsReseedEnabled)
            operations.Add($"Reseed (Start:{_appState.ReseedStartValue}, Order:{_appState.ReseedOrder})");
        if (_appState.IsConvertEnabled)
            operations.Add("Convert WebP");

        return operations.Any() ? string.Join(", ", operations) : "None";
    }

    private void UpdateOperationSummary()
    {
        OperationSummaryTextBlock.Text = _appState.GetOperationSummary();
    }

    private void UpdateStartButtonState()
    {
        StartButton.IsEnabled = _appState.CanStartOperations();

        if (!StartButton.IsEnabled)
        {
            OperationSummaryTextBlock.Text = "Please configure operations and select a folder first.";
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        UpdateOperationSummary();
        UpdateStartButtonState();
    }
}
