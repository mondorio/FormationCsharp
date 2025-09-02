using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public static class Factorial
    {
        /// <summary>
        /// factoriel exemple
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Factorial_(int n)
        {
            try
            {
                if (n < 0)
                    throw new ArgumentOutOfRangeException(nameof(n), "EXCEPTION !!! n doit être >= 0");

                int result = 1;
                for (int i = 2; i <= n; i++)
                {
                    result *= i;
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        /// <summary>
        /// factoriel recursive 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int FactorialRecursive(int n)
        {
            try
            {
                if (n < 0)
                    throw new ArgumentOutOfRangeException(nameof(n), "EXCEPTION !!! n doit être >= 0");

                if (n == 0)
                    return 1;

                return n * FactorialRecursive(n - 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
