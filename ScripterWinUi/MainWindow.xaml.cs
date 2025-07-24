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

    private void NavigateToPage(int index)
    {
        if (index >= 0 && index < _pageSequence.Length)
        {
            _currentPageIndex = index;
            ContentFrame.Navigate(_pageSequence[index]);
        }
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

    public void GoBack()
    {
        if (_currentPageIndex > 0)
        {
            NavigateToPage(_currentPageIndex - 1);
        }
    }

    public void GoForward()
    {
        if (_currentPageIndex < _pageSequence.Length - 1)
        {
            NavigateToPage(_currentPageIndex + 1);
        }
    }
}