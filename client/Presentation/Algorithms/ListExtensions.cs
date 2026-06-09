// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

namespace client.Presentation.Algorithms;

public static class ListExtensions
{
    // Author: Nicolaj and Oliver
    private static void Merge<T>(List<T> arr, int l, int m, int r, Comparison<T> comparison)
    {
        int n1 = m - l + 1;
        int n2 = r - m;

        // Copy both halves because merging writes back into the source list in place.
        List<T> L = new List<T>(n1);
        List<T> R = new List<T>(n2);
        int i, j;

        for (i = 0; i < n1; ++i)
        {
            L.Add(arr[l + i]);
        }
        for (j = 0; j < n2; ++j)
        {
            R.Add(arr[m + 1 + j]);
        }

        i = 0;
        j = 0;
        int k = l;

        while (i < n1 && j < n2)
        {
            // Prefer the left item on equality to keep the sort stable.
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

        while (i < n1)
        {
            arr[k] = L[i];
            i++;
            k++;
        }

        while (j < n2)
        {
            arr[k] = R[j];
            j++;
            k++;
        }
    }

    // Author: Nicolaj and Oliver
    public static void MergeSort<T>(this List<T> arr, int l, int r, Comparison<T> comparison)
    {
        if (l < r)
        {
            // This form avoids overflowing if the index range is ever very large.
            int m = l + (r - l) / 2;

            MergeSort(arr, l, m, comparison);
            MergeSort(arr, m + 1, r, comparison);

            Merge(arr, l, m, r, comparison);
        }
    }
}
