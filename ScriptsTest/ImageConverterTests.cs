using Common;
using Scripts;

namespace ScriptsTest
{
    [TestClass]
    public class ImageConverterTests : TestBase
    {
        const string WebpTestFolder = "WebpTestFiles";

        [TestInitialize]
        public void Init()
        {
            CleanUpFolder(TestOutputFolder);
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanUpFolder(TestOutputFolder);
        }

        [TestMethod]
        public void ConvertTest()
        {
            var webpTestFolder = Path.Combine(TestBaseFolder, WebpTestFolder);

            CopyFolderContents(webpTestFolder,  TestOutputFolder);

            ImageConverter.Convert(TestOutputFolder, TestOutputFolder, ImageFormat.WEBP, ImageFormat.JPEG);

            var input = FileService.GetFiles(webpTestFolder);
            var output = FileService.GetFiles(TestOutputFolder);
            Assert.AreEqual(input.Count(), output.Count());
            Assert.IsTrue(output.All(x => x.Extension.Equals(".jpeg")));
        }
    }
}