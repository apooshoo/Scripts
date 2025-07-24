using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ScripterWinUi.Models.Ui;
using ScripterWinUi.Services;
using System.Collections.Generic;
using System.Linq;

namespace ScripterWinUi.Pages;

public sealed partial class ReseedOptionsPage : Page
{
    private ReseedOrderSelectionOption[] _reseedOrderOptions = UiSelectionOptions.DefaultReseedSelectionOptions;
    private readonly AppStateService _appState = AppStateService.Instance;

    public ReseedOptionsPage()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        ReseedOrderComboBox.ItemsSource = _reseedOrderOptions;
        ReseedOrderComboBox.SelectedIndex = 0;

        UpdateStatus();
    }

    private void LoadStateFromAppService()
    {
        TrimCheckBox.IsChecked = _appState.IsTrimEnabled;
        TrimLeftNumberBox.Value = _appState.TrimLeft;
        TrimRightNumberBox.Value = _appState.TrimRight;

        NormalizeCheckBox.IsChecked = _appState.IsNormalizeEnabled;

        ReseedCheckBox.IsChecked = _appState.IsReseedEnabled;
        ReseedValueNumberBox.Value = _appState.ReseedStartValue;

        ReseedOrderComboBox.SelectedItem = _reseedOrderOptions.FirstOrDefault(x => x.Enum == _appState.ReseedOrder) 
            ?? _reseedOrderOptions.FirstOrDefault(x => x.Enum == ReseedOrderSelectionEnum.FileName);

        ConvertCheckBox.IsChecked = _appState.IsConvertEnabled;

        UpdateVisibility();

        UpdateStatus();
    }

    private void SaveStateToAppService()
    {
        _appState.IsTrimEnabled = TrimCheckBox.IsChecked == true;
        _appState.TrimLeft = (int)(TrimLeftNumberBox?.Value ?? 0);
        _appState.TrimRight = (int)(TrimRightNumberBox?.Value ?? 0);
        
        _appState.IsNormalizeEnabled = NormalizeCheckBox.IsChecked == true;
        
        _appState.IsReseedEnabled = ReseedCheckBox.IsChecked == true;
        _appState.ReseedStartValue = (int)(ReseedValueNumberBox?.Value ?? 0);
        _appState.ReseedOrder = ((ReseedOrderSelectionOption?)ReseedOrderComboBox?.SelectedItem)?.Enum ?? ReseedOrderSelectionEnum.FileName;
        
        _appState.IsConvertEnabled = ConvertCheckBox.IsChecked == true;
    }

    private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        UpdateVisibility();
        SaveStateToAppService();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        var selectedOptions = new List<string>();
        
        if (TrimCheckBox.IsChecked == true)
            selectedOptions.Add("Trim");
            
        if (NormalizeCheckBox.IsChecked == true)
            selectedOptions.Add("Normalize");
            
        if (ReseedCheckBox.IsChecked == true)
            selectedOptions.Add("Reseed");
            
        if (ConvertCheckBox.IsChecked == true)
            selectedOptions.Add("Convert");

        if (selectedOptions.Any())
        {
            StatusTextBlock.Text = $"Selected operations: {string.Join(", ", selectedOptions)}";
        }
        else
        {
            StatusTextBlock.Text = "No operations selected";
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        LoadStateFromAppService();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        SaveStateToAppService();
    }

    private void UpdateVisibility()
    {
        TrimOptionsPanel.Visibility = TrimCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        ReseedOptionsPanel.Visibility = ReseedCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
    }
}
