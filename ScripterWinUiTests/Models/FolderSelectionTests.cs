using ScripterWinUi.Models;

namespace ScripterWinUiTests.Models;

[TestClass]
public class FolderSelectionTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithValidName_SetsNameCorrectly()
    {
        // Arrange
        var folderName = "TestFolder";

        // Act
        var folderSelection = new FolderSelection(folderName);

        // Assert
        Assert.AreEqual(folderName, folderSelection.Name, "Name should match provided folder name");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithEmptyName_SetsNameToEmpty()
    {
        // Arrange
        var folderName = "";

        // Act
        var folderSelection = new FolderSelection(folderName);

        // Assert
        Assert.AreEqual("", folderSelection.Name, "Name should be empty when provided empty string");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithComplexPath_SetsNameCorrectly()
    {
        // Arrange
        var folderName = @"C:\Complex\Path\With\Multiple\Levels";

        // Act
        var folderSelection = new FolderSelection(folderName);

        // Assert
        Assert.AreEqual(folderName, folderSelection.Name, "Name should preserve full path");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithSpecialCharacters_SetsNameCorrectly()
    {
        // Arrange
        var folderName = "Folder with spaces & special chars!";

        // Act
        var folderSelection = new FolderSelection(folderName);

        // Assert
        Assert.AreEqual(folderName, folderSelection.Name, "Name should preserve special characters");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        var folderSelection = new FolderSelection(null!);
    }
}