using BanqueAccount.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BanqueAccount.Core
{
    static class CsvIo
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        private const string DateFormat = "dd/MM/yyyy HH:mm:ss";
        private static bool HeaderDone = false;


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

                if (!int.TryParse(parts[0].Trim(), out int id)) continue; //skip id vide
                string typeStr = parts[2].Trim();
                //check du type
                AccountType type;
                if (string.Equals(typeStr, "Courant", StringComparison.OrdinalIgnoreCase)) type = AccountType.Courant;
                else if (string.Equals(typeStr, "Livret", StringComparison.OrdinalIgnoreCase)) type = AccountType.Livret;
                else continue; // type invalide 

                decimal initial = 0;
                if (parts.Length >= 3 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    // solde initial doit être entier >=0
                    if (!decimal.TryParse(parts[3].Trim(), out initial)) continue;
                    if (initial < 0) continue;
                }


                if (seen.Contains(id)) continue;
                if (bank.AddAccount(id,  type, initial)) seen.Add(id);
            }
            return true;
        }

        /// <summary>
        /// récupére nos transaction sur le fichier transactions et fait un premier trie sur les lignes en KO 
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
               => $"{line}:KO;{reason}";
               //=> $"{line}:KO;";

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
                File.WriteAllText(path, "id;status;\n");
                HeaderDone = true;
            }
            File.AppendAllText(path, $"{line}\n");
        }
    }
}
