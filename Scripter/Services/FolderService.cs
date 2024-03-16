using System.IO;

namespace Scripter.Services
{
    public static class FolderService
    {
        public static string[] GetFolderNames(string path) => Directory.GetDirectories(path).ToArray();
    }
}
