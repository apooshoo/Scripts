using ScripterWinUi.Models;

namespace ScripterWinUiTests.Models;

[TestClass]
[DoNotParallelize]
public class FileSelectionTests : TestBase
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
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithValidFileInfo_SetsPropertiesCorrectly()
    {
        // Arrange
        var testFileName = "test.txt";
        var testFilePath = Path.Combine(TestOutputFolder, testFileName);
        CreateEmptyFile(testFilePath);
        var fileInfo = new FileInfo(testFilePath);

        // Act
        var fileSelection = new FileSelection(fileInfo);

        // Assert
        Assert.AreEqual(testFileName, fileSelection.Name, "Name should match file name");
        Assert.AreEqual(".txt", fileSelection.Extension, "Extension should match file extension");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithFileWithoutExtension_SetsExtensionToEmpty()
    {
        // Arrange
        var testFileName = "testfile";
        var testFilePath = Path.Combine(TestOutputFolder, testFileName);
        CreateEmptyFile(testFilePath);
        var fileInfo = new FileInfo(testFilePath);

        // Act
        var fileSelection = new FileSelection(fileInfo);

        // Assert
        Assert.AreEqual(testFileName, fileSelection.Name, "Name should match file name");
        Assert.AreEqual("", fileSelection.Extension, "Extension should be empty for files without extension");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithComplexFileName_ParsesCorrectly()
    {
        // Arrange
        var testFileName = "complex.file.name.with.dots.jpg";
        var testFilePath = Path.Combine(TestOutputFolder, testFileName);
        CreateEmptyFile(testFilePath);
        var fileInfo = new FileInfo(testFilePath);

        // Act
        var fileSelection = new FileSelection(fileInfo);

        // Assert
        Assert.AreEqual(testFileName, fileSelection.Name, "Name should include full file name");
        Assert.AreEqual(".jpg", fileSelection.Extension, "Extension should be the last extension only");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullFileInfo_ThrowsArgumentNullException()
    {
        // Act & Assert
        var fileSelection = new FileSelection(null!);
    }
}