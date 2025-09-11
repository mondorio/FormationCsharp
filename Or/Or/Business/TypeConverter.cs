using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Or.Business
{
    internal class TypeConverter : IValueConverter
    {

        private static readonly CultureInfo EuroCulture = new CultureInfo("fr-FR");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Operation op)
            {
                switch (op)
                {
                    case Operation.DepotSimple: return "Dépôt";
                    case Operation.RetraitSimple: return "Retrait";
                    case Operation.InterCompte: return "Virement";
                    default: return "Inconnu";
                }
            }
            return "Inconnu";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException(); // pas besoin pour l’affichage
    }
}
