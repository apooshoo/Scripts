using System.Collections.Concurrent;
using System.Diagnostics;

namespace Scripts.Unused
{
    internal class PowershellScripter
    {
        private static void ExecuteCommand(string command, string fromFolderPath,
            IProducerConsumerCollection<string> log)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.WorkingDirectory = fromFolderPath;
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
    }
}
