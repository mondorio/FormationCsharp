using Argent.Enum;
using System.Diagnostics;
using System.Globalization;

namespace Argent.Serie1
{
    static class CsvIo
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        private const string DateFormat = "dd/MM/yyyy HH:mm:ss";
        private static bool HeaderDone = false;

        /// <summary>
        /// récupére nos carte dans le fichier cartes
        /// </summary>
        /// <param name="path">chemain du fichier</param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static bool LoadCards(string path, Banque bank)
        {
            if (!File.Exists(path)) return false; //verif que le fichier existe 

            var seen = new HashSet<long>();       // liste d'id pour eviter les doublons 
            foreach (var line in File.ReadLines(path))
            {
                int plafond = 500;
                long numCpt = 0;

                if (string.IsNullOrWhiteSpace(line)) continue; //skip les lignes vide 
                var parts = line.Split(';');
                string num = parts[0].Trim();
                if (parts.Length < 2) continue;

                //recupére le plafond
                if (!string.IsNullOrWhiteSpace(parts[1]) && parts[0].All(char.IsDigit))
                {
                    if (!int.TryParse(parts[1].Trim(), out plafond))
                        continue;
                }
                // récupére l'id du compte - en principe, tu pourrais mettre ce code (et celui dans LoadCompte) dans une 
                // méthode commune 
                if (parts[0].Length == 16 && !string.IsNullOrWhiteSpace(parts[1]) && parts[0].All(char.IsDigit))
                {
                    if (!long.TryParse(parts[0].Trim(), Inv, out numCpt))
                        continue;
                }
                else continue;

                if (seen.Contains(numCpt)) continue;
                plafond = (plafond / 100) * 100;
                plafond = (plafond is >= 500 and <= 3000) ? plafond : 500; // approche fonctionnelle ! :)

                if (bank.AddCard(numCpt, plafond)) seen.Add(numCpt);
            }
            return true;
        }

        /// <summary>
        /// recupére les comptes sur notre fichier comtpes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static bool LoadAccounts(string path, Banque bank)
        {
            if (!File.Exists(path)) return false;

            var seen = new HashSet<int>();
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue; //skip ligne vide
                var parts = line.Split(';');
                if (parts.Length < 3) continue;

                if (!int.TryParse(parts[0].Trim(), out int id)) continue; //skip id vide
                long cardNumb = 0;
                string typeStr = parts[2].Trim();
                //check du type
                AccountType type;
                // C'est correct, précis, mais peut-être trop, moins facilement maintenable, mais OK
                if (string.Equals(typeStr, "Courant", StringComparison.OrdinalIgnoreCase))
                {
                    type = AccountType.Courant;
                }
                else if (string.Equals(typeStr, "Livret", StringComparison.OrdinalIgnoreCase))
                {
                    type = AccountType.Livret;
                }
                else { continue; }// type invalide 

                decimal initial = 0;
                if (parts.Length >= 4 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    // solde initial doit être entier >=0
                    if (!decimal.TryParse(parts[3].Trim(), out initial)) continue;
                    if (initial < 0) continue;
                }
                // recupe le numero de carte - peut-être refactoriser cette expression booléenne dans une méthode privée renvoyant un booléen
                if (parts[1].Length == 16 && !string.IsNullOrWhiteSpace(parts[1]) && parts[1].All(char.IsDigit))
                {
                    if (!long.TryParse(parts[1].Trim(), Inv, out cardNumb)) continue;
                }
                else continue;

                if (seen.Contains(id)) continue;
                if (bank.AddAccount(id, cardNumb, type, initial)) seen.Add(id);
            }
            return true;
        }

        /// <summary>
        /// Récupére nos transaction sur le fichier transactions et fait un premier trie sur les lignes en KO 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathS"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static bool LoadTransaction(string path, string pathS, Banque bank, out List<string> rejet )
        {

            rejet = new List<string>();
            if (!File.Exists(path)) return false;

            //a commenter pour le prog final
            string Fmt(string line, string reason = "")
               //=> $"{line}:KO;{reason}";
               => $"{line}:KO;";

            // Pour le débogage, WriteLine n'est pas forcément le plus adapté.
            // Tu peux utiliser Debug.WriteLine()

            // HashSet<int>, intéressant
            var seen = new HashSet<int>();
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line))
                {// skip la ligne vide
                    continue;
                }
                var parts = line.Split(';');
                if (parts.Length < 5)
                {// verifie qu'on a bien toute les info
                    rejet.Add(Fmt(parts[0].Trim(), "information manquante"));
                    Debug.WriteLine(Fmt(parts[0].Trim(), "information manquante"), pathS); // un exemple 
                    //WriteFile(Fmt(parts[0].Trim(), "information manquante"), pathS);
                    continue;
                }

                if (!int.TryParse(parts[0].Trim(), out int id))
                {
                    rejet.Add(Fmt(parts[0].Trim(), "mauvais id"));
                    //WriteFile(Fmt(parts[0].Trim(), "mauvais id"), pathS);
                    continue;
                }
               
                //06/09/2025 14:05:00
                if (!DateTime.TryParseExact(parts[1].Trim(),"dd/MM/yyyy HH:mm:ss" , Inv, DateTimeStyles.None, out DateTime date))
                {
                    rejet.Add(Fmt(parts[0].Trim(), "date erronée"));
                    //WriteFile(Fmt(parts[0].Trim(), "date erronée"), pathS);
                    continue;
                }
                if (!decimal.TryParse(parts[2].Trim(), Inv, out decimal montant))
                {
                    rejet.Add(Fmt(parts[0].Trim(), "montant erronée"));
                    //WriteFile(Fmt(parts[0].Trim(), "montant erronée"), pathS);
                    continue;
                }
                if (!int.TryParse(parts[3].Trim(), out int idExpe))
                {
                    rejet.Add(Fmt(parts[0].Trim(), "expediteur erronée"));
                    //WriteFile(Fmt(parts[0].Trim(), "expediteur erronée"), pathS);
                    continue;
                }
                if (!int.TryParse(parts[4].Trim(), out int idDest))
                {
                    rejet.Add(Fmt(parts[0].Trim(), "destinataire erronée"));
                    //WriteFile(Fmt(parts[0].Trim(), "destinataire erronée"), pathS);
                    continue;
                }

                if (seen.Contains(id)) continue;
                if (bank.AddTransaction(id, date, montant, idExpe, idDest))
                {
                    seen.Add(id);
                }
                else
                {
                    rejet.Add(Fmt(parts[0].Trim(), "Compte non trouver"));
                    //WriteFile(Fmt(parts[0].Trim(), "Compte non trouver"), pathS);
                    continue;
                }
            }
            return true;
        }
        /// <summary>
        /// ecrit sur le fichier de sortie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="path"></param>
        public static void WriteFile(string line, string path)
        {
            if (!HeaderDone)
            {
                // Pas d'en-tête dans le fichier de sortie
                //File.WriteAllText(path, "id;date;montant;expediteur;reception\n");
                HeaderDone = true;
            }
            File.AppendAllText(path, $"{line}\n");
        }
    }
}
