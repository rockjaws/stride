using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.ViewModels;

namespace client.Presentation.UserControls
{
    /// <summary>
    /// Interaction logic for ProjectUserControl.xaml
    /// </summary>
    public partial class ProjectUserControl : UserControl
    {
        public ProjectUserControl()
        {
            InitializeComponent();
        }

        private void TaskCard_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (sender is not FrameworkElement { DataContext: ProjectTask task } taskCard)
                return;

            DragDrop.DoDragDrop(taskCard, task, DragDropEffects.Move);
        }

        private void TaskCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            if (sender is not FrameworkElement { DataContext: ProjectTask task })
                return;

            if (DataContext is not ProjectViewModel viewModel)
                return;

            viewModel.SelectedTask = task;
            if (viewModel.ShowSelectedTaskCommand.CanExecute(task))
            {
                viewModel.ShowSelectedTaskCommand.Execute(task);
                e.Handled = true;
            }
        }

        private void Column_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(typeof(ProjectTask))
                ? DragDropEffects.Move
                : DragDropEffects.None;
            e.Handled = true;
        }

        private async void Column_Drop(object sender, DragEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: TaskProgress progress })
                return;

            if (DataContext is not ProjectViewModel viewModel)
                return;

            if (e.Data.GetData(typeof(ProjectTask)) is not ProjectTask task)
                return;

            await viewModel.MoveTaskAsync(task, progress);
            e.Handled = true;
        }
    }
}
