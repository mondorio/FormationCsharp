
using BanqueAccount.Core;

namespace BanqueAccount
{
    internal class Program
    {
        static string pathAccounts = "C:\\INTM\\FormationCsharp\\BanqueAccount\\File\\comptes.csv";
        static string pathTransaction = "C:\\INTM\\FormationCsharp\\BanqueAccount\\File\\transactions.csv";
        static string pathSortie = "C:\\INTM\\FormationCsharp\\BanqueAccount\\File\\Rejets.csv";
        static List<string> rejet = new List<string>();
        static void Main(string[] args)
        {
            Banque banque = new Banque();

            Console.WriteLine();
            if (CsvIo.LoadAccounts(pathAccounts, banque))
            {
                banque.afficheAccounts();
            }
            Console.WriteLine();
            if (CsvIo.LoadTransaction(pathTransaction, pathSortie, banque, out rejet))
            {
                banque.afficheTransactios();
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
    }
}
