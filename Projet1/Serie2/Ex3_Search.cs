using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie2
{
    public static class Search
    {
        public static int LinearSearch(int[] tableau, int valeur)
        {
            if (tableau == null || tableau.Length == 0)
            {
                return -1;
            }
            for (int i = 0; i < tableau.Length; i++)
            {
                if (tableau[i] == valeur)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int BinarySearch(int[] tableau, int valeur)
        {
            if (tableau == null || tableau.Length == 0) return -1;
            int left = 0, right = tableau.Length - 1;

            while (left <= right)
            {
                int mid = left + ((right - left) / 2);
                if (tableau[mid] == valeur)
                {
                    return mid;
                }
                if (tableau[mid] < valeur)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }
            return -1;
        }
    }
}
