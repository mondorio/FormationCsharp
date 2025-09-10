using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    public class ModelPageBinding
    {
        public Titulaire Titulaire { get; set; }

        public ObservableCollection<Titulaire> ListePersonnes { get; set; }

        public string NomSelectionne { get; set; }
    }
}
