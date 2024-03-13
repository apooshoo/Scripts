using System.Collections.Concurrent;
using System.Diagnostics;

namespace Scripts
{
    public static class FileConverter
    {
        const string cmd1 = "magick mogrify -format jpeg *.webp";
        const string cmd2 = "del *.webp";

        public static void ConvertWebps(string folderPath, IProducerConsumerCollection<string> log)
        {
            try
            {
                if (NeedsConverting(folderPath))
                {
                    log.TryAdd("Converting: " + folderPath);
                    ExecuteCommand(cmd1, folderPath);
                    ExecuteCommand(cmd2, folderPath);
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

        private static void ExecuteCommand(string command, string folderPath)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.WorkingDirectory = folderPath;
            processStartInfo.Arguments = $"-Command \"{command}\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;

            using var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
        }

        private static bool NeedsConverting(string path)
        {
            var files = Directory.GetFiles(path);
            return files.Any(f => Path.GetExtension(f).Contains("webp"));
        }
    }
}
