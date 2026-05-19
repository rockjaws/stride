namespace client.Presentation.Algorithms;

public static class ListExtensions
{
    private static void Merge<T>(List<T> arr, int l, int m, int r, Comparison<T> comparison)
    {
        // Find sizes of two subarrays to be merged
        int n1 = m - l + 1;
        int n2 = r - m;

        // Create temp arrays
        List<T> L = new List<T>(n1);
        List<T> R = new List<T>(n2);
        int i, j;

        // Copy data to temp arrays
        for (i = 0; i < n1; ++i)
        {
            L.Add(arr[l + i]);
        }
        for (j = 0; j < n2; ++j)
        {
            R.Add(arr[m + 1 + j]);
        }

        // Merge the temp arrays
        i = 0;
        j = 0;
        int k = l; // Initial index of merged subarray

        while (i < n1 && j < n2)
        {
            if (comparison(L[i], R[j]) <= 0)
            {
                arr[k] = L[i];
                i++;
            }
            else
            {
                arr[k] = R[j];
                j++;
            }
            k++;
        }

        // Copy remaining elements of L[] if any
        while (i < n1)
        {
            arr[k] = L[i];
            i++;
            k++;
        }

        // Copy remaining elements of R[] if any
        while (j < n2)
        {
            arr[k] = R[j];
            j++;
            k++;
        }
    }

    public static void MergeSort<T>(this List<T> arr, int l, int r, Comparison<T> comparison)
    {
        if (l < r)
        {
            // Find the middle point
            int m = l + (r - l) / 2;

            // Sort first and second halves
            MergeSort(arr, l, m, comparison);
            MergeSort(arr, m + 1, r, comparison);

            // Merge the sorted halves
            Merge(arr, l, m, r, comparison);
        }
    }
}
