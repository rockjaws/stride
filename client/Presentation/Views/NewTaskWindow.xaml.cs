using System.Windows;
using client.Presentation.ViewModels;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : System.Windows.Window
    {
        public NewTaskWindow()
        {
            InitializeComponent();
        }

        private void onCreateTask(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
