using Serie1;
using Serie2;
using Serie3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1
{
    internal class Program
    {
        static string text = "";
        static int[] numbers = new int[6] { 1, 2, 3, 4, 5, 6 };
        static int[] numbers2 = new int[3] { 7 , 8 , 9 };
        static string[] caviardage = new string[4] { "crack", "islamophobes", "xénophobes", "camarades" };
        static int[] numb = new int[0] ;
        static char[,] morpion = new char[3,3] 
{ //ligne
    { 'X', 'O', 'X' },
    { 'X', 'O', 'O' },
    { 'X', 'X', 'X' }
};
        static char[,] morpion2 = new char[3, 3]
{ // Colonne
    { 'X', 'O', 'O' },
    { 'X', 'X', 'O' },
    { 'X', 'O', 'X' }
};
        static char[,] morpion3 = new char[3, 3]
{//match null
    { 'X', 'O', 'X' },
    { 'X', 'O', 'O' },
    { 'O', 'X', 'X' }
};
        static char[,] morpion4 = new char[3, 3]
{ //non finis
    { '_', 'O', 'X' },
    { 'X', 'O', 'O' },
    { 'O', '_', 'X' }
};
        static char[,] morpion5 = new char[3, 3]
{ // diagonal
    { 'X', 'O', 'O' },
    { 'O', 'X', 'O' },
    { 'X', 'O', 'X' }
};
        static void Main(string[] args)
        {
            //serie 1
           /*  Tp1();
             Tp2();
             Tp3();
             TP4();
             TP5();
             TP6();

            //serie 2
            Tp7();
            Tp8();
            Tp9();*/

            //serie 3
            //Tp10();
            //Tp11();
            Tp12();

        }
        //serie 3
        static void Tp10()
        {

            text = AdministrativeTasks.EliminateSeditiousThoughts("je suis chauve et j'aime beaucoup le coca et le crack, " +
            "j'en fume beaucoup avec mes collègues islamophobes et xénophobes. Des vrais camarades.", caviardage);
            Console.WriteLine(text);
        }

        static void Tp11()
        {
            Console.WriteLine("tp11 : ");
            text = "M.  Jean-pascal Pilote      23";
            Console.Write(text + " : ");
            Console.WriteLine(AdministrativeTasks.ControlFormat(text));
            text = "M.  Renoir      Pepito      99";
            Console.Write(text + " : ");
            Console.WriteLine(AdministrativeTasks.ControlFormat(text));
            text = "Mme anthoinette delacour    55";
            Console.Write(text + " : ");
            Console.WriteLine(AdministrativeTasks.ControlFormat(text));
            text = "Mme gzaef*15fds  chong chin 25";
            Console.Write(text + " : ");
            Console.WriteLine(AdministrativeTasks.ControlFormat(text));
        }

        static void Tp12()
        {
            text = "2018-02-02 : la cible mange un cookie sur le balcon depuis le 2018-02-01";
            Console.WriteLine(text);
            text = AdministrativeTasks.ChangeDate(text);
            Console.WriteLine(text);
        }
            //serie 2
            static void Tp7()
        {
            Console.WriteLine("SumTab");
            Console.WriteLine(TasksTables.SumTab(numbers));
            Console.WriteLine(TasksTables.SumTab(numb));
            Console.WriteLine("OpeTab");
            Console.WriteLine(string.Join(", ", TasksTables.OpeTab(numbers,'+',2)));
            Console.WriteLine(string.Join(", ", TasksTables.OpeTab(numbers, '-', 2)));
            Console.WriteLine(string.Join(", ", TasksTables.OpeTab(numbers, '*', 2)));
            Console.WriteLine(string.Join(", ", TasksTables.OpeTab(numbers, 't', 2)));
            Console.WriteLine("ConcatTab");
            Console.WriteLine(string.Join(", ", TasksTables.ConcatTab(numbers, numbers2)));
            Console.WriteLine(string.Join(", ", TasksTables.ConcatTab(numbers, numb)));
            Console.WriteLine(string.Join(", ", TasksTables.ConcatTab(numb, numb)));
        }

        static void Tp8()
        {

            Console.WriteLine("-----1-----");
            Morpion.DisplayMorpion(morpion);
            Console.WriteLine(Morpion.CheckMorpion(morpion));
            Console.WriteLine("-----2-----");
            Morpion.DisplayMorpion(morpion2);
            Console.WriteLine(Morpion.CheckMorpion(morpion2));
            Console.WriteLine("-----3-----");
            Morpion.DisplayMorpion(morpion3);
            Console.WriteLine(Morpion.CheckMorpion(morpion3));
            Console.WriteLine("-----4-----");
            Morpion.DisplayMorpion(morpion4);
            Console.WriteLine(Morpion.CheckMorpion(morpion4));
            Console.WriteLine("-----5-----");
            Morpion.DisplayMorpion(morpion5);
            Console.WriteLine(Morpion.CheckMorpion(morpion5));

        }

        static void Tp9()
        {
            Console.Write("linear : ");
            Console.WriteLine(Search.LinearSearch(numbers, 5));
            Console.Write("linear : ");
            Console.WriteLine(Search.LinearSearch(numb, 5));
            Console.Write("dichoto : ");
            Console.WriteLine(Search.BinarySearch(numbers, 6));
            Console.Write("dichoto : ");
            Console.WriteLine(Search.BinarySearch(numb, 1));

        }

            //serie 1
            //exec 1
            static void Tp1()
        {
            Console.Write("TP1 : ");
            ElementaryOperations.BasicOperation(1, 2, '+');
            ElementaryOperations.BasicOperation(1, 2, '-');
            ElementaryOperations.BasicOperation(1, 2, '*');
            ElementaryOperations.BasicOperation(1, 2, '/');
            ElementaryOperations.BasicOperation(8, 0, '/');
            ElementaryOperations.BasicOperation(1, 2, 'T');
            
        }

        static void Tp2()
        {
            Console.Write("TP2 : ");
            ElementaryOperations.IntegerDivision(2, 4);
            ElementaryOperations.IntegerDivision(2, 2);
            ElementaryOperations.IntegerDivision(125, 6);
            ElementaryOperations.IntegerDivision(5, 0);
            
        }

        static void Tp3()
        {
            Console.Write("TP3 : ");
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
