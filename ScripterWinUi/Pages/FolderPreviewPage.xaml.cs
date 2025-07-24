using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ScripterWinUi.Pages
{
    public sealed partial class FolderPreviewPage : Page
    {
        public FolderPreviewPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current?.GoBack();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current?.GoForward();
        }
    }
}
