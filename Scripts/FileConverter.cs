using ImageMagick;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Concurrent;

namespace Scripts
{
    public static class FileConverter
    {
        public static void ConvertWebps(string folderPath, IProducerConsumerCollection<string> log)
        {
            try
            {
                if (NeedsConverting(folderPath))
                {
                    var files = FileSystem.GetFiles(folderPath).Select(f => FileSystem.GetFileInfo(f));
                    var queuedFiles = new Queue<FileInfo>(files);

                    log.TryAdd("Converting: " + folderPath);
                    ConvertFiles(queuedFiles);

                    log.TryAdd("Done converting. Deleting old files...");
                    DeleteFiles(queuedFiles);

                    log.TryAdd("Done.");
                }
                else
                {
                    log.TryAdd("Skipping: " + folderPath);
                }
            }
            catch (Exception e)
            {
                log.TryAdd($"Error while converting for: {folderPath} : {e.Message}");
            }
        }

        private static bool NeedsConverting(string path)
        {
            var files = Directory.GetFiles(path);
            return files.Any(f => Path.GetExtension(f).Contains("webp"));
        }

        private static void ConvertFiles(Queue<FileInfo> queuedFiles)
        {
            foreach (var file in queuedFiles)
            {
                using var image = new MagickImage(file);
                image.Write(file.FullName.Replace(".webp", ".jpeg"));
            }
        }

        private static void DeleteFiles(Queue<FileInfo> queuedFiles)
        {
            while (queuedFiles.TryDequeue(out var file))
            {
                file.Delete();
            }
        }
    }
}
