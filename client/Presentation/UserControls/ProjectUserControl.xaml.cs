using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private ProjectTask? _selectedPriorityTask;

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
            if (sender is not Button { Tag: ProjectTask task } button)
                return;

            _selectedPriorityTask = task;

            var position = button.TransformToAncestor(ProjectRoot)
                .Transform(new Point(0, button.ActualHeight + 4));

            var maxLeft = Math.Max(8, ProjectRoot.ActualWidth - PriorityPicker.Width - 8);
            var left = Math.Min(position.X, maxLeft);
            PriorityPicker.Margin = new Thickness(left, position.Y, 0, 0);
            PriorityPicker.Visibility = Visibility.Visible;

            RestoreApplicationFocus();
            e.Handled = true;
        }

        private async void Priority_Low_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(TaskPriority.Low);
            e.Handled = true;
        }

        private async void Priority_Medium_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(TaskPriority.Normal);
            e.Handled = true;
        }

        private async void Priority_High_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTaskPriorityAsync(TaskPriority.High);
            e.Handled = true;
        }

        private async Task UpdateTaskPriorityAsync(TaskPriority priority)
        {
            var task = _selectedPriorityTask;
            HidePriorityPicker();

            if (task == null)
            {
                RestoreApplicationFocus();
                return;
            }

            if (DataContext is not ProjectViewModel viewModel)
            {
                RestoreApplicationFocus();
                return;
            }

            if (task.Priority == priority)
            {
                RestoreApplicationFocus();
                return;
            }

            // Tasks are immutable in the UI layer, so changing priority means saving a replacement instance.
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
                RestoreApplicationFocus();
            }
        }

        private void ProjectRoot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PriorityPicker.Visibility != Visibility.Visible)
                return;

            if (e.OriginalSource is DependencyObject source &&
                IsElementInside(source, PriorityPicker))
                return;

            HidePriorityPicker();
            RestoreApplicationFocus();
        }

        private void HidePriorityPicker()
        {
            PriorityPicker.Visibility = Visibility.Collapsed;
        }

        private void RestoreApplicationFocus()
        {
            // The inline priority picker can leave mouse/keyboard focus captured after a click.
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

        private static bool IsElementInside(DependencyObject child, DependencyObject parent)
        {
            var current = child;

            // Walk up the visual tree so outside clicks can dismiss the floating priority picker.
            while (current != null)
            {
                if (ReferenceEquals(current, parent))
                    return true;

                current = VisualTreeHelper.GetParent(current);
            }

            return false;
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
