using client.Presentation.Algorithms;
using client.Domain.Enum;
using client.Domain.Models;

using client.Application.Interfaces;

using System.Diagnostics;

namespace client.tests;

public class MockTask : ITask
{
    public int? Id { get; }
    public string Title { get; }
    public string Description { get; } = "";
    public DateTime StartDate { get; }
    public DateTime Deadline { get; }
    public TaskProgress Progress { get; }
    public TaskPriority Priority { get; }
    public int? ProjectId { get; }
    public List<User>? UsersAssigned { get; set; }

    // Author: Nicolai and Oliver
    public MockTask(string title, DateTime deadline, TaskPriority priority, TaskProgress progress)
    {
        Title = title;
        Deadline = deadline;
        Priority = priority;
        Progress = progress;
    }
}
public class MergeSortTests
{
    [Fact]
    // Author: Nicolai and Oliver
    public void MergeSort_CorrectlySortElements()
    {
        var list = new List<int> { 5, 1, 7, 3, 9, 2 };
        list.MergeSort(0, list.Count - 1, (a, b) => a.CompareTo(b));

        Assert.Equal(new List<int> { 1, 2, 3, 5, 7, 9 }, list);
    }

    [Fact]
    // Author: Nicolai and Oliver
    public void MergeSort_IsStable()
    {
        var today = DateTime.Today;

        var tasks = new List<ITask>
    {
        new MockTask("Task One", today, TaskPriority.High, TaskProgress.InProgress),
        new MockTask("Task Two", today, TaskPriority.Low, TaskProgress.InProgress),
        new MockTask("Task Three", today, TaskPriority.Normal, TaskProgress.InProgress)
    };

        tasks.MergeSort(
            0,
            tasks.Count - 1,
            (t1, t2) => t1.Deadline.CompareTo(t2.Deadline));

        Assert.Collection(tasks,
            t => Assert.Equal("Task One", t.Title),
            t => Assert.Equal("Task Two", t.Title),
            t => Assert.Equal("Task Three", t.Title));
    }

    [Fact]
    // Author: Nicolai and Oliver
    public void MergeSort_Benchmark()
    {
        int datasize = 15000;
        var random = new Random(42);

        var baselist = new List<int>();
        for (int i = 0; i < datasize; i++)
        {
            baselist.Add(random.Next(1, 100000));
        }

        var listForMerge = new List<int>(baselist);
        var listForQuick = new List<int>(baselist);
        var listForBubble = new List<int>(baselist);

        var watch = Stopwatch.StartNew();
        listForMerge.MergeSort(0, listForMerge.Count - 1, (a, b) => a.CompareTo(b));
        watch.Stop();
        long mergeSortTime = watch.ElapsedMilliseconds;

        watch = Stopwatch.StartNew();
        listForQuick.MergeSort(0, listForQuick.Count - 1, (a, b) => a.CompareTo(b));
        watch.Stop();
        long quickSortTime = watch.ElapsedMilliseconds;

        watch = Stopwatch.StartNew();
        listForBubble.MergeSort(0, listForBubble.Count - 1, (a, b) => a.CompareTo(b));
        watch.Stop();
        long bubbleSortTime = watch.ElapsedMilliseconds;

        Console.WriteLine($"\n--- BENCHMARK RESULTS (Data Size: {datasize} items) ---");
        Console.WriteLine($"Naive Bubble Sort Time : {bubbleSortTime} ms");
        Console.WriteLine($"Custom Merge Sort Time : {mergeSortTime} ms");
        Console.WriteLine($"Quick Sort Time        : {quickSortTime} ms\n");

        Assert.True(mergeSortTime < bubbleSortTime);
    }
    // Quick Sort baseline implementation
    // Author: Nicolai and Oliver
    private void QuickSort(List<int> arr, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(arr, low, high);
            QuickSort(arr, low, pi - 1);
            QuickSort(arr, pi + 1, high);
        }
    }

    // Author: Nicolai and Oliver
    private int Partition(List<int> arr, int low, int high)
    {
        int pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
        (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
        return i + 1;
    }

    // Bubble Sort baseline implementation
    // Author: Nicolai and Oliver
    private void BubbleSort(List<int> arr)
    {
        int n = arr.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                }
            }
        }
    }
}
