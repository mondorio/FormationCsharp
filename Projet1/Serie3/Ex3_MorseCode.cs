using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie3
{
    public class Morse
    {
        private const string Taah = "===";
        private const string Ti = "=";
        private const char Point = '.'; //Séparation entre des impulsions : . ou ..
        private const string PointLetter = "..."; // Séparation entre deux lettres : ... ou ....
        private const string PointWord = "....."; // Séparation entre deux mots : ..... ou plus.

        private readonly Dictionary<string, char> _alphabet;
        private readonly Dictionary<char, string> _alphabetInverse;

        public Morse()
        {
            _alphabet = new Dictionary<string, char>()
            {
                {$"{Ti}.{Taah}", 'A'},
                {$"{Taah}.{Ti}.{Ti}.{Ti}", 'B'},
                {$"{Taah}.{Ti}.{Taah}.{Ti}", 'C'},
                {$"{Taah}.{Ti}.{Ti}", 'D'},
                {$"{Ti}", 'E'},
                {$"{Ti}.{Ti}.{Taah}.{Ti}", 'F'},
                {$"{Taah}.{Taah}.{Ti}", 'G'},
                {$"{Ti}.{Ti}.{Ti}.{Ti}", 'H'},
                {$"{Ti}.{Ti}", 'I'},
                {$"{Ti}.{Taah}.{Taah}.{Taah}", 'J'},
                {$"{Taah}.{Ti}.{Taah}", 'K'},
                {$"{Ti}.{Taah}.{Ti}.{Ti}", 'L'},
                {$"{Taah}.{Taah}", 'M'},
                {$"{Taah}.{Ti}", 'N'},
                {$"{Taah}.{Taah}.{Taah}", 'O'},
                {$"{Ti}.{Taah}.{Taah}.{Ti}", 'P'},
                {$"{Taah}.{Taah}.{Ti}.{Taah}", 'Q'},
                {$"{Ti}.{Taah}.{Ti}", 'R'},
                {$"{Ti}.{Ti}.{Ti}", 'S'},
                {$"{Taah}", 'T'},
                {$"{Ti}.{Ti}.{Taah}", 'U'},
                {$"{Ti}.{Ti}.{Ti}.{Taah}", 'V'},
                {$"{Ti}.{Taah}.{Taah}", 'W'},
                {$"{Taah}.{Ti}.{Ti}.{Taah}", 'X'},
                {$"{Taah}.{Ti}.{Taah}.{Taah}", 'Y'},
                {$"{Taah}.{Taah}.{Ti}.{Ti}", 'Z'},
            };

            _alphabetInverse = _alphabet.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        public int LettersCount(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0;

            string[] words = code.Split(new[] { PointWord }, StringSplitOptions.RemoveEmptyEntries);
            int total = 0;
            foreach (string w in words)
            {
                string[] letters = w.Split(new[] { PointLetter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string l in letters)
                {
                    string letter = l.Trim(Point);
                    if (letter.Length > 0) total++;
                }
            }
            return total;
        }

        public int WordsCount(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0;

            string[] words = code.Split(new[] { PointWord }, StringSplitOptions.RemoveEmptyEntries);
            int total = 0;
            foreach (string w in words)
            {
                total++;
            }
            return total;
        }

        public string MorseTranslation(string code)
        {
            if (string.IsNullOrEmpty(code)) return "ligne d'entrée vide ou null";

            StringBuilder sb = new StringBuilder();
            string[] words = code.Split(new[] { PointWord,  }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string w in words)
            {
                sb.Append(' '); //pour les espaces entre les mots
                string[] letters = w.Split(new[] { PointLetter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string l in letters)
                {
                    string letter = l.Trim(Point);
                    if (_alphabet.TryGetValue(letter, out char ch)) sb.Append(_alphabet[letter]);//si on trouve la lettre dans le dico on l'ajoute.
                    else sb.Append("+");
                }
            }

            return sb.ToString().Trim();
        }

        public string EfficientMorseTranslation(string code)
        {
            if (string.IsNullOrEmpty(code)) return "ligne d'entrée vide ou null";

            StringBuilder sb = new StringBuilder();
            string[] words = code.Split(new[] { PointWord }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string w in words)
            {
                sb.Append(' '); //pour les espaces entre les mots
                string[] letters = w.Split(new[] { PointLetter  }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string l in letters)
                {
                    string letter = l.Trim('.');
                    while (letter.Contains(".."))
                        letter = letter.Replace("..", ".");

                    if (_alphabet.TryGetValue(letter, out char ch)) sb.Append(_alphabet[letter]); //si on trouve la lettre dans le dico on l'ajoute.
                    else sb.Append("+");
                }
            }

            return sb.ToString().Trim(' ', '.');
        }

        /// <summary>
        /// encription d'un message morse.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string MorseEncryption(string sentence)
        {
            //string text = "JE SUIS DU MORSE";
            // =.===.===.===...=.......=.=.=...=.=.===...=.=...=.=.=.......===.=.=...=.=.===.......===.===...===.===.===...=.===.=...=.=.=...=
            //  J              E         S      U         I      S            D        U              M           O           R        S     E


            if (string.IsNullOrEmpty(sentence)) return "ligne d'entrée vide ou null";

            StringBuilder sb = new StringBuilder();
            sentence = sentence.ToUpper();

            for (int i = 0; i < sentence.Length; i++)
            {
                char c = sentence[i];

                if (c == ' ')
                {
                    sb.Append(".......");
                }
                else if (_alphabetInverse.TryGetValue(c, out string morse))
                {
                    if (sb.Length > 0 && !sb.ToString().EndsWith(".......")) sb.Append("..."); 

                    sb.Append(morse);
                }
                else
                {
                    if (sb.Length > 0 && !sb.ToString().EndsWith(".......")) sb.Append("...");

                    sb.Append("+");
                }
            }
            return sb.ToString();
        }
    }
}
