using Serie1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Tp1();
            Tp2();
            Tp3();
            TP4();
            TP5();
            TP6();
        }

        //exec 1
        static void Tp1()
        {
            ElementaryOperations.BasicOperation(1, 2, '+');
            ElementaryOperations.BasicOperation(1, 2, '-');
            ElementaryOperations.BasicOperation(1, 2, '*');
            ElementaryOperations.BasicOperation(1, 2, '/');
            ElementaryOperations.BasicOperation(8, 0, '/');
            ElementaryOperations.BasicOperation(1, 2, 'T');
            Console.ReadLine();
        }

        static void Tp2()
        {
            ElementaryOperations.IntegerDivision(2, 4);
            ElementaryOperations.IntegerDivision(2, 2);
            ElementaryOperations.IntegerDivision(125, 6);
            ElementaryOperations.IntegerDivision(5, 0);
            Console.ReadLine();
        }

        static void Tp3()
        {
            ElementaryOperations.Pow(2, 2);
            ElementaryOperations.Pow(2, -3);
            ElementaryOperations.Pow(2, 0);
            ElementaryOperations.Pow(0, 3);
            ElementaryOperations.Pow(-2, 2);
            Console.ReadLine();
        }

        //exec 2 
        static void TP4()
        {
            Console.WriteLine(SpeakingClock.GoodDay(-2));
            Console.WriteLine(SpeakingClock.GoodDay(0));
            Console.WriteLine(SpeakingClock.GoodDay(9));
            Console.WriteLine(SpeakingClock.GoodDay(12));
            Console.WriteLine(SpeakingClock.GoodDay(13));
            Console.WriteLine(SpeakingClock.GoodDay(20));
            Console.ReadLine();
        }

        //exec 3
        static void TP5()
        {
            Pyramid.PyramidConstruction(5, true);
            Pyramid.PyramidConstruction(10, true);
            Pyramid.PyramidConstruction(10, false);
            Pyramid.PyramidConstruction(50, false);
            Console.ReadLine();
        }

        //exec 4
        static void TP6()
        {

            Console.WriteLine(Factorial.Factorial_(2));
            Console.WriteLine(Factorial.Factorial_(3));
            Console.WriteLine(Factorial.Factorial_(4));
            Console.WriteLine(Factorial.FactorialRecursive(2));
            Console.WriteLine(Factorial.FactorialRecursive(5));
            Console.WriteLine(Factorial.FactorialRecursive(0));
            Console.WriteLine(Factorial.Factorial_(-1));
            Console.ReadLine();

        }
    }
}
