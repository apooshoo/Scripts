using ScripterWinUi.Services;
using ScripterWinUi.Models;
using ScripterWinUi.Models.Ui;

namespace ScripterWinUiTests.Services;

[TestClass]
[DoNotParallelize] // Disable parallel execution to prevent file system race conditions
public class FileSelectionServiceTests : TestBase
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
    [TestCategory("Integration")]
    [TestCategory("Services")]
    public void GetSelectedFiles_WithValidPath_ReturnsFileSelections()
    {
        // Arrange
        var testFiles = new[] { "test1.txt", "test2.jpg", "document.pdf" };
        foreach (var fileName in testFiles)
        {
            CreateEmptyFile(Path.Combine(TestOutputFolder, fileName));
        }

        // Act
        var result = FileSelectionService.GetSelectedFiles(TestOutputFolder);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(testFiles.Length, result.Length, "Should return correct number of files");
        
        var resultNames = result.Select(f => f.Name).ToArray();
        foreach (var expectedFile in testFiles)
        {
            Assert.IsTrue(resultNames.Contains(expectedFile), $"Result should contain {expectedFile}");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("Services")]
    public void GetSelectedFiles_WithEmptyFolder_ReturnsEmptyArray()
    {
        // Act
        var result = FileSelectionService.GetSelectedFiles(TestOutputFolder);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array for empty folder");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("Services")]
    public void GetSelectedFolders_WithFolderOption_ReturnsSingleFolder()
    {
        // Arrange
        var option = new FolderSelectionOption(FolderSelectionEnum.Folder, "Selected folder");

        // Act
        var result = FileSelectionService.GetSelectedFolders(TestOutputFolder, option);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(1, result.Length, "Should return single folder for Folder option");
        Assert.AreEqual(TestOutputFolder, result[0].Name, "Should return the specified folder path");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("Services")]
    public void GetSelectedFolders_WithSubFoldersOption_ReturnsSubfolders()
    {
        // Arrange
        var subFolders = new[] { "SubFolder1", "SubFolder2", "SubFolder3" };
        CreateTestSubFolders(TestOutputFolder, subFolders);
        
        var option = new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders");

        // Act
        var result = FileSelectionService.GetSelectedFolders(TestOutputFolder, option);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(subFolders.Length, result.Length, "Should return correct number of subfolders");
        
        var resultNames = result.Select(f => Path.GetFileName(f.Name)).ToArray();
        foreach (var expectedFolder in subFolders)
        {
            Assert.IsTrue(resultNames.Contains(expectedFolder), $"Result should contain subfolder {expectedFolder}");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("Services")]
    public void GetSelectedFolders_WithSubFoldersOptionAndEmptyFolder_ReturnsEmptyArray()
    {
        // Arrange
        var option = new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders");

        // Act
        var result = FileSelectionService.GetSelectedFolders(TestOutputFolder, option);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array when no subfolders exist");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetSelectedFolders_WithNullOption_ReturnsEmptyArray()
    {
        // Act
        var result = FileSelectionService.GetSelectedFolders(TestOutputFolder, null);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array for null option");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetSelectedFiles_WithNonExistentPath_ReturnsEmptyArray()
    {
        // Arrange
        var nonExistentPath = Path.Combine(TestOutputFolder, "NonExistentFolder");

        // Act
        var result = FileSelectionService.GetSelectedFiles(nonExistentPath);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array for non-existent path");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetSelectedFiles_WithNullPath_ReturnsEmptyArray()
    {
        // Act
        var result = FileSelectionService.GetSelectedFiles(null!);

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array for null path");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetSelectedFiles_WithEmptyPath_ReturnsEmptyArray()
    {
        // Act
        var result = FileSelectionService.GetSelectedFiles("");

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(0, result.Length, "Should return empty array for empty path");
    }
}