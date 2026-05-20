using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
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
        private FrameworkElement? _lastPriorityFocusTarget;

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

        private async void DeleteTask_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (sender is not FrameworkElement { DataContext: ProjectTask task })
                return;

            if (DataContext is not ProjectViewModel viewModel)
                return;

            var msgBox = MessageBox.Show(
                "Do you want to delete this task?",
                "Are you sure?",
                MessageBoxButton.OKCancel
            );

            if (msgBox != MessageBoxResult.OK)
                return;

            await viewModel.DeleteTaskAsync(task);
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

        private void ChangePriority_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.ContextMenu == null)
                return;

            _lastPriorityFocusTarget = button;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private async void Priority_Low_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(sender, TaskPriority.Low);
        }

        private async void Priority_Medium_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(sender, TaskPriority.Normal);
        }

        private async void Priority_High_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(sender, TaskPriority.High);
        }

        private async Task UpdateTaskPriorityAsync(object sender, TaskPriority priority)
        {
            var focusTarget = ClosePriorityContextMenu(sender);

            if (sender is not FrameworkElement { Tag: ProjectTask task })
            {
                RestoreApplicationFocus(focusTarget);
                return;
            }

            if (DataContext is not ProjectViewModel viewModel)
            {
                RestoreApplicationFocus(focusTarget);
                return;
            }

            if (task.Priority == priority)
            {
                RestoreApplicationFocus(focusTarget);
                return;
            }

            var updatedTask = new ProjectTask(
                task.Id,
                task.Title,
                task.Description,
                task.StartDate,
                task.Deadline,
                task.Progress,
                priority,
                task.ProjectId
            );

            try
            {
                await viewModel.UpdateTaskAsync(updatedTask);
            }
            finally
            {
                RestoreApplicationFocus(focusTarget);
            }
        }

        private FrameworkElement? ClosePriorityContextMenu(object sender)
        {
            if (sender is MenuItem menuItem &&
                ItemsControl.ItemsControlFromItemContainer(menuItem) is ContextMenu contextMenu)
            {
                var focusTarget = contextMenu.PlacementTarget as FrameworkElement;
                contextMenu.IsOpen = false;
                return focusTarget ?? _lastPriorityFocusTarget;
            }

            return _lastPriorityFocusTarget;
        }

        private void RestoreApplicationFocus(FrameworkElement? focusTarget)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Mouse.Capture(null);
                Keyboard.ClearFocus();

                var window = Window.GetWindow(ProjectRoot);
                window?.Activate();
                window?.Focus();

                FocusManager.SetFocusedElement(window, ProjectRoot);
                Keyboard.Focus(ProjectRoot);
                ProjectRoot.Focus();
            }, DispatcherPriority.ApplicationIdle);
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
