using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace ScripterWinUi;

public sealed partial class MainWindow : Window
{
    public static new MainWindow? Current { get; private set; }
    
    private readonly Type[] _pageSequence = 
    {
        typeof(Pages.FolderPreviewPage),
        typeof(Pages.ReseedOptionsPage),
        typeof(Pages.LogStatusPage)
    };
    
    private int _currentPageIndex = 0;

    public MainWindow()
    {
        InitializeComponent();
        Current = this;
        ContentFrame.Navigated += ContentFrame_Navigated;
        NavigateToPage(0);
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        var pageType = e.SourcePageType;
        
        if (pageType == typeof(Pages.FolderPreviewPage))
        {
            NavView.SelectedItem = NavView.MenuItems.Cast<NavigationViewItem>().First(item => item.Tag.Equals("FolderPreview"));
            _currentPageIndex = 0;
        }
        else if (pageType == typeof(Pages.ReseedOptionsPage))
        {
            NavView.SelectedItem = NavView.MenuItems.Cast<NavigationViewItem>().First(item => item.Tag.Equals("ReseedOptions"));
            _currentPageIndex = 1;
        }
        else if (pageType == typeof(Pages.LogStatusPage))
        {
            NavView.SelectedItem = NavView.MenuItems.Cast<NavigationViewItem>().First(item => item.Tag.Equals("LogStatus"));
            _currentPageIndex = 2;
        }
        
        UpdateNavigationButtonStates();
    }

    private void NavigateToPage(int index)
    {
        if (index >= 0 && index < _pageSequence.Length)
        {
            _currentPageIndex = index;
            ContentFrame.Navigate(_pageSequence[index]);
            UpdateNavigationButtonStates();
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "FolderPreview":
                    NavigateToPage(0);
                    break;
                case "ReseedOptions":
                    NavigateToPage(1);
                    break;
                case "LogStatus":
                    NavigateToPage(2);
                    break;
            }
        }
    }

    private void UpdateNavigationButtonStates()
    {
        // WinUI automatically handles disabled styling when IsEnabled is set
        BackButton.IsEnabled = CanGoBack();
        ForwardButton.IsEnabled = CanGoForward();
    }


    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (CanGoBack())
        {
            NavigateToPage(_currentPageIndex - 1);
        }
    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
        if (CanGoForward())
        {
            NavigateToPage(_currentPageIndex + 1);
        }
    }

    private bool CanGoBack() => _currentPageIndex > 0;
    private bool CanGoForward() => _currentPageIndex < _pageSequence.Length - 1;
}