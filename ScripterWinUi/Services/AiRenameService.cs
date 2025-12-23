using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScripterWinUi.Services;

/// <summary>
/// Service for executing AI-suggested file renames using staging to avoid conflicts
/// </summary>
public static class AiRenameService
{
    /// <summary>
    /// Executes the rename operations from the AI suggestion
    /// </summary>
    public static async Task ExecuteRenamesAsync(
        string folderPath,
        List<RenameMapping> renames,
        IProgress<string> progressReporter,
        CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            try
            {
                progressReporter.Report($"Starting AI rename operation for {renames.Count} files...");
                progressReporter.Report($"Target folder: {folderPath}");
                progressReporter.Report("");

                // Build list of rename operations with full paths
                var operations = renames
                    .Select(r => new RenameUnitOfWork(
                        new FileInfo(Path.Combine(folderPath, r.OldName)),
                        r.NewName))
                    .ToArray();

                // Validate all source files exist
                var missingFiles = operations.Where(op => !op.SourceFile.Exists).ToArray();
                if (missingFiles.Length > 0)
                {
                    foreach (var missing in missingFiles.Take(5))
                    {
                        progressReporter.Report($"Warning: File not found - {missing.SourceFile.Name}");
                    }
                    if (missingFiles.Length > 5)
                    {
                        progressReporter.Report($"... and {missingFiles.Length - 5} more missing files");
                    }
                    
                    // Remove missing files from operations
                    operations = operations.Where(op => op.SourceFile.Exists).ToArray();
                    progressReporter.Report($"Proceeding with {operations.Length} files that exist.");
                    progressReporter.Report("");
                }

                if (operations.Length == 0)
                {
                    progressReporter.Report("No files to rename.");
                    return;
                }

                // Phase 1: Move all files to staging (to avoid conflicts like 1.jpg -> 2.jpg when 2.jpg exists)
                progressReporter.Report("Phase 1: Moving files to staging...");
                cancellationToken.ThrowIfCancellationRequested();
                Parallel.ForEach(operations, op => op.MoveToStaging());
                progressReporter.Report($"Moved {operations.Length} files to staging.");

                // Phase 2: Move all files from staging to destination
                progressReporter.Report("Phase 2: Moving files to final names...");
                cancellationToken.ThrowIfCancellationRequested();
                Parallel.ForEach(operations, op => op.MoveToDestination());

                progressReporter.Report("");
                progressReporter.Report($"Successfully renamed {operations.Length} files.");
            }
            catch (OperationCanceledException)
            {
                progressReporter.Report("Rename operation cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                progressReporter.Report($"Error during rename: {ex.Message}");
                throw;
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Represents a single file rename operation with staging
    /// </summary>
    private class RenameUnitOfWork
    {
        public FileInfo SourceFile { get; private set; }
        public string DestinationPath { get; private set; }
        public string StagingPath { get; private set; }

        public RenameUnitOfWork(FileInfo sourceFile, string newFileName)
        {
            SourceFile = sourceFile;
            DestinationPath = Path.Combine(sourceFile.DirectoryName!, newFileName);
            StagingPath = Path.Combine(sourceFile.DirectoryName!, Path.GetRandomFileName() + sourceFile.Extension);
        }

        public void MoveToStaging()
        {
            SourceFile.MoveTo(StagingPath);
        }

        public void MoveToDestination()
        {
            SourceFile.MoveTo(DestinationPath);
        }
    }
}
