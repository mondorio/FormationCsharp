using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolation
{
    public class Percolation
    {
        private readonly bool[,] Open;
        private readonly bool[,] Full;
        public int Size;
        private bool _percolate;

        public Percolation(int size)
        {
            Size = size;
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Size), Size, "Taille de la grille négative ou nulle.");
            }

            Open = new bool[Size, Size];
            Full = new bool[Size, Size];
            _percolate = false;

        }


        public bool IsOpen(int i, int j) => Open[i, j];
        public bool IsFull(int i, int j) => Full[i, j];
        public bool Percolate() => _percolate;

        // retourne voisins (haut, bas, gauche, droite) si dans la grille 
        public List<KeyValuePair<int, int>> CloseNeighbors(int i, int j)
        {
            var res = new List<KeyValuePair<int, int>>(4);
            if (i > 0) res.Add(new KeyValuePair<int, int>(i - 1, j));        // haut
            if (i < Size - 1) res.Add(new KeyValuePair<int, int>(i + 1, j)); // bas
            if (j > 0) res.Add(new KeyValuePair<int, int>(i, j - 1));        // gauche
            if (j < Size - 1) res.Add(new KeyValuePair<int, int>(i, j + 1)); // droite
            return res;
        }

        /// <summary>
        /// Ouvrir une case ; propage l'eau depuis cette case si eau il y a 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void OpenCell(int i, int j)
        {
            if (Open[i, j]) return; // déjà ouverte
            Open[i, j] = true;

            // si c'est sur la 1ère ligne, c'est plein
            bool becomesFull = (i == 0);

            if (!becomesFull)
            {
                // si un voisin est déjà plein, ça devient plein
                var voisins = CloseNeighbors(i, j);
                foreach (var kv in voisins)
                {
                    if (Full[kv.Key, kv.Value])
                    {
                        becomesFull = true;
                        break;
                    }
                }
            }

            if (becomesFull)
            {
                Fill(i, j);
            }
        }
        /// <summary>
        /// pour remplir les cases toucher par l'eau 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void Fill(int i, int j)
        {
            if (!Open[i, j]) return;
            if (Full[i, j]) return;

            var q = new Queue<KeyValuePair<int, int>>();
            Full[i, j] = true;
            q.Enqueue(new KeyValuePair<int, int>(i, j));
            if (i == Size - 1) _percolate = true;

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                var voisins = CloseNeighbors(cur.Key, cur.Value);
                foreach (var nb in voisins)
                {
                    int nx = nb.Key, ny = nb.Value;
                    if (Open[nx, ny] && !Full[nx, ny])
                    {
                        Full[nx, ny] = true;
                        if (nx == Size - 1) _percolate = true;
                        q.Enqueue(new KeyValuePair<int, int>(nx, ny));
                    }
                }
            }
        }
        /// <summary>
        /// Méthode pour la visu : reset et recalcul Full depuis la 1ere ligne
        /// </summary>
        public void RecomputeFullFromTop()
        {
            // reset
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    Full[x, y] = false;

            _percolate = false;
            var q = new Queue<KeyValuePair<int, int>>();

            for (int col = 0; col < Size; col++)
            {
                if (Open[0, col])
                {
                    Full[0, col] = true;
                    q.Enqueue(new KeyValuePair<int, int>(0, col));
                }
            }

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                if (cur.Key == Size - 1) _percolate = true;
                var voisins = CloseNeighbors(cur.Key, cur.Value);
                foreach (var nb in voisins)
                {
                    int nx = nb.Key, ny = nb.Value;
                    if (Open[nx, ny] && !Full[nx, ny])
                    {
                        Full[nx, ny] = true;
                        q.Enqueue(new KeyValuePair<int, int>(nx, ny));
                    }
                }
            }
        }      
    }
}
