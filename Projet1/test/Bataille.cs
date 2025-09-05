using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1.test
{
    enum Cellule
    {
        Vide,
        Touche,
        Rate
    }

    struct Coord
    {
        public int L;  // ligne (0..N-1)
        public int C;  // colonne (0..N-1)
        public Coord(int l, int c) { L = l; C = c; }
    }

    class Navire
    {
        public string Nom { get; }
        public int Taille { get; }
        public List<Coord> Cases { get; } = new List<Coord>();
        public HashSet<(int, int)> Touches { get; } = new HashSet<(int, int)>();

        public Navire(string nom, int taille)
        {
            Nom = nom; Taille = taille;
        }

        public bool EstCoule => Touches.Count >= Taille;
        public bool Contient(int l, int c) => Cases.Any(x => x.L == l && x.C == c);
        public void MarquerTouche(int l, int c) => Touches.Add((l, c));
    }

    class Plateau
    {
        public int Taille { get; }
        private readonly int[,] navireMap;   // -1 = aucun navire, sinon index du navire
        private readonly Cellule[,] tirs;    // état des tirs reçus sur ce plateau
        public List<Navire> Navires { get; } = new List<Navire>();
        private readonly Random rng = new Random();

        public Plateau(int taille = 10)
        {
            Taille = taille;
            navireMap = new int[taille, taille];
            tirs = new Cellule[taille, taille];
            for (int l = 0; l < taille; l++)
                for (int c = 0; c < taille; c++)
                    navireMap[l, c] = -1;
        }

        public void PlacerNaviresAleatoire()
        {
            var defs = new (string nom, int size)[]
            {
                ("Porte-avions", 5),
                ("Cuirassé", 4),
                ("Croiseur", 3),
                ("Sous-marin", 3),
                ("Torpilleur", 2)
            };

            foreach (var (nom, size) in defs)
            {
                bool place = false;
                int guard = 0;
                while (!place && guard++ < 10_000)
                {
                    bool horizontal = rng.Next(2) == 0;
                    int l = rng.Next(Taille);
                    int c = rng.Next(Taille);
                    if (horizontal && c + size > Taille) continue;
                    if (!horizontal && l + size > Taille) continue;

                    // vérif collision
                    bool libre = true;
                    for (int k = 0; k < size; k++)
                    {
                        int ll = l + (horizontal ? 0 : k);
                        int cc = c + (horizontal ? k : 0);
                        if (navireMap[ll, cc] != -1) { libre = false; break; }
                    }
                    if (!libre) continue;

                    // poser
                    var n = new Navire(nom, size);
                    int idx = Navires.Count;
                    for (int k = 0; k < size; k++)
                    {
                        int ll = l + (horizontal ? 0 : k);
                        int cc = c + (horizontal ? k : 0);
                        navireMap[ll, cc] = idx;
                        n.Cases.Add(new Coord(ll, cc));
                    }
                    Navires.Add(n);
                    place = true;
                }
                if (!place) throw new Exception($"Impossible de placer le navire {nom}");
            }
        }

        public bool TousCoules => Navires.All(n => n.EstCoule);

        public (bool valide, bool deja, bool touche, Navire coule)
            Tirer(int l, int c)
        {
            if (l < 0 || l >= Taille || c < 0 || c >= Taille)
                return (false, false, false, null);

            if (tirs[l, c] != Cellule.Vide)
                return (true, true, false, null);

            int id = navireMap[l, c];
            if (id == -1)
            {
                tirs[l, c] = Cellule.Rate;
                return (true, false, false, null);
            }
            else
            {
                tirs[l, c] = Cellule.Touche;
                var n = Navires[id];
                n.MarquerTouche(l, c);
                return (true, false, true, n.EstCoule ? n : null);
            }
        }

        public Cellule EtatCase(int l, int c) => tirs[l, c];
        public bool AUnNavire(int l, int c) => navireMap[l, c] != -1;

        public void Afficher(double zoom = 1.0, bool montrerNavires = false)
        {
            // En-tête colonnes
            Console.Write("    ");
            for (int c = 0; c < Taille; c++) Console.Write($"{c + 1,2} ");
            Console.WriteLine();

            for (int l = 0; l < Taille; l++)
            {
                char row = (char)('A' + l);
                Console.Write($" {row} | ");
                for (int c = 0; c < Taille; c++)
                {
                    char ch;
                    switch (tirs[l, c])
                    {
                        case Cellule.Touche: ch = 'X'; break;
                        case Cellule.Rate: ch = 'o'; break;
                        default:
                            if (montrerNavires && AUnNavire(l, c)) ch = '#'; else ch = '.';
                            break;
                    }
                    Console.Write($"{ch}  ");
                }
                Console.WriteLine();
            }
        }

        public Coord TirIA(HashSet<(int, int)> dejaTires)
        {
            // IA très simple : aléatoire sur case non tirée
            int guard = 0;
            while (guard++ < 10_000)
            {
                int l = rng.Next(Taille);
                int c = rng.Next(Taille);
                if (!dejaTires.Contains((l, c))) return new Coord(l, c);
            }
            // fallback – devrait jamais arriver
            for (int l = 0; l < Taille; l++)
                for (int c = 0; c < Taille; c++)
                    if (!dejaTires.Contains((l, c))) return new Coord(l, c);
            return new Coord(0, 0);
        }
    }

    static class Parse
    {
        public static bool TryLireCoord(string s, int taille, out Coord coord)
        {
            coord = new Coord(-1, -1);
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim().ToUpperInvariant();

            // Forme attendue: A1 .. J10
            char r = s[0];
            if (r < 'A' || r >= 'A' + taille) return false;
            string num = s.Substring(1);
            if (!int.TryParse(num, out int col)) return false;
            if (col < 1 || col > taille) return false;

            coord = new Coord(r - 'A', col - 1);
            return true;
        }
    }

    class Programme
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("===== BATAILLE NAVALE (console, FR) =====\n");
            Console.WriteLine("Règles rapides :\n- Vous et l'IA avez 5 navires (5,4,3,3,2).\n- Entrez vos tirs au format A1..J10.\n- ‘X’ = touché, ‘o’ = à l’eau, ‘.’ = inconnu, ‘#’ = navire (sur votre plateau).\n");

            var joueur = new Plateau(10);
            var ia = new Plateau(10);
            joueur.PlacerNaviresAleatoire();
            ia.PlacerNaviresAleatoire();

            var dejaTiresIA = new HashSet<(int, int)>();
            var dejaTiresJoueur = new HashSet<(int, int)>();

            while (true)
            {
                Console.WriteLine("\n-- Votre plateau (navires visibles) --");
                joueur.Afficher(montrerNavires: true);
                Console.WriteLine("\n-- Plateau ennemi (brouillard) --");
                ia.Afficher(montrerNavires: false);

                // TOUR JOUEUR
                Coord vis;
                while (true)
                {
                    Console.Write("\nVotre tir (ex: B7) : ");
                    string s = Console.ReadLine();
                    if (!Parse.TryLireCoord(s, ia.Taille, out vis))
                    {
                        Console.WriteLine("Entrée invalide. Réessayez (A1..J10).");
                        continue;
                    }
                    if (dejaTiresJoueur.Contains((vis.L, vis.C)))
                    {
                        Console.WriteLine("Vous avez déjà tiré ici. Choisissez une autre case.");
                        continue;
                    }
                    break;
                }

                dejaTiresJoueur.Add((vis.L, vis.C));
                var (valide, deja, touche, coule) = ia.Tirer(vis.L, vis.C);
                if (!valide)
                {
                    Console.WriteLine("Tir hors plateau (ne devrait pas arriver).");
                }
                else if (deja)
                {
                    Console.WriteLine("(Note) Case déjà jouée (ne devrait pas arriver).");
                }
                else
                {
                    if (touche)
                    {
                        if (coule != null)
                            Console.WriteLine($"➡️ Touché-coulé ! ({coule.Nom})");
                        else
                            Console.WriteLine("➡️ Touché !");
                    }
                    else
                    {
                        Console.WriteLine("💧 À l’eau.");
                    }
                }

                if (ia.TousCoules)
                {
                    Console.WriteLine("\n🎉 Victoire ! Tous les navires ennemis ont été coulés.");
                    break;
                }

                // TOUR IA
                var tirIA = joueur.TirIA(dejaTiresIA);
                dejaTiresIA.Add((tirIA.L, tirIA.C));
                var resIA = joueur.Tirer(tirIA.L, tirIA.C);

                Console.WriteLine($"\nTour de l’IA : {(char)('A' + tirIA.L)}{tirIA.C + 1}");
                if (resIA.touche)
                {
                    if (resIA.coule != null)
                        Console.WriteLine($"⚠️ L’IA a coulé votre {resIA.coule.Nom} !");
                    else
                        Console.WriteLine("⚠️ L’IA a touché l’un de vos navires !");
                }
                else
                {
                    Console.WriteLine("L’IA a tiré à l’eau.");
                }

                if (joueur.TousCoules)
                {
                    Console.WriteLine("\n💥 Défaite… Tous vos navires ont été coulés.");
                    break;
                }
            }

            Console.WriteLine("\nFin de partie. Appuyez sur Entrée pour fermer.");
            Console.ReadLine();
        }
    }
}
