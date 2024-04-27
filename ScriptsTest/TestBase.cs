using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScriptsTest
{
    public abstract class TestBase
    {
        public string TestBaseFolder
        {
            get
            {
                return Path.Combine(GetRootDir(), "TestFolder");
            }
        }

        public string TestOutputFolder
        {
            get
            {
                var outputFolder = Path.Combine(TestBaseFolder, "Output");
                if (Directory.Exists(outputFolder))
                {
                    return outputFolder;
                }
                else
                {
                    Directory.CreateDirectory(outputFolder);
                    return outputFolder;
                }
            }
        }

        public string GetTestFolder(string folderName) => Path.Combine(TestBaseFolder, folderName);

        private string GetRootDir() => 
            new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName ?? throw new Exception();

        public static void CleanUpFolder(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void CopyFolderContents(string from, string to)
        {
            var dir = new DirectoryInfo(from);
            foreach (var file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(to, file.Name));
            }
        }
    }
}
