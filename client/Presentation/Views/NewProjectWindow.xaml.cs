// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows;

using client.Presentation.ViewModels;

namespace client.Presentation.Views;

/// <summary>
/// Interaction logic for NewProjectWindow.xaml
/// </summary>
public partial class NewProjectWindow : Window
{
    // Author: Oliver
    public NewProjectWindow()
    {
        InitializeComponent();
    }

    // Author: Oliver
    private void onCreateProject(object sender, RoutedEventArgs e)
    {
        if (DataContext is NewProjectViewModel viewModel && !viewModel.Validate(out var message))
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
