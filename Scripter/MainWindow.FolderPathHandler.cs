using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Scripter
{
    public partial class MainWindow
    {
        private void FolderPathTextBox_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select folder",
                InitialDirectory = "C:\\Users\\jonat\\Downloads",
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Multiselect = false,
            };

            dialog.ShowDialog();
            var result = dialog.FolderNames.FirstOrDefault();
            if (!string.IsNullOrEmpty(result))
            {
                FolderPathTextBox.Text = result;
                OnFolderPathChanged();
            }
        }

        private void FolderPathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            OnFolderPathChanged();
        }

        private void FolderSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFolderPathChanged();
        }

        private void OnFolderPathChanged()
        {
            if (!string.IsNullOrEmpty(FolderPathTextBox.Text))
            {
                TryPopulatePreviews(FolderPathTextBox.Text);
            }
        }

        private void ReseedSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TBD: Perhaps show a mockup of the result
        }
    }
}
