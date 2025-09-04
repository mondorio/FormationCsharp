using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serie3
{
    public static class AdministrativeTasks
    {
        static string caviardage = "";
        /// <summary>
        /// caviarde les terms prohibé par macaron
        /// </summary>
        /// <param name="text"></param>
        /// <param name="prohibitedTerms"></param>
        /// <returns></returns>
        public static string EliminateSeditiousThoughts(string text, string[] prohibitedTerms)
        {
            if (string.IsNullOrEmpty(text) || prohibitedTerms == null) return string.Empty;
            foreach (string term in prohibitedTerms)
            {
                caviardage = new string('X', term.Length);
                text = text.Replace(term, caviardage);
            }

            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool ControlFormat(string line)
        {
            
            if (line == null || line.Length != 30) return false;

            string civ = line.Substring(0, 4);
            string nom = line.Substring(4, 12);
            string pren = line.Substring(16, 12);
            string age = line.Substring(28, 2);

            bool civOk = false;
            if (civ == "M.  " || civ == "Mme " || civ == "Mlle")
            {
                civOk = true;
            }
            bool nomOk = nom.Trim().All(char.IsLetter);
            bool preOk = pren.Trim().All(char.IsLetter);
            bool ageOk = age.Trim().All(char.IsDigit);

            return civOk && nomOk && preOk && ageOk;
        }

        public static string ChangeDate(string report)
        {
            if (string.IsNullOrEmpty(report)) return string.Empty;

            string[] parts = report.Split(' ');

            for (int i = 0; i < parts.Length; i++)
            {       // éssaie de convertir chaque morceau en dateTime si ça marche convertie la date en un autre format. 
                if (DateTime.TryParseExact(parts[i], "yyyy-MM-dd",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime date))
                {
                    // JJ.MM.AAAA
                    parts[i] = date.ToString("dd.MM.yy");
                }
            }
            return string.Join(" ", parts);
        }
    }
}
