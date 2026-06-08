// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows;
using client.Presentation.ViewModels;

namespace client.Presentation.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Author: Oliver
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
