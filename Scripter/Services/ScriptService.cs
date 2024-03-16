using Scripts;
using System.Collections.Concurrent;

namespace Scripter.Services
{
    public static class ScriptService
    {
        public static void Trim(string[] folders, bool? isChecked, string trimLeftStr, string trimRightStr,
            ConcurrentQueue<string> log)
        {
            if (isChecked.GetValueOrDefault()
                && int.TryParse(trimLeftStr, out var trimLeft)
                && int.TryParse(trimRightStr, out var trimRight)
                && trimLeft + trimRight > 0)
            {
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder, trimLeft, trimRight, log);
                }
            }
        }

        public static void Convert(string[] folders, bool? isChecked, ConcurrentQueue<string> log)
        {
            if (isChecked.GetValueOrDefault())
            {
                foreach (var folder in folders)
                {
                    FileConverter.ConvertWebps(folder, log);
                }
            }
        }
    }
}
