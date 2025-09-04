using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie2
{
    public static class TasksTables
    {
        static int sum = 0;
        public static int SumTab(int[] tab)
        {
            if (tab == null || tab.Length == 0) return -1;

            foreach (var x in tab)
            {
                sum += x;
            }
            return sum;
        }

        public static int[] OpeTab(int[] tab, char ope, int b)
        {
            if (tab == null || tab.Length == 0) return Array.Empty<int>();
            if (ope != '+' && ope != '-' && ope != '*') return Array.Empty<int>();

            var res = new int[tab.Length];
            for (int i = 0; i < tab.Length; i++)
            {
                switch (ope)
                {
                    case '+':
                        res[i] = i+b;
                        break;
                    case '-':
                        res[i] = i - b;
                        break;
                    case '*':
                        res[i] = i * b;
                        break;
                    default:
                        Console.WriteLine("operator incorrecte");
                        break;
                }
            }
            return res;
        }

        public static int[] ConcatTab(int[] tab1, int[] tab2)
        {
            // Tu pourrais renvoyer null ou new int[0], après cela fonctionne. 
            if (tab1 == null || tab1.Length == 0 && tab2 == null || tab2.Length == 0) 
                return Array.Empty<int>();

            int l = tab1.Length + tab2.Length;
            int[] tab3 = new int[l];
            // Fonctionne
            Array.Copy(tab1,tab3 , tab1.Length);
            Array.Copy(tab2,tab3 , tab2.Length);
            return tab3;
        }
    }
}
