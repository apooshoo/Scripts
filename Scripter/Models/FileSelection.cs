using System.IO;

namespace Scripter.Models
{
    public class FileSelection
    {
        public string Name { get; set; }
        public string Extension { get; set; }

        public FileSelection(FileInfo file)
        {
            Name = file.Name;
            Extension = file.Extension;
        }
    }
}
