using Argent.Serie1;

namespace Argent
{
    internal class Program
    {
        static string pathCards = "C:\\INTM\\FormationCsharp\\Argent\\File\\cartes.csv";
        static string pathAccounts = "C:\\INTM\\FormationCsharp\\Argent\\File\\comptes.csv";
        static string pathTransaction = "C:\\INTM\\FormationCsharp\\Argent\\File\\transactions.csv";
        static string pathSortie = "C:\\INTM\\FormationCsharp\\Argent\\File\\Rejets.csv";
        static List<string> rejet = new List<string>();
        static void Main(string[] args)
        {
            Banque banque = new Banque();
            
            if(CsvIo.LoadCards(pathCards, banque))
            {
                banque.AfficheCarte();
            }

            Console.WriteLine();
            if (CsvIo.LoadAccounts(pathAccounts, banque))
            {
                banque.AfficheAccounts();
            }
            Console.WriteLine();
            if (CsvIo.LoadTransaction(pathTransaction, pathSortie, banque, out rejet))
            {
                banque.AfficheTransactions();
            }

            var list = banque.TraiterTransactions(pathSortie);

            var fusionTriee = list.Concat(rejet).OrderBy(GetId).ToList();

            Console.WriteLine();
            foreach (var transaction in fusionTriee) 
            {
                CsvIo.WriteFile(transaction, pathSortie);
                Console.WriteLine(transaction);
            }
        }

        // Une classe statique Tools pourrait être intéressante 

        /// <summary>
        /// permet de trié 2 liste de string sur l'id.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static int GetId(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return int.MaxValue;
            int iColon = line.IndexOf(':'), iSemi = line.IndexOf(';');
            int end = iColon >= 0 ? iColon : (iSemi >= 0 ? iSemi : line.Length);
            var idSpan = line.AsSpan(0, end).Trim();
            return int.TryParse(idSpan, out int id) ? id : int.MaxValue;
        }

        /*****************************************************
         * Suivant le schéma précédent et les contraintes sur les cartes énoncées, lister les diférentes
         *  opérations entre deux comptes autorisées.
         *  Carte 1234… : 1 -> 11 | 1 -> 12 | 11 -> 12 | 11-> 12 | 1->2         
         *  Carte 4567… : 2 -> 21 | 21-> 2 |2->1
         *  **
         *  Le 17/04/2025, quel est le montant maximum autorisé par le plafond d'un virement efectué avec cette carte ?
         *  Somme des débits (du 07/04 au 17/04) :
         *  100 + 350 + 80 + 90 = 620 €
         *  Reste disponible = 1 000 − 620 = max autorisé = 380 €.
         *  **
         *  Pour un virement de 400, à partir de quel horodatage, celui-ci est il possible ? Pour un virement de 600   ?
         *  Pour 400 € : il manque 20 €.
         *  Le premier débit de 100 € du 12/04/2025 10:50:10.
         *  Il sort 10 jours plus tard, donc à partir du 22/04/2025 10:50:11.
         *  
         *  Pour 600 € : il manque 220 €.
         *  Après la sortie du 100 €, il manque encore 120 €.
         *  Le débit suivant à sortir est le 350 € du 14/04/2025 15:54:14,
         *  donc à partir du 24/04/2025 15:54:15.
         ************************************************************/
    }
}
