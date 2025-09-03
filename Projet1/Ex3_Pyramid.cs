using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public static class Pyramid
    {
        static int left;
        static int right;
        static int width;
        static string line = "";

        /// <summary>
        /// création de piramide
        /// </summary>
        /// <param name="n"></param>
        /// <param name="isSmooth"></param>
        public static void PyramidConstruction(int n, bool isSmooth)
        {
            if (n <= 0)
            {
                Console.WriteLine("Hauteur invalide");
                return;
            }

            width = 2 * n - 1;

            for (int j = 1; j < n + 1; j++)
            {
                left = n - j + 1;
                right = n + j - 1;

                for (int pos = 1; pos <= width; pos++)
                {
                    if (isSmooth)
                    {
                        if (pos < left || pos > right)
                        {
                            line += " ";
                        }
                        else
                        {
                            line += "+";
                        }

                    }
                    else
                    {
                        if (pos < left || pos > right)
                        {
                            line += " ";
                        }
                        else if (j % 2 == 1)
                        {
                            line += "+";
                        }
                        else
                        {
                            line += "-";
                        }
                    }
                }
                Console.WriteLine(line);
                line = "";
            }
        }
    }
}

