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
        public static void ConvertWebps(string path)
        {
            var cmd1 = "magick mogrify -format jpeg *.webp";
            var cmd2 = "del *.webp";

            try
            {
                if (NeedsConverting(path))
                {
                    Console.WriteLine("Converting: " + path);
                    ExecuteCommand(cmd1, path);
                    ExecuteCommand(cmd2, path);
                }
                else
                {
                    Console.WriteLine("Skipping: " + path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + path + " " + ex.Message);
            }
        }

        private static void ExecuteCommand(string command, string workingDirectory)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.WorkingDirectory = workingDirectory;
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
