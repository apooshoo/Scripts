using Common;
using Scripts;

namespace ScriptsTest
{
    [TestClass]
    public class ImageConverterTests
    {
        const string testFolder = @"C:\Users\jonat\OneDrive\Desktop\test\TestWebp\";
        const string outputFolder = @"C:\Users\jonat\OneDrive\Desktop\test\Output\";

        private ImageConverter _model;

        [TestInitialize]
        public void Init()
        {
            _model = new ImageConverter();
            CleanUpFolder(outputFolder);
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanUpFolder(outputFolder);
        }

        private static void CleanUpFolder(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        [TestMethod]
        public void RunTest()
        {
            _model.Convert(testFolder, outputFolder);

            var input = FileService.GetFiles(testFolder);
            var output = FileService.GetFiles(outputFolder);
            Assert.AreEqual(input.Count(), output.Count());
            Assert.IsTrue(output.All(x => x.Extension.Equals(".jpeg")));
        }
    }
}