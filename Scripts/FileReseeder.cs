using Common;
using Scripts.Models;

namespace Scripts
{
    public class FileReseeder
    {
        public static void ReseedFiles(FileInfo[] files, int initialSeed)
        {
            var i = initialSeed - 1;

            var unitsOfWork = files.Select(f =>
            {
                i++;
                return new ReseedUnitOfWork(f, i);
            }).ToArray();

            Parallel.ForEach(unitsOfWork, task =>
            {
                task.MoveToStaging();
            });

            Parallel.ForEach(unitsOfWork, task =>
            {
                task.MoveToDestination();
            });
        }

        public static FileInfo[] ReorderFilesByFileName(string folderPath)
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

        public static FileInfo[] ReorderFilesByModifiedDate(string folderPath)
        {
            var files = FileService.GetFiles(folderPath);
            return files.OrderBy(f => f.CreationTime).ToArray();
        }
    }
}
