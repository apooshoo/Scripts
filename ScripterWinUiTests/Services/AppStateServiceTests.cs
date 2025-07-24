using ScripterWinUi.Models.Ui;
using ScripterWinUi.Services;

namespace ScripterWinUiTests.Services;

[TestClass]
public class AppStateServiceTests
{
    private AppStateService _appStateService;

    [TestInitialize]
    public void Setup()
    {
        // Get a fresh instance for each test
        _appStateService = AppStateService.Instance;
        
        // Reset state
        _appStateService.SelectedFolderPath = string.Empty;
        _appStateService.SelectedFolderOption = null;
        _appStateService.IsTrimEnabled = false;
        _appStateService.IsNormalizeEnabled = false;
        _appStateService.IsReseedEnabled = false;
        _appStateService.IsConvertEnabled = false;
        _appStateService.SelectedFiles.Clear();
        _appStateService.SelectedFolders.Clear();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetOperationSummary_WithNoOperations_ReturnsNoOperationsMessage()
    {
        // Arrange
        _appStateService.SelectedFolderPath = @"C:\TestFolder";

        // Act
        var result = _appStateService.GetOperationSummary();

        // Assert
        Assert.AreEqual("No operations selected", result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void GetOperationSummary_WithAllOperations_ReturnsCorrectSummary()
    {
        // Arrange
        _appStateService.SelectedFolderPath = @"C:\TestFolder";
        _appStateService.IsTrimEnabled = true;
        _appStateService.TrimLeft = 2;
        _appStateService.TrimRight = 3;
        _appStateService.IsNormalizeEnabled = true;
        _appStateService.IsReseedEnabled = true;
        _appStateService.ReseedStartValue = 5;
        _appStateService.ReseedOrder = ReseedOrderSelectionEnum.CreationDate;
        _appStateService.IsConvertEnabled = true;

        // Act
        var result = _appStateService.GetOperationSummary();

        // Assert
        StringAssert.Contains(result, "Trim (Left: 2, Right: 3)");
        StringAssert.Contains(result, "Normalize");
        StringAssert.Contains(result, "Reseed (Start: 5, Order: CreationDate)");
        StringAssert.Contains(result, "Convert WebP");
        StringAssert.Contains(result, @"C:\TestFolder");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void CanStartOperations_WithNoFolderPath_ReturnsFalse()
    {
        // Arrange
        _appStateService.IsTrimEnabled = true;

        // Act
        var result = _appStateService.CanStartOperations();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void CanStartOperations_WithNoOperations_ReturnsFalse()
    {
        // Arrange
        _appStateService.SelectedFolderPath = @"C:\TestFolder";

        // Act
        var result = _appStateService.CanStartOperations();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void CanStartOperations_WithFolderAndOperation_ReturnsTrue()
    {
        // Arrange
        _appStateService.SelectedFolderPath = @"C:\TestFolder";
        _appStateService.IsTrimEnabled = true;

        // Act
        var result = _appStateService.CanStartOperations();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Services")]
    public void Instance_ReturnsSameInstance()
    {
        // Act
        var instance1 = AppStateService.Instance;
        var instance2 = AppStateService.Instance;

        // Assert
        Assert.AreSame(instance1, instance2);
    }
}