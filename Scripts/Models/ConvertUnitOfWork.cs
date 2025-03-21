using ImageMagick;

namespace Scripts.Models
{
    public class ConvertUnitOfWork
    {
        public FileInfo File { get; private set; }
        public string InputFolder { get; private set; }
        public string OutputFolder { get; private set; }
        public string ExtensionIn { get; private set; }
        public string ExtensionOut { get; private set; }

        public ConvertUnitOfWork(FileInfo file, string inputFolder, string outputFolder, string extensionIn, string extensionOut)
        {
            File = file;
            InputFolder = inputFolder;
            OutputFolder = outputFolder;
            ExtensionIn = extensionIn;
            ExtensionOut = extensionOut;
        }

        public void Convert()
        {
            using var image = new MagickImage(File);
            image.Write(File.FullName.Replace(ExtensionIn, ExtensionOut));
            File.Delete();
        }
    }
}
