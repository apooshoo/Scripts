using Common;
using Scripts;

namespace ScriptsTest
{
    [TestClass]
    public class FileReseederTests : TestBase
    {
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
        public void ReseedTest()
        {
            string[] fileNames =
            [
                "2.txt",
                "1.txt",
                "4.txt",
                "3.txt",
                "5.txt",
                "7.txt",
                "6.txt",
                "10.txt",
                "9.txt",
                "8.txt"
            ];

            foreach (var fileName in fileNames)
            {
                CreateEmptyFile(Path.Combine(TestOutputFolder, fileName));
            }

            FileRenamer.ReseedFiles(TestOutputFolder, 5);

            var output = FileService.GetFiles(TestOutputFolder).OrderBy(f => f.Name).ToArray();
            Assert.AreEqual("05.txt", output.First().Name);
            Assert.AreEqual("14.txt", output.Last().Name);
        }
    }
}
