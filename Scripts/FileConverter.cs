using System.Collections.Concurrent;
using System.Diagnostics;

namespace Scripts
{
    public static class FileConverter
    {
        const string exePath = @"C:\Users\jonat\Downloads\New folder\ImageMagick-7.1.1-43-portable-Q16-x64\ImageMagick-7.1.1-43-portable-Q16-x64\magick.exe";
        const string exeCmd = "mogrify -format jpeg *.webp";

        public static void ConvertWebps(string folderPath, IProducerConsumerCollection<string> log)
        {
            try
            {
                string cmd1 = $"&'{exePath}' {exeCmd}";
                string cmd2 = "del *.webp";

                if (NeedsConverting(folderPath))
                {
                    log.TryAdd("Converting: " + folderPath);
                    ExecuteCommand(cmd1, folderPath, log);
                    ExecuteCommand(cmd2, folderPath, log);
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

        private static void ExecuteCommand(string command, string folderPath,
            IProducerConsumerCollection<string> log)
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
            string output = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                log.TryAdd("Powershell output: " + output);
            }
        }

        private static bool NeedsConverting(string path)
        {
            var files = Directory.GetFiles(path);
            return files.Any(f => Path.GetExtension(f).Contains("webp"));
        }
    }
}
