using Common;
using Scripts.Models;

namespace Scripts
{
    public static class ImageConverter
    {
        public static void Convert(string inputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            Convert(inputFolder, inputFolder, formatIn, formatOut);
        }

        public static void Convert(string inputFolder, string outputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            var extensionIn = formatIn.ToString().ToLowerInvariant();
            var extensionOut = formatOut.ToString().ToLowerInvariant();
            var files = FileService.GetFiles(inputFolder, extensionIn);

            var unitsOfWork = files.Select(f => new ConvertUnitOfWork(f, inputFolder, outputFolder, extensionIn, extensionOut)).ToArray();

            Parallel.ForEach(unitsOfWork, task =>
            {
                task.Convert();
            });
        }
    }
}
