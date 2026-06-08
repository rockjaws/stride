// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace client.Presentation.Views
{
    /// <summary>
    /// Interaction logic for NewChannelWindow.xaml
    /// </summary>
    public partial class NewChannelWindow : Window
    {
        // Author: Nicolai
        public NewChannelWindow()
        {
            InitializeComponent();
        }

        // Author: Nicolai
        private void OnCreate(object sender, RoutedEventArgs e) => DialogResult = true;
        // Author: Nicolai
        private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
