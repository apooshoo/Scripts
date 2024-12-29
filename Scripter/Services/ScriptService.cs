using Common;
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

        public static void Convert(string[] folders, IProducerConsumerCollection<string> log)
        {
            foreach (var folder in folders)
            {
                try
                {
                    log.TryAdd("Converting: " + folder);
                    ImageConverter.Convert(folder, ImageFormat.WEBP, ImageFormat.JPEG);
                    log.TryAdd("Done converting.");
                }
                catch (Exception e)
                {
                    log.TryAdd($"Error while converting for: {folder} : {e.Message}");
                }
            }
        }
    }
}
