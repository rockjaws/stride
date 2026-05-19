using System;
using System.Windows;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for SelectedTaskWindow.xaml
    /// </summary>
    public partial class SelectedTaskWindow : Window
    {
        public SelectedTaskWindow()
        {
            InitializeComponent();
        }

        private void onUpdateTask(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
