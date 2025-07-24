using ScripterWinUi.Models;
using ScripterWinUi.Models.Ui;
using Scripts;
using System.Collections.Concurrent;
using Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScripterWinUi.Services;

/// <summary>
/// Service that adapts the Scripts project functionality for WinUI
/// </summary>
public static class ScriptOperationService
{
    /// <summary>
    /// Execute all configured operations asynchronously
    /// </summary>
    public static async Task ExecuteOperationsAsync(
        string folderPath,
        FolderSelectionOption folderOption,
        bool shouldTrim, int trimLeft, int trimRight,
        bool shouldNormalize,
        bool shouldReseed, int reseedValue, ReseedOrderSelectionEnum reseedOrder,
        bool shouldConvert,
        IProgress<string> progressReporter,
        CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            try
            {
                var folders = GetTargetFolders(folderPath, folderOption);
                
                if (folders.Length == 0)
                {
                    progressReporter.Report("No folders found to process.");
                    return;
                }

                progressReporter.Report($"Found {folders.Length} folder(s) to process.");

                ConcurrentQueue<string> log = new();

                // Execute operations in sequence
                if (shouldTrim && trimLeft + trimRight > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progressReporter.Report("Starting trim operation...");
                    
                    foreach (var folder in folders)
                    {
                        FileRenamer.KeepFirstXAndLastYCharacters(folder.Name, trimLeft, trimRight, log);
                        DrainLogQueue(log, progressReporter);
                    }
                    
                    progressReporter.Report("Trim operation completed.");
                }

                if (shouldNormalize)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progressReporter.Report("Starting normalize operation...");
                    
                    foreach (var folder in folders)
                    {
                        FileRenamer.RemoveNonNumbers(folder.Name);
                        FileRenamer.Fill(folder.Name);
                    }
                    
                    progressReporter.Report("Normalize operation completed.");
                }

                if (shouldReseed && reseedValue >= 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progressReporter.Report("Starting reseed operation...");
                    
                    foreach (var folder in folders)
                    {
                        switch (reseedOrder)
                        {
                            case ReseedOrderSelectionEnum.FileName:
                                FileRenamer.ReseedFiles(folder.Name, reseedValue);
                                break;
                            case ReseedOrderSelectionEnum.CreationDate:
                                FileRenamer.ReseedFilesByCreationDate(folder.Name, reseedValue);
                                break;
                        }
                    }
                    
                    progressReporter.Report("Reseed operation completed.");
                }

                if (shouldConvert)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progressReporter.Report("Starting convert operation...");
                    
                    foreach (var folder in folders)
                    {
                        try
                        {
                            progressReporter.Report($"Converting: {folder.Name}");
                            ImageConverter.Convert(folder.Name, ImageFormat.WEBP, ImageFormat.JPEG);
                            progressReporter.Report($"Converted: {folder.Name}");
                        }
                        catch (Exception e)
                        {
                            progressReporter.Report($"Error converting {folder.Name}: {e.Message}");
                        }
                    }
                    
                    progressReporter.Report("Convert operation completed.");
                }

                // Final log drain
                DrainLogQueue(log, progressReporter);
                
                progressReporter.Report("All operations completed successfully.");
            }
            catch (OperationCanceledException)
            {
                progressReporter.Report("Operations cancelled by user.");
                throw;
            }
            catch (Exception ex)
            {
                progressReporter.Report($"Error during operations: {ex.Message}");
                throw;
            }
        }, cancellationToken);
    }

    private static FolderSelection[] GetTargetFolders(string folderPath, FolderSelectionOption folderOption)
    {
        return folderOption.Enum switch
        {
            FolderSelectionEnum.Folder => [new FolderSelection(folderPath)],
            FolderSelectionEnum.SubFolders => FileSelectionService.GetSelectedFolders(folderPath, folderOption),
            _ => throw new ArgumentOutOfRangeException(nameof(folderOption))
        };
    }

    private static void DrainLogQueue(ConcurrentQueue<string> log, IProgress<string> progressReporter)
    {
        while (log.TryDequeue(out var message))
        {
            if (!string.IsNullOrEmpty(message))
            {
                progressReporter.Report(message);
            }
        }
    }
}