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
        private Dictionary<long, DebitWindow> debit;  //liste de debit par numero de carte
        public Carte(long idCarte, int plafond)
        {
            this.idCarte = idCarte;
            this.plafond = plafond;
            this.debit = new();
        }
        /// <summary>
        /// ajoute un compte a notre liste
        /// </summary>
        /// <param name="compte"></param>
        public void AddCpt(Compte compte) => compteList.Add(compte);

        /// <summary>
        /// cherche la liste dans mon dictionnaire de carte debit si il ne le trouve pas en crée un nouveau.
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public DebitWindow WindowFor()
            => debit.TryGetValue(idCarte, out var w) ? w : (debit[idCarte] = new DebitWindow());

    }

}

