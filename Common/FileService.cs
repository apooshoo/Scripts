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
            if (string.IsNullOrEmpty(folder))
            {
                return Array.Empty<FileInfo>();
            }
            else
            {
                return Directory.GetFiles(folder).Select(f => FileSystem.GetFileInfo(f));
            }
        }
    }
}
