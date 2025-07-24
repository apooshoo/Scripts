using ScripterWinUi.Models.Ui;

namespace ScripterWinUiTests.Models.Ui;

[TestClass]
public class FolderSelectionOptionTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        // Arrange
        var enumValue = FolderSelectionEnum.Folder;
        var text = "Selected folder";

        // Act
        var option = new FolderSelectionOption(enumValue, text);

        // Assert
        Assert.AreEqual(enumValue, option.Enum, "Enum property should match provided value");
        Assert.AreEqual(text, option.Text, "Text property should match provided value");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void ToString_ReturnsTextProperty()
    {
        // Arrange
        var enumValue = FolderSelectionEnum.SubFolders;
        var text = "Subfolders of selected folder";
        var option = new FolderSelectionOption(enumValue, text);

        // Act
        var result = option.ToString();

        // Assert
        Assert.AreEqual(text, result, "ToString should return the Text property for ComboBox display");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithFolderEnum_CreatesCorrectOption()
    {
        // Arrange & Act
        var option = new FolderSelectionOption(FolderSelectionEnum.Folder, "Test Folder");

        // Assert
        Assert.AreEqual(FolderSelectionEnum.Folder, option.Enum, "Should store Folder enum correctly");
        Assert.AreEqual("Test Folder", option.Text, "Should store text correctly");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithSubFoldersEnum_CreatesCorrectOption()
    {
        // Arrange & Act
        var option = new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Test SubFolders");

        // Assert
        Assert.AreEqual(FolderSelectionEnum.SubFolders, option.Enum, "Should store SubFolders enum correctly");
        Assert.AreEqual("Test SubFolders", option.Text, "Should store text correctly");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    public void Constructor_WithEmptyText_SetsEmptyText()
    {
        // Arrange & Act
        var option = new FolderSelectionOption(FolderSelectionEnum.Folder, "");

        // Assert
        Assert.AreEqual("", option.Text, "Should accept empty text");
        Assert.AreEqual("", option.ToString(), "ToString should return empty string");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Models")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullText_ThrowsArgumentNullException()
    {
        // Act & Assert
        var option = new FolderSelectionOption(FolderSelectionEnum.Folder, null!);
    }
}