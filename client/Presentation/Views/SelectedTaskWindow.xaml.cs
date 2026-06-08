// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for SelectedTaskWindow.xaml
    /// </summary>
    public partial class SelectedTaskWindow : System.Windows.Window
    {
        public bool DeleteRequested { get; private set; }

        // Author: Oliver
        public SelectedTaskWindow()
        {
            InitializeComponent();
        }

        // Author: Oliver
        private void onUpdateTask(object sender, RoutedEventArgs e) => DialogResult = true;

        // Author: Oliver
        private void onCancel(object sender, RoutedEventArgs e) => DialogResult = false;

        // Author: Oliver
        private void onDeleteTask(object sender, RoutedEventArgs e)
        {
            var msgBox = MessageBox.Show(
                "Do you want to delete this task?",
                "Are you sure?",
                MessageBoxButton.OKCancel
            );

            if (msgBox != MessageBoxResult.OK)
                return;

            DeleteRequested = true;
            DialogResult = true;
        }
    }
}
