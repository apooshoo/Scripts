using Common;
using Scripts;
using System.Reflection;

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

            ImageConverter.Convert(webpTestFolder, TestOutputFolder, ImageFormat.WEBP, ImageFormat.JPEG);

            var input = FileService.GetFiles(webpTestFolder);
            var output = FileService.GetFiles(TestOutputFolder);
            Assert.AreEqual(input.Count(), output.Count());
            Assert.IsTrue(output.All(x => x.Extension.Equals(".jpeg")));
        }

        [TestMethod]
        public void ConvertToSameFolderTest()
        {
            var webpTestFolder = Path.Combine(TestBaseFolder, WebpTestFolder);

            ImageConverter.Convert(webpTestFolder, TestOutputFolder, ImageFormat.WEBP, ImageFormat.JPEG);

            ImageConverter.Convert(TestOutputFolder, TestOutputFolder, ImageFormat.JPEG, ImageFormat.PNG);

            var input = FileService.GetFiles(webpTestFolder);
            var output = FileService.GetFiles(TestOutputFolder);
            Assert.AreEqual(input.Count(), output.Where(x => x.Extension.Equals(".jpeg")).Count());
            Assert.AreEqual(input.Count(), output.Where(x => x.Extension.Equals(".png")).Count());
        }
    }
}