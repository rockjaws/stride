using System.Windows;

namespace client.Presentation.Views;

/// <summary>
/// Interaction logic for NewProjectWindow.xaml
/// </summary>
public partial class NewProjectWindow : Window
{
    public NewProjectWindow()
    {
        InitializeComponent();
    }

    private void onCreateProject(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void onCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
