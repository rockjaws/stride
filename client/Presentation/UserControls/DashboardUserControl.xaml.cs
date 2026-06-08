// =============================================================================
// Author: Nicolaj and Oliver
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using client.Presentation.Strategies;
using client.Presentation.ViewModels;

namespace client.Presentation.UserControls
{
    /// <summary>
    /// Interaction logic for DashboardUserControl.xaml
    /// </summary>
    public partial class DashboardUserControl : UserControl
    {
        // Author: Oliver
        public DashboardUserControl()
        {
            InitializeComponent();
        }

        // Author: Nicolaj
        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            if (DataContext is not DashboardViewModel viewModel) return;

            switch (e.Column.SortMemberPath)
            {
                case "Deadline":
                    e.Handled = true;
                    viewModel.ChangeSortingStrategy(new SortByDeadline());
                    break;
                case "Priority":
                    e.Handled = true;
                    viewModel.ChangeSortingStrategy(new SortByPriority());
                    break;
                case "Title":
                    // maybe later
                    e.Handled = true;
                    break;
            }
        }
    }
}
