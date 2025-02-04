using Microsoft.VisualBasic.FileIO;
using Scripter.Models;
using System.IO;

namespace Scripter.Services
{
    public static class FileService
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
                    ? GetSubFolderNames(path)
                    : [path];
            }
        }

        private static string[] GetSubFolderNames(string path)
        {
            try
            {
                return !string.IsNullOrEmpty(path)
                    ? Directory.GetDirectories(path)
                    : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public static FileSelection[] GetSelectedFiles(string path)
        {
            return GetFileNames(path)
                .Select(f => FileSystem.GetFileInfo(f))
                .Select(f => new FileSelection(f)).ToArray();
        }

        private static IReadOnlyCollection<string> GetFileNames(string path)
        {
            try
            {
                return !string.IsNullOrEmpty(path)
                    ? FileSystem.GetFiles(path)
                    : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}
