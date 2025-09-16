using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolation
{
    static class PercoRender
    {
        // dessin : # = bloquée, . = ouverte, o = pleine
        public static void Draw(Percolation p)
        {
            int n = p.Size;
            var sb = new StringBuilder(n * (n + 1));
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    char ch;
                    if (!p.IsOpen(i, j)) ch = '#';
                    else if (p.IsFull(i, j)) ch = 'o';
                    else ch = '.';
                    sb.Append(ch);
                }
                sb.AppendLine();
            }

            // place l'affichage sous le titre
            Console.SetCursorPosition(0, 1);
            Console.Write(sb.ToString());
        }
    }
}
