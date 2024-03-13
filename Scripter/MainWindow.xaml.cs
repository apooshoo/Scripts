using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scripter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Setup();
            DataContext = this;
        }

        private void Setup()
        {
            TargetType.ItemsSource = new TargetTypeOption[]
            {
                new TargetTypeOption { Name = "LOL1"},
                new TargetTypeOption { Name = "LOL2"},
            };
            TargetType.SelectedIndex = 0;
        }

        private void TargetType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //debug
        }
    }

    public class TargetTypeOption
    {
        public string Name { get; set; }
    }
}