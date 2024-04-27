using Common;

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
            var tasks = GetConvertTasks(inputFolder, outputFolder, formatIn, formatOut);

            foreach (var task in tasks)
            {
                task.Invoke();
            }
        }

        public static IEnumerable<Action> GetConvertTasks(string inputFolder, string outputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            var imageExtensionName = formatIn.ToString().ToLowerInvariant();
            var files = FileService.GetFiles(inputFolder, imageExtensionName);

            foreach (var file in files)
            {
                yield return () => Convert(file, outputFolder, formatOut);
            }
        }

        private static void Convert(FileInfo file, string outputFolder, ImageFormat formatOut)
        {
            using (var img = Aspose.Imaging.Image.Load(file.FullName))
            {
                var newFullName = Path.Combine(outputFolder, file.Name);
                newFullName = Path.ChangeExtension(newFullName, formatOut.ToString().ToLower());
                Convert(img, newFullName, formatOut);
            }
            file.Delete();
        }

        private static void Convert(Aspose.Imaging.Image img, string fullName, ImageFormat formatOut)
        {
            switch (formatOut)
            {
                case ImageFormat.JPEG:
                    img.Save(fullName, new Aspose.Imaging.ImageOptions.JpegOptions());
                    break;
                case ImageFormat.PNG:
                    img.Save(fullName, new Aspose.Imaging.ImageOptions.PngOptions());
                    break;
                default:
                    throw new ArgumentException(nameof(formatOut));
            }
        }
    }

    public enum ImageFormat
    {
        WEBP,
        JPEG, 
        PNG
    }
}
