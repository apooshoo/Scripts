using Common;
using Scripter.Models;
using Scripter.Models.Ui;

namespace Scripter.Services
{
    public static class FileSelectionService
    {
        public static FolderSelection[] GetSelectedFolders(string path, FolderSelectionOption? folderSelection)
             => GetFolderNames(path, folderSelection)
                    .Select(f => new FolderSelection(f))
                    .ToArray();

        private static string[] GetFolderNames(string path, FolderSelectionOption? folderSelection)
        {
            if (folderSelection == null)
            {
                return Array.Empty<string>();
            }
            else
            {
                return folderSelection.Enum == FolderSelectionEnum.SubFolders
                    ? FileService.GetSubFolderNames(path)
                    : [path];
            }
        }

        public static FileSelection[] GetSelectedFiles(string path)
        {
            return FileService.GetFiles(path).Select(f => new FileSelection(f)).ToArray();
        }
    }
}
