using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie4
{
    public static class ClassCouncil
    {
        public struct Notes
        {
            public string nom;
            public string matiere;
            public int note;
        }
        static List<Notes> notes = new List<Notes>();

        public static void SchoolMeans(string input, string output) 
        {

            CalcMoyenne(ReadFileCsv(input), output);

        }

        /// <summary>
        /// lecture du fichier csv
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Notes> ReadFileCsv(string path)
        {
            //chemain   : C:\INTM\FormationCsharp\Projet1\Serie4\notes.csv
            // ..\..\Serie4\notes.csv

            if (File.Exists(path))
            {
                try
                {
                    // Open the stream and read it back.
                    using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine()?.Trim();

                            string[] values = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            notes.Add(new Notes { nom = values[0], matiere = values[1], note = int.Parse(values[2]) });

                        }
                    }
                    return notes;
                }
                catch (ArgumentException ae)
                {
                    Console.Write("Caractères non conformes dans le chemin daccès.");
                    Console.WriteLine(", as expected: {0}", ae.ToString());
                }
                catch (Exception e)
                {
                    Console.Write("error while reading file.");
                    Console.WriteLine(", as expected: {0}", e.ToString());

                }

            }
            return notes;
        }

        /// <summary>
        ///  calcul de la moyenne et ecriture sur le fichier
        /// </summary>
        /// <param name="notes"></param>
        public static void CalcMoyenne(List<Notes> notes, String output)
        {
            if (notes.Count == 0) return;

            BubbleSortParMatiere(notes); //trie ma liste de notes
            try
            {
                using (StreamWriter sw = new StreamWriter(output, false)) // false : overwrite ; true : ajoute a la fin
                {
                    sw.WriteLine("Matiere;Moyenne"); //en tête du csv

                    string currentMatiere = notes[0].matiere;
                    int somme = 0;
                    int count = 0;

                    for (int i = 0; i < notes.Count; i++)
                    {
                        
                        //si la matière a changer
                        if (!currentMatiere.Equals(notes[i].matiere) || i == notes.Count)
                        {
                            double moyenne = somme / (double)count;
                            sw.WriteLine($"{currentMatiere};{moyenne:0.00}");

                            // reset pour la nouvelle matière
                            currentMatiere = notes[i].matiere;
                            somme = 0;
                            count = 0;
                        }
                        somme += notes[i].note;
                        count++;
                    }

                    // dernier groupe
                    double derniere = somme / (double)count;
                    sw.WriteLine($"{currentMatiere};{derniere:0.00}");
                }
            }
            catch (ArgumentException ae)
            {
                Console.Write("Caractères non conformes dans le chemin daccès.");
                Console.WriteLine(", as expected: {0}", ae.ToString());
            }
            catch (Exception e)
            {
                Console.Write("error while reading file.");
                Console.WriteLine(", as expected: {0}", e.ToString());

            }

        }

        /// <summary>
        /// trie bulles par matière
        /// </summary>
        /// <param name="notes"></param>
        public static void BubbleSortParMatiere(List<Notes> notes)
        {
            for (int i = 0; i < notes.Count - 1; i++)
            {
                bool swapped = false;
                for (int j = 0; j < notes.Count - 1 - i; j++)
                {
                    if (string.Compare(notes[j].matiere, notes[j + 1].matiere, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Notes temp = notes[j];
                        notes[j] = notes[j + 1];
                        notes[j + 1] = temp;
                        swapped = true;
                    }
                }
                if (!swapped) break;
            }
        }
    }
}
