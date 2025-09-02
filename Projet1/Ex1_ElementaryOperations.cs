using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public static class ElementaryOperations
    {

        /// <summary>
        /// Opérations de élémentaire.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="operation"></param>
        public static void BasicOperation(int a, int b, char operation)
        {
            switch (operation)
            {
                case '+':
                    Console.WriteLine($"{a} {operation} {b} = {a + b}");
                    break;
                case '-':
                    Console.WriteLine($"{a} {operation} {b} = {a - b}");
                    break;
                case '*':
                    Console.WriteLine($"{a} {operation} {b} = {a * b}");
                    break;
                case '/':
                    if (b == 0)
                    {
                        Console.WriteLine($"{a} {operation} {b} = Opération invalide");
                    }
                    else
                    {
                        Console.WriteLine($"{a} {operation} {b} = {a / b}"); // division entière comme en C#
                    }
                    break;
                default:
                    Console.WriteLine("Opération invalide");
                    break;
            }
        }

        /// <summary>
        /// reste d'une division de 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void IntegerDivision(int a, int b)
        {
            if (b == 0)
            {
                Console.WriteLine($"{a} : {b} = Opération invalide");
                return;
            }

            int q = a / b;
            int r = a % b;

            if (r == 0)
            { Console.WriteLine($"{a} = {q} * {b}"); }
            else
            { Console.WriteLine($"{a} = {q} * {b} + {r}"); }
        }

        /// <summary>
        /// a puissance de b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Pow(int a, int b)
        {
            double result = 1;
            if (b == 0)
            {
                Console.WriteLine($"{a} ^ {b} = {result}");

            }
            else if (0 > b)
            {
                Console.WriteLine($"{a} ^ {b} = Opération invalide");
                return;
            }
            else
            {
                result = Math.Pow(a, b);
                Console.WriteLine($"{a} ^ {b} = {result}");
            }


        }
    }
}
