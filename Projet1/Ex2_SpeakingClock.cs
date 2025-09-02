using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public static class SpeakingClock
    {
        static string message;
        public static string GoodDay(int heure) 
        {
            Console.WriteLine();
            Console.WriteLine("Ô Soleil ! Toi sans qui les choses Ne seraient que ce qu'elles sont. Edmond Rostand");


            if (heure < 0 || heure > 23)
                return "Heure invalide";


            if (heure >= 0 && heure < 6)
                message = "Merveilleuse nuit !";
            else if (heure >= 6 && heure < 12)
                message = "Bonne matinée !";
            else if (heure == 12)
                message = "Bon appétit !";
            else if (heure >= 13 && heure <= 18)
                message = "Profitez de votre après-midi !";
            else
                message = "Passez une bonne soirée !";

            return $"Il est heure {heure}, {message}";
        }
    }
}
