using Scripter.Models;

namespace Scripter.Services
{
    public static class FolderSelectionService
    {
        public static readonly FolderSelectionOption[] DefaultOptions = 
        {
            new FolderSelectionOption(FolderSelectionEnum.Folder, "Selected folder"),
            new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders of selected folder")
        };

        public static string[] GetFoldersToProcess(string path, FolderSelectionOption? folderSelection)
        {
            if (string.IsNullOrEmpty(path) || folderSelection == null)
            {
                return Array.Empty<string>();
            }
            else if (folderSelection.Enum == FolderSelectionEnum.Folder)
            {
                return [path];
            }
            else
            {
                return FolderService.GetFolderNames(path);
            }
        }
    }
}
