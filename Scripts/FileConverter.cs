using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public static class FileConverter
    {
        public static void ConvertWebps(string folderPath)
        {
            var cmd1 = "magick mogrify -format jpeg *.webp";
            var cmd2 = "del *.webp";

            try
            {
                if (NeedsConverting(folderPath))
                {
                    Console.WriteLine("Converting: " + folderPath);
                    ExecuteCommand(cmd1, folderPath);
                    ExecuteCommand(cmd2, folderPath);
                }
                else
                {
                    Console.WriteLine("Skipping: " + folderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + folderPath + " " + ex.Message);
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

            using var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }

        private static bool NeedsConverting(string path)
        {
            var files = Directory.GetFiles(path);
            var needs = files.Any(f => Path.GetExtension(f).Contains("webp"));
            return needs;
        }
    }
}
