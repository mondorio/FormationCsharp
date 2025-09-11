using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    public class Carte
    {
        public long idCarte { get; }
        public int plafond { get; private set; }
        public List<Compte> compteList { get; private set; } = new();

        public Carte(long idCarte, int plafond)
        {
            this.idCarte = idCarte;
            this.plafond = plafond;
        }
        /// <summary>
        /// ajoute un compte a notre liste
        /// </summary>
        /// <param name="compte"></param>
        public void AddCpt(Compte compte) => compteList.Add(compte);

    }

}

