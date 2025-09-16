using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolation
{
    public struct PclData
    {
        /// <summary>
        /// Moyenne 
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// Ecart-type
        /// </summary>
        public double StandardDeviation { get; set; }
        /// <summary>
        /// Fraction
        /// </summary>
        public double Fraction { get; set; }
    }


    public class PercolationSimulation
    {
        public bool Visualize { get; set; } = false;
        public int DelayMs { get; set; } = 15;

        private Random rnd = new Random();

        /// <summary>
        /// test pour visualiser ce qui se passe recup sur internet
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public double PercolationValue(int size)
        {
            var p = new Percolation(size);
            int total = size * size;
            int opened = 0;

            var blocked = new List<KeyValuePair<int, int>>(total);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    blocked.Add(new KeyValuePair<int, int>(i, j));

            if (Visualize)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Console.WriteLine("Percolation ( # = bloquée, . = ouverte, o = pleine )");
                p.RecomputeFullFromTop();
                PercoRender.Draw(p);
            }

            while (!p.Percolate())
            {
                int idx = rnd.Next(blocked.Count);
                var c = blocked[idx];
                blocked.RemoveAt(idx);

                p.OpenCell(c.Key, c.Value);
                opened++;

                if (Visualize)
                {
                    // pour être sûr que l'eau apparaisse correctement, on recalcule depuis le haut
                    p.RecomputeFullFromTop();
                    PercoRender.Draw(p);
                    if (DelayMs > 0) Thread.Sleep(DelayMs);
                }
            }

            if (Visualize)
            {
                Console.SetCursorPosition(0, size + 3);
                Console.CursorVisible = true;
                Console.WriteLine($"Percolation ! cases ouvertes: {opened}/{total}  =>  {(double)opened / total:F4}");
            }

            return (double)opened / total;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public PclData MeanPercolationValue(int size, int t)
        {
            var vals = new double[t];
            for (int k = 0; k < t; k++)
            {
                bool prevVisu = Visualize;
                Visualize = (k == 0) && prevVisu;

                vals[k] = PercolationValue(size);

                Visualize = prevVisu;
            }

            double mean = vals.Average();
            double mean2 = vals.Select(v => v * v).Average();
            double var = mean2 - mean * mean;
            if (var < 0) var = 0; // éviter -epsilon
            double sigma = Math.Sqrt(var);

            return new PclData
            {
                Mean = mean,
                StandardDeviation = sigma,
                Fraction = vals[vals.Length - 1] // dernière fraction
            };
        }
    }
}
