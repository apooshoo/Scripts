using Common;

namespace Scripts
{
    public class FileReseeder
    {
        private class ReseedTask
        {
            public FileInfo File { get; private set; }
            public string Destination { get; private set; }
            public string Temp { get; set; }

            public ReseedTask(FileInfo file, string destination)
            {
                File = file;
                Destination = destination;
            }

            public void MoveToTemp()
            {
                // Move to temp to avoid clashes (eg: 1.jpg -> 2.jpg while 2.jpg exists)
                var temp = Path.Combine(File.DirectoryName, Path.GetRandomFileName() + File.Extension);
                File.MoveTo(temp);
                Temp = temp; // Store for rollback (TBD)
            }

            public void Rename()
            {
                File.MoveTo(Destination);
            }
        }

        public static void ReseedFiles(string folderPath, int initialSeed)
        {
            FileInfo[] orderedFiles = GetOrderedFiles(folderPath);
            var currentSeed = initialSeed - 1;

            var reseedTasks = orderedFiles.Select(f =>
            {
                // Get new file names
                currentSeed++;
                var destination = Path.Combine(f.DirectoryName, currentSeed + f.Extension);
                return new ReseedTask(f, destination);
            }).ToArray();

            foreach (var task in reseedTasks)
            {
                task.MoveToTemp();
            }

            foreach (var task in reseedTasks)
            {
                task.Rename();
            }
        }

        private static FileInfo[] GetOrderedFiles(string folderPath)
        {
            var files = FileService.GetFiles(folderPath);

            if (files.All(f => int.TryParse(Path.GetFileNameWithoutExtension(f.Name), out _)))
            {
                return files.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f.Name))).ToArray();
            }
            else
            {
                return files.OrderBy(f => f.Name).ToArray();
            }
        }
    }
}
