using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie2
{
    public static class Morpion
    {
        public static void DisplayMorpion(char[,] grille)
        {
            if (grille == null || grille.GetLength(0) != 3 || grille.GetLength(1) != 3)
            {
                Console.WriteLine("Grille invalide");
            }

            for (int i = 0; i < grille.GetLength(0); i++)
            {
                Console.WriteLine($"{grille[i, 0]} {grille[i, 1]} {grille[i, 2]}");
            }
        }

        public static int CheckMorpion(char[,] grille)
        {
            int res = -1;
            // Lignes
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                res = Winner(grille[i, 0], grille[i, 1], grille[i, 2]);
                if (res != 0) return res;
            }
            
            // Colonnes
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                res = Winner(grille[0, i], grille[1, i], grille[2, i]);
                if (res != 0) return res;
            }

            //diagonal de mort
            res = Winner(grille[0, 0], grille[1, 1], grille[2, 2]);
            if (res != 0) return res;
            res = Winner(grille[2, 0], grille[1, 1], grille[0, 2]);
            if (res != 0) return res;

            return 0;

        }

        static int Winner(char a, char b, char c)
        {
            if (a == '_' || b == '_' || c == '_') return -1;
            else if (a == 'X' && b == 'X' && c == 'X') return 1;
            else if (a == 'O' && b == 'O' && c == 'O') return 2;
            else return 0;
        }
    }
}
