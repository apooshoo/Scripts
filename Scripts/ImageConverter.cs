using Common;
using ImageMagick;

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
            var tasks = CreateConvertTasks(inputFolder, outputFolder, formatIn, formatOut);

            foreach (var task in tasks)
            {
                task.Invoke();
            }
        }

        public static IEnumerable<Action> CreateConvertTasks(string inputFolder, string outputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            var extensionIn = formatIn.ToString().ToLowerInvariant();
            var extensionOut = formatOut.ToString().ToLowerInvariant();
            var files = FileService.GetFiles(inputFolder, extensionIn);

            foreach (var file in files)
            {
                yield return () => Convert(file, outputFolder, extensionIn, extensionOut);
            }
        }

        private static void Convert(FileInfo file, string outputFolder, string extensionIn, string extensionOut)
        {
            using var image = new MagickImage(file);
            image.Write(file.FullName.Replace(extensionIn, extensionOut));
            file.Delete();
        }
    }
}
