using Microsoft.VisualBasic.FileIO;

namespace Common
{
    public static class FileService
    {
        public static IEnumerable<FileInfo> GetFiles(string folder, string extensionName)
        {
            var extension = "." + extensionName;
            return GetFiles(folder).Where(x => x.Extension.Equals(extension));
        }

        public static IEnumerable<FileInfo> GetFiles(string folder)
        {
            return GetFileNames(folder).Select(f => FileSystem.GetFileInfo(f));
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
        public static string[] GetSubFolderNames(string path)
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
    }
}
