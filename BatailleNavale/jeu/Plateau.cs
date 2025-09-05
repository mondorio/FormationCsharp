using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Bataille_Navale.Position;
using static System.Net.WebRequestMethods;

namespace Bataille_Navale
{
    internal class Plateau
    {
        private readonly Random rng = new Random();
        public Position[,] PlateauJeu { get; set; }

        public List<Bateau> Bateaux { get; set; }

        public Plateau(int taille)
        {
            PlateauJeu = new Position[10, 10];
            Bateaux = new List<Bateau>()
            {
               new Bateau("A", 5, new List<Position>()),
               new Bateau("B", 4, new List<Position>()),
               new Bateau("C", 3, new List<Position>()),
               new Bateau("D", 3, new List<Position>()),
               new Bateau("E", 2, new List<Position>())
            };
        }
        /// <summary>
        /// crée notre plateau de jeu
        /// </summary>
        public void CreationPlateau()
        {

            int n = PlateauJeu.GetLength(0);
            n = n == 0 ? 1 : n;

            //creation du plateau
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    PlateauJeu[x, y] = new Position(x, y);
                }
            }

            //donne la position au bateau 
            foreach (var b in Bateaux)
            {
                bool place = false; int guard = 0;
                while (!place && guard != 5) //tant que les 5 ne sont pas placer
                {
                    bool vertical = rng.Next(2) == 0;
                    int x = rng.Next(n);
                    int y = rng.Next(n);
                    place = PlacerBateau(x, y, b.Taille, vertical);
                    if (place) guard++;
                }
            }
        }
        /// <summary>
        /// lancement de la partie
        /// </summary>
        public void LancementPartie()
        {
            CreationPlateau();
            int cpt = 0;
            while (!FindePartie())
            {
                Console.Clear();
                AfficherPlateau();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Quelle case visez-vous : (format: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("ligne");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(",");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("colonne");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(")");
                Console.WriteLine();

                //recupére le coup du joueur
                string val = Console.ReadLine();
                string[] position = val.Split(',', '.');
                if (position.Length == 2)
                {
                    //test le coup
                    bool t = true;
                    while (t)
                    {
                        if (int.TryParse(position[0], out int pos1) && int.TryParse(position[1], out int pos2)) // check que c'est bien des chiffre
                        {
                            if (pos1 > 0 && pos1 < 11 && pos2 > 0 && pos2 < 11) //verifi qu'on est bien dans les clous
                            {
                                Viser(pos1 - 1, pos2 - 1); //-1 car on est pas sensé rentré 0
                                t = false;
                                cpt++;
                            }
                            else break;
                        }
                        else break;
                    }
                    t = true;
                }
            }
            //affichage de fin de partie
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            AfficherPlateau();
            Console.Write($"GG {cpt} coups effectués !");
        }

        /// <summary>
        /// Peut-on placer le navire sur la grille sans qu'il dépasse les bords et qu'il ne touche les autres bateaux ? 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="taille"></param>
        /// <param name="estVertical"></param>
        /// <returns></returns>
        private bool PlacerBateau(int x, int y, int taille, bool estVertical)
        {
            if (estVertical)
            {
                if (x < 0 || x + taille > 10 || y < 0 || y >= 10) return false;
            }
            else
            {
                if (x < 0 || x >= 10 || y < 0 || y + taille > 10) return false;
            }

            //verif bateau pas placer
            Bateau cible = null;
            for (int bi = 0; bi < Bateaux.Count; bi++)
            {
                var bt = Bateaux[bi];
                if (bt.Taille == taille && bt.Positions.Count == 0)
                {
                    cible = bt;
                    break;
                }
            }
            if (cible == null) return false;


            //on prend toute les position du bateau
            List<Position> pos = new List<Position>();
            for (int i = 0; i < taille; i++)
            {
                if (estVertical)
                {
                    pos.Add(new Position(x + i, y));
                }
                else
                {
                    pos.Add(new Position(x, y + i));
                }
            }

            //verifie si on touche le voisin
            foreach (var b in Bateaux) // parcour les bateaux
            {
                if (b.Positions.Count == 0) continue; //skip si pas de position

                foreach (var p in b.Positions) // on parcour les position des bateaux
                {
                    foreach (var po in pos) // on parcour les position du bateaux que l'on veux ajouter pour verifier qu'elle ne se touche pas
                    {
                        if (ToucheVoisin(p, po)) return false;
                    }
                }
            }

            //  ajouter les cases au bateau
            for (int i = 0; i < pos.Count; i++)
            {
                cible.Positions.Add(pos[i]);
            }
            return true;
        }

        /// <summary>
        /// on verifie que le bateau ne touche pas les case voisine a un autre bateau (j'ai mis 2 par sécurité)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool ToucheVoisin(Position a, Position b)
        {
            int dx = a.X - b.X;
            if (dx < -2 || dx > 2) return false;

            int dy = a.Y - b.Y;
            return dy >= -2 && dy <= 2;
        }

        /// <summary>
        /// Choix de la case (x , y) 
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public void Viser(int x, int y)
        {
            bool touch = false;
            Position position = new Position(x, y);
            foreach (var b in Bateaux) //pour chaque bateaux
            {
                position = b.Cible(x, y);
                if (position != null && position.Statut != Etat.Coulé) // si la pos recupéré n'est pas null c'est qu'on touche
                {
                    position.Touché();
                    b.doitCoule();       //verification de si il est coulé si oui le coule.
                    touch = true;
                    break;
                }
                else if (position != null && position.Statut == Etat.Coulé) // si c'est déja coulé on ne fait rien
                {
                    touch = true;
                    break;
                }
            }
            if (!touch) //si on ne touche pas affiche plouf a la position
            {
                PlateauJeu[x, y].Plouf();
            }

        }

        /// <summary>
        /// Affichage de l'état de la grille et de la situation de la partie
        /// </summary>
        public void AfficherPlateau()
        {
            List<Position> list = new List<Position>();
            foreach (Bateau b in Bateaux)
            {
                list.AddRange(b.Positions);
                Console.WriteLine($"{b.Nom}: {b.Taille} de long, coulé: {b.EstCoulé()}");
            }

            foreach (Position p in list)
            {
                PlateauJeu.SetValue(p, p.X, p.Y);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   1 2 3 4 5 6 7 8 9 10");
            int cpt = 0, tmp = 0;
            foreach (Position p in PlateauJeu)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (p.X != tmp || cpt == 0)
                {
                    if (cpt > 0)
                    {
                        Console.WriteLine();
                    }
                    Console.Write(string.Format("{0,-3}", ++cpt));
                }

                ConsoleColor foreground;
                switch (p.Statut)
                {
                    case Position.Etat.Plouf:
                        foreground = ConsoleColor.Blue;
                        break;
                    case Position.Etat.Touché:
                        foreground = ConsoleColor.Red;
                        break;
                    case Position.Etat.Coulé:
                        foreground = ConsoleColor.Green;
                        break;
                    default:
                        foreground = ConsoleColor.White;
                        break;
                }
                Console.ForegroundColor = foreground;
                Console.Write((char)p.Statut + " ");

                tmp = p.X;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        /// <summary>
        /// La partie est-elle finie ? 
        /// </summary>
        /// <returns></returns>
        internal bool FindePartie()
        {
            return Bateaux.All(b => b.EstCoulé());
        }
    }
}