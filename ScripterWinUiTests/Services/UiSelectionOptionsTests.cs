using ScripterWinUi.Services;
using ScripterWinUi.Models.Ui;

namespace ScripterWinUiTests.Services;

[TestClass]
public class UiSelectionOptionsTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void DefaultFolderSelectionOptions_ContainsExpectedOptions()
    {
        // Act
        var options = UiSelectionOptions.DefaultFolderSelectionOptions;

        // Assert
        Assert.IsNotNull(options, "Default options should not be null");
        Assert.AreEqual(2, options.Length, "Should contain exactly 2 default options");
        
        // Check first option
        Assert.AreEqual(FolderSelectionEnum.Folder, options[0].Enum, "First option should be Folder");
        Assert.AreEqual("Selected folder", options[0].Text, "First option text should match expected value");
        
        // Check second option
        Assert.AreEqual(FolderSelectionEnum.SubFolders, options[1].Enum, "Second option should be SubFolders");
        Assert.AreEqual("Subfolders of selected folder", options[1].Text, "Second option text should match expected value");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void DefaultFolderSelectionOptions_AllOptionsHaveValidToString()
    {
        // Act
        var options = UiSelectionOptions.DefaultFolderSelectionOptions;

        // Assert
        foreach (var option in options)
        {
            Assert.IsFalse(string.IsNullOrEmpty(option.ToString()), $"Option {option.Enum} should have non-empty ToString for ComboBox display");
        }
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void DefaultFolderSelectionOptions_IsReadonly()
    {
        // Arrange
        var options1 = UiSelectionOptions.DefaultFolderSelectionOptions;
        var options2 = UiSelectionOptions.DefaultFolderSelectionOptions;

        // Assert
        Assert.AreSame(options1, options2, "Should return the same readonly array instance");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void DefaultFolderSelectionOptions_CoversAllEnumValues()
    {
        // Arrange
        var allEnumValues = Enum.GetValues<FolderSelectionEnum>();
        var options = UiSelectionOptions.DefaultFolderSelectionOptions;

        // Act
        var optionEnums = options.Select(o => o.Enum).ToArray();

        // Assert
        foreach (var enumValue in allEnumValues)
        {
            Assert.IsTrue(optionEnums.Contains(enumValue), $"Default options should include {enumValue}");
        }
    }
}