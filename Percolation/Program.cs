using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolation
{
    class Program
    {
        static void Main(string[] args)
        {

            int n = 20;
            int t = 100;

            var sim = new PercolationSimulation
            {
                Visualize = true,
                DelayMs = 10
            };

            var res = sim.MeanPercolationValue(n, t);

            Console.WriteLine();
            Console.WriteLine($"N={n}, t={t} -> Percolation moyenne: {res.Mean:F6}, écart-type: {res.StandardDeviation:F6}");
            Console.WriteLine("Appuie sur une touche pour quitter...");
            Console.ReadKey();
 
        }
    }
}
