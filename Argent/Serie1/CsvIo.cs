using Argent.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Argent.Serie1
{
    static class CsvIo
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        private const string DateFormat = "dd/MM/yyyy HH:mm:ss";
        private static bool HeaderDone = false;
        /// <summary>
        /// lecture des carte
        /// </summary>
        /// <param name="path">chemain du fichier</param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static bool LoadCards(string path, Banque bank)
        {
            if (!File.Exists(path)) return false;

            var seen = new HashSet<long>();
            foreach (var line in File.ReadLines(path))
            {
                int plafond = 500;
                long numCpt = 0;

                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(';');
                string num = parts[0].Trim();
                if (parts.Length < 2) continue;

                if (!string.IsNullOrWhiteSpace(parts[1]) && parts[0].All(char.IsDigit))
                {
                    if (!int.TryParse(parts[1].Trim(), out plafond))
                        continue;
                }

                if (parts[0].Length == 16 && !string.IsNullOrWhiteSpace(parts[1]) && parts[0].All(char.IsDigit))
                {
                    if (!long.TryParse(parts[0].Trim(), Inv, out numCpt))
                        continue;
                }
                else continue;

                if (seen.Contains(numCpt)) continue;

                plafond = (plafond is >= 500 and <= 3000) ? plafond : 500;

                if (bank.AddCard(numCpt, plafond)) seen.Add(numCpt);
            }
            return true;
        }

        public static bool LoadAccounts(string path, Banque bank)
        {
            if (!File.Exists(path)) return false;

            var seen = new HashSet<int>();
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(';');
                if (parts.Length < 3) continue;

                if (!int.TryParse(parts[0].Trim(), out int id)) continue;
                long cardNumb = 0;
                string typeStr = parts[2].Trim();

                AccountType type;
                if (string.Equals(typeStr, "Courant", StringComparison.OrdinalIgnoreCase)) type = AccountType.Courant;
                else if (string.Equals(typeStr, "Livret", StringComparison.OrdinalIgnoreCase)) type = AccountType.Livret;
                else continue; // type invalide 

                decimal initial = 0;
                if (parts.Length >= 4 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    // solde initial doit être entier >=0
                    if (!decimal.TryParse(parts[3].Trim(), out initial)) continue;
                    if (initial < 0) continue;
                }

                if (parts[1].Length == 16 && !string.IsNullOrWhiteSpace(parts[1]) && parts[1].All(char.IsDigit))
                {
                    if (!long.TryParse(parts[1].Trim(), Inv, out cardNumb))
                        continue;
                }
                else continue;

                if (seen.Contains(id)) continue;
                if (bank.AddAccount(id, cardNumb, type, initial)) seen.Add(id);
            }
            return true;
        }


        public static bool LoadTransaction(string path, string pathS, Banque bank)
        {
            if (!File.Exists(path)) return false;

            //a commenter pour le prog final
            string Fmt(string line, string reason = "")
               => $"{line}:KO;{reason}";
               //=> $"{line}:KO;";

            var seen = new HashSet<int>();
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var parts = line.Split(';');
                if (parts.Length < 5)
                {
                    WriteFile(Fmt(line, "information manquante"), pathS);
                    continue;
                }

                if (!int.TryParse(parts[0].Trim(), out int id))
                {
                    WriteFile(Fmt(line, "mauvais id"), pathS);
                    continue;
                }
               
                //06/09/2025 14:05:00
                if (!DateTime.TryParseExact(parts[1].Trim(),"dd/MM/yyyy HH:mm:ss" , Inv, DateTimeStyles.None, out DateTime date))
                {
                    WriteFile(Fmt(line, "date erronée"), pathS);
                    continue;
                }
                if (!decimal.TryParse(parts[2].Trim(), Inv, out decimal montant))
                {
                    WriteFile(Fmt(line, "montant erronée"), pathS);
                    continue;
                }
                if (!int.TryParse(parts[3].Trim(), out int idExpe))
                {
                    WriteFile(Fmt(line, "expediteur erronée"), pathS);
                    continue;
                }
                if (!int.TryParse(parts[4].Trim(), out int idDest))
                {
                    WriteFile(Fmt(line, "destinataire erronée"), pathS);
                    continue;
                }


                if (seen.Contains(id)) continue;
                if (bank.AddTransaction(id, date, montant, idExpe, idDest))
                {
                    seen.Add(id);
                }
                else
                {
                    WriteFile(Fmt(line, "Compte non trouver"), pathS);
                    continue;
                }
            }
            return true;
        }

        public static void WriteFile(string line, string path)
        {
            if (!HeaderDone)
            {
                File.WriteAllText(path, "id;date;montant;expediteur;reception\n");
                HeaderDone = true;
            }
            File.AppendAllText(path, $"{line}\n");
        }
    }
}
