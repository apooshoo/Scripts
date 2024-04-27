using Common;

namespace Scripts
{
    public class ImageConverter
    {
        public void Convert(string inputFolder, string outputFolder)
        {
            Convert(inputFolder, outputFolder, ImageFormat.WEBP, ImageFormat.JPEG);
        }

        public void Convert(string inputFolder, string outputFolder, ImageFormat formatIn, ImageFormat formatOut)
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
                            var jpeg = Path.ChangeExtension(outputFolder + file.Name, "jpeg");
                            img.Save(jpeg, new Aspose.Imaging.ImageOptions.JpegOptions());
                            break;
                        case ImageFormat.PNG:
                            var png = Path.ChangeExtension(outputFolder + file.Name, "png");
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
