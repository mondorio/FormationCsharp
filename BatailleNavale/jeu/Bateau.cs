using System.Collections.Generic;
using System.Linq;
using static Bataille_Navale.Position;

namespace Bataille_Navale
{
    internal class Bateau
    {
        public string Nom { get; private set; }
        public int Taille { get; private set; }
        public List<Position> Positions { get; private set; }

        public Bateau(string nom, int taille, List<Position> position)
        {
            Nom = nom;
            Taille = taille;
            Positions = position;
        }

        /// <summary>
        /// Case à l'état touché si elle appartient au bateau
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Touché(int x, int y)
        {
            if (x == 0) return;

            foreach (var position in this.Positions)
            {
                if (position.X == x && position.Y == y) 
                {
                    position.Touché();
                }
            }


        }

        /// <summary>
        /// Le bateau est-il coulé ? 
        /// </summary>
        public bool EstCoulé()
        {
            return Positions.All(p => p.Statut == Etat.Coulé);
        }

        /// <summary>
        /// Le bateau est-il coulé ? 
        /// </summary>
        public void doitCoule()
        {
            if (Positions.All(p => p.Statut == Etat.Touché))
            {
                foreach (var position in this.Positions)
                {
                    position.Coulé();
                }
            }
        }

        /// <summary>
        /// Renvoie la position si celle-ci appartient au Bateau
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Position Cible(int x, int y) => Positions.Find(p => p.X == x && p.Y == y);

    }
}