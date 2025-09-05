using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Serie4
{
    public static class Morpion
    {
        private static int res; //resultat pour le test du winner
        private static bool setStarter = false; //verif si la saisie est bonne a la demande de celui qui veux commencer
        private static bool coupCorrect = false; //verif que le coup saisie est correct
        private static string currentPlayer; //joueur en train de jouer
        private static string coupJouer; //saisie du coup du joueur
        private static (int ligne, int col) pos; //position dans le morpion
        static char[,] grille = new char[3, 3]; //grille du morpion
        static readonly Dictionary<string, (int ligne, int col)> coupsValides = new Dictionary<string, (int, int)> //coup possible
{
    { "A1", (0,0) }, { "A2", (0,1) }, { "A3", (0,2) },
    { "B1", (1,0) }, { "B2", (1,1) }, { "B3", (1,2) },
    { "C1", (2,0) }, { "C2", (2,1) }, { "C3", (2,2) },
};
        /// <summary>
        /// fonction principal
        /// </summary>
        public static void MorpionGame()
        {
            InitialiserGrille();
            HowStart();
            while (CheckMorpion(grille) < 0)
            {
                InputPlayer(pos);
                DisplayMorpion(grille);
            }

            if (res == 1)
            {
                Console.WriteLine(" Le joueur X a remporté la partie.");
            }
            else if (res == 2)
            {
                Console.WriteLine(" Le joueur O a remporté la partie.");
            }
            else
            {
                Console.WriteLine(" match null");
            }
        }

        /// <summary>
        /// gére la saisi du joueur lors de la partie 
        /// </summary>
        /// <param name="pos"></param>
        private static void InputPlayer((int ligne, int col) pos)
        {
            coupCorrect = false;
            Console.WriteLine($"Coup du joueur {currentPlayer} :");
            coupJouer = Console.ReadLine().ToUpper();
            while (!coupCorrect)
            {
                if (coupsValides.TryGetValue(coupJouer, out pos) && Check(pos))
                {
                    grille[pos.Item1, pos.Item2] = char.Parse(currentPlayer);   //on sais qu'on a qu'un charactère
                    coupCorrect = true;
                    currentPlayer = currentPlayer == "X" ? "O" : "X";
                }
                else
                {
                    Console.WriteLine("Coup incorrect, veuillez réessayer.");
                    coupJouer = Console.ReadLine().ToUpper();
                }
            }
            
        }
        /// <summary>
        /// verifie si il y a déja un X ou un O a la position
        /// </summary>
        /// <param name="pos">position dans le morpion</param>
        /// <returns></returns>
        private static bool Check((int ligne, int col) pos)
        {

            if(grille[pos.Item1, pos.Item2] == 'X' || grille[pos.Item1, pos.Item2] == 'O')
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// ConsoleKeyInfo key; avec le readkey existe
        /// demande quel joueur veux commencer
        /// </summary>
        private static void HowStart()
        {
            Console.WriteLine("Quel joueur veux commencer ? O ou X ?");
            currentPlayer = Console.ReadLine().ToUpper();
            while (!setStarter)
            {
                if (currentPlayer == "X" || currentPlayer == "O")
                {
                    setStarter = true; 
                }
                else
                {
                    Console.WriteLine("ce joueur n'existe pas et j'ai la flemme d'afficher les current player name X ou O ? :");
                    currentPlayer = Console.ReadLine().ToUpper();
                }
            }

        }
        /// <summary>
        /// affiche le morpion
        /// </summary>
        /// <param name="grille">morpion</param>
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

        /// <summary>
        /// verifie si il y a un gagnant.
        /// </summary>
        /// <param name="grille">morpion</param>
        /// <returns></returns>
        public static int CheckMorpion(char[,] grille)
        {
            res = -1;

            // Lignes
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                res = Winner(grille[i, 0], grille[i, 1], grille[i, 2]);
                if (res == 1 || res == 2) return res;
            }

            // Colonnes
            for (int i = 0; i < grille.GetLength(1); i++)
            {
                res = Winner(grille[0, i], grille[1, i], grille[2, i]);
                if (res == 1 || res == 2) return res;
            }

            // Diagonales de mort 
            res = Winner(grille[0, 0], grille[1, 1], grille[2, 2]);
            if (res == 1 || res == 2) return res;

            res = Winner(grille[2, 0], grille[1, 1], grille[0, 2]);
            if (res == 1 || res == 2) return res;

            // pas de gagnant trouvé : on regarde si cases vides
            for (int r = 0; r < grille.GetLength(0); r++)
            {
                for (int c = 0; c < grille.GetLength(1); c++)
                {
                    if (grille[r, c] == '_') return -1;   
                }
            }
                
            return 0; 
        }

        /// <summary>
        /// check si la condition est bonne pour gagner 
        /// </summary>
        /// <param name="a">pos 1</param> 
        /// <param name="b">pos 2</param>
        /// <param name="c">pos 3</param>
        /// <returns></returns>
        static int Winner(char a, char b, char c)
        {
            if (a == 'X' && b == 'X' && c == 'X') return 1;
            if (a == 'O' && b == 'O' && c == 'O') return 2;
            return 0;
        }
        /// <summary>
        /// initialise la grille avec des _
        /// </summary>
        /// <returns></returns>
        public static char[,] InitialiserGrille()
        {
            Console.WriteLine("Début de partie de Morpion:");
            int i, j;
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    grille[i, j] = '_';
                }
            }
            DisplayMorpion(grille);
            return grille;
        }
    }
}
