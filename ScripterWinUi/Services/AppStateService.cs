using ScripterWinUi.Models;
using ScripterWinUi.Models.Ui;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace ScripterWinUi.Services;

/// <summary>
/// Shared application state service for communicating between pages
/// </summary>
public class AppStateService
{
    private static AppStateService? _instance;
    public static AppStateService Instance => _instance ??= new AppStateService();

    private AppStateService() { }

    // Folder selection data
    public string SelectedFolderPath { get; set; } = string.Empty;
    public FolderSelectionOption? SelectedFolderOption { get; set; }
    public ObservableCollection<FileSelection> SelectedFiles { get; } = new();
    public ObservableCollection<FolderSelection> SelectedFolders { get; } = new();

    // Processing options
    public bool IsTrimEnabled { get; set; }
    public int TrimLeft { get; set; }
    public int TrimRight { get; set; }
    
    public bool IsNormalizeEnabled { get; set; }
    
    public bool IsReseedEnabled { get; set; }
    public int ReseedStartValue { get; set; }
    public ReseedOrderSelectionEnum ReseedOrder { get; set; } = ReseedOrderSelectionEnum.FileName;
    
    public bool IsConvertEnabled { get; set; }

    /// <summary>
    /// Get a summary of selected operations for display
    /// </summary>
    public string GetOperationSummary()
    {
        var operations = new List<string>();
        
        if (IsTrimEnabled)
            operations.Add($"Trim (Left: {TrimLeft}, Right: {TrimRight})");
            
        if (IsNormalizeEnabled)
            operations.Add("Normalize");
            
        if (IsReseedEnabled)
            operations.Add($"Reseed (Start: {ReseedStartValue}, Order: {ReseedOrder})");
            
        if (IsConvertEnabled)
            operations.Add("Convert WebP");

        if (!operations.Any())
            return "No operations selected";

        return $"Operations: {string.Join(", ", operations)} | Target: {SelectedFolderPath}";
    }

    /// <summary>
    /// Check if we have enough configuration to start operations
    /// </summary>
    public bool CanStartOperations()
    {
        return !string.IsNullOrEmpty(SelectedFolderPath) && 
               (IsTrimEnabled || IsNormalizeEnabled || IsReseedEnabled || IsConvertEnabled);
    }
}