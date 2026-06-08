// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows;
using client.Presentation.ViewModels;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : System.Windows.Window
    {
        // Author: Oliver
        public NewTaskWindow()
        {
            InitializeComponent();
        }

        // Author: Oliver
        private void onCreateTask(object sender, RoutedEventArgs e)
        {
            if (DataContext is NewTaskViewModel viewModel && !viewModel.Validate(out var message))
            {
                MessageBox.Show(message, "Missing title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        // Author: Oliver
        private void onCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
