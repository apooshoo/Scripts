using Common;

namespace Scripts
{
    public static class ImageConverter
    {
        public static void Convert(string inputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            Convert(inputFolder, outputFolder: inputFolder, formatIn, formatOut);
        }

        public static void Convert(string inputFolder, string outputFolder, ImageFormat formatIn, ImageFormat formatOut)
        {
            var imageExtensionName = formatIn.ToString().ToLowerInvariant();
            var files = FileService.GetFiles(inputFolder, imageExtensionName);

            foreach (var file in files)
            {
                using (var img = Aspose.Imaging.Image.Load(file.FullName))
                {
                    switch (formatOut)
                    {
                        case ImageFormat.JPEG:
                            var jpeg = Path.ChangeExtension(Path.Combine(outputFolder, file.Name), "jpeg");
                            img.Save(jpeg, new Aspose.Imaging.ImageOptions.JpegOptions());
                            break;
                        case ImageFormat.PNG:
                            var png = Path.ChangeExtension(Path.Combine(outputFolder, file.Name), "png");
                            img.Save(png, new Aspose.Imaging.ImageOptions.PngOptions());
                            break;
                        default:
                            throw new ArgumentException(nameof(formatOut));
                    }
                }
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
