using Bataille_Navale;

namespace BatailleNavale
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Plateau p = new Plateau(10); 
            p.LancementPartie();
        }
    }
}

/******************** question ************************************************
 * - Dans quelle circonstance un bateau est-il coulé ?
 * Un bateau est considéré comme coulé lorsque toutes les cases qui le composent ont été touchées ; à ce moment-là, on passe toutes ses cases à l’état Coulé 
 * 
 * - Dans quelle situation une partie de bataille navale est terminée ?
 * La partie se termine lorsque tous les bateaux d’un des deux joueurs sont coulés. (ici un joueur)
 * *****************************************************************************/