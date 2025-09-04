using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Serie3
{
    public class Cesar
    {
        private readonly char[,] _cesarTable;
        private readonly char[] Alphab;
        bool found;
        StringBuilder line2 = new StringBuilder();

        public Cesar()
        {
            Alphab = new char[]
            {
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'U',
                'V',
                'W',
                'T',
                'X',
                'Y',
                'Z',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'X',
                'Y',
                'Z'
            };

            _cesarTable = new char[,]
            {
                { 'A', 'D' },
                { 'B', 'E' },
                { 'C', 'F' },
                { 'D', 'G' },
                { 'E', 'H' },
                { 'F', 'I' },
                { 'G', 'J' },
                { 'H', 'K' },
                { 'I', 'L' },
                { 'J', 'M' },
                { 'K', 'N' },
                { 'L', 'O' },
                { 'M', 'P' },
                { 'N', 'Q' },
                { 'O', 'R' },
                { 'P', 'S' },
                { 'Q', 'T' },
                { 'R', 'U' },
                { 'S', 'V' },
                { 'T', 'W' },
                { 'U', 'X' },
                { 'V', 'Y' },
                { 'W', 'Z' },
                { 'X', 'A' },
                { 'Y', 'B' },
                { 'Z', 'C' }
            };
        }

        /// <summary>
        /// cryptage césar : on parcours notre table césar pour chaque charactère et change la chaine de charactère.
        /// décalage de 3
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string CesarCode(string line)
        {
            if (string.IsNullOrEmpty(line)) return string.Empty;

            var sb = new StringBuilder();

            foreach (char c in line.ToUpper())
            {

                bool found = false;
                for (int i = 0; i < _cesarTable.GetLength(0); i++)
                {
                    if (_cesarTable[i, 0] == c)
                    {
                        sb.Append(_cesarTable[i, 1]); 
                        found = true;
                        break;
                    }
                }
                if (!found)
                    sb.Append(c); 
            }
            return sb.ToString();
        }

        /// <summary>
        /// décryptage du code cesar
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string DecryptCesarCode(string line)
        {
            if (string.IsNullOrEmpty(line)) return string.Empty;

            var sb = new StringBuilder();

            foreach (char c in line.ToUpper())
            {

                bool found = false;
                for (int i = 0; i < _cesarTable.GetLength(0); i++)
                {
                    if (_cesarTable[i, 1] == c)
                    {
                        sb.Append(_cesarTable[i, 0]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    sb.Append(c);
            }
            return sb.ToString();

        }

        public string GeneralCesarCode(string line, int x)
        {
            if (string.IsNullOrEmpty(line)) return "ligne d'entrée vide ou null";
            if (x < 0 || x > 26) return "impossible le numéro ne peut étre qu'entre 1 et 25";

            var sb = new StringBuilder();

            foreach (char c in line.ToUpper())
            {

                bool found = false;
                for (int i = 0; i < Alphab.GetLength(0); i++)
                {
                    if (Alphab[i] == c)
                    {
                        sb.Append(Alphab[i+x]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public string GeneralDecryptCesarCode(string line, int x)
        {
            if (string.IsNullOrEmpty(line)) return "ligne d'entrée vide ou null";
            if (x < 0 || x > 25) return "impossible le numéro ne peut étre qu'entre 1 et 25";
        
            var sb = new StringBuilder();

            foreach (char c in line.ToUpper())
            {

                bool found = false;
                for (int i = 0; i < Alphab.GetLength(0); i++)
                {
                    if (Alphab[i] == c)
                    {
                        sb.Append(Alphab[i - x]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
