using Scripts;
using System.Collections.Concurrent;

namespace Scripter.Services
{
    public static class ScriptService
    {
        public static void Trim(string[] folders, string trimLeftStr, string trimRightStr,
            ConcurrentQueue<string> log)
        {
            if (int.TryParse(trimLeftStr, out var trimLeft)
                && int.TryParse(trimRightStr, out var trimRight)
                && trimLeft + trimRight > 0)
            {
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight, log);
                }
            }
        }

        public static void Normalise(string[] folders)
        {
            foreach (var folder in folders)
            {
                FileRenamer.RemoveNonNumbers(folder);
                FileRenamer.Fill(folder);
            }
        }

        public static void Convert(string[] folders, ConcurrentQueue<string> log)
        {
            foreach (var folder in folders)
            {
                FileConverter.ConvertWebps(folder, log);
            }
        }
    }
}
