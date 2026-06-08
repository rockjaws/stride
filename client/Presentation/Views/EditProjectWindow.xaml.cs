// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for EditProjectWindow.xaml
    /// </summary>
    public partial class EditProjectWindow : System.Windows.Window
    {
        // Author: Oliver
        public EditProjectWindow()
        {
            InitializeComponent();
        }

        // Author: Oliver
        private void onUpdateProject(object sender, RoutedEventArgs e) => DialogResult = true;

        // Author: Oliver
        private void onCancel(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
