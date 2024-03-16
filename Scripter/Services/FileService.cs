using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Scripter.Services
{
    public static class FileService
    {
        public static FileInfo[] GetFiles(string folderPath)
            => FileSystem.GetFiles(folderPath).Select(f => FileSystem.GetFileInfo(f)).ToArray();
    }
}
