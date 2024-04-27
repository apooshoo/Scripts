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
                    if (formatOut == ImageFormat.JPEG)
                    {
                        var output = Path.ChangeExtension(outputFolder + file.Name, "jpeg");
                        img.Save(output, new Aspose.Imaging.ImageOptions.JpegOptions());
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }
    }

    public enum ImageFormat
    {
        WEBP,
        JPEG
    }
}
