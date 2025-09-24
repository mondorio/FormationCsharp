using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    public class Carte
    {
        public long IdCarte { get; }
        public int Plafond { get; private set; }
        public List<Compte> CompteList { get; private set; } = new();
        private Dictionary<long, DebitWindow> _debit;  //liste de debit par numero de carte
        public Carte(long idCarte, int plafond)
        {
            this.IdCarte = idCarte;
            this.Plafond = plafond;
            this._debit = new();
        }
        /// <summary>
        /// ajoute un compte a notre liste
        /// </summary>
        /// <param name="compte"></param>
        public void AddCpt(Compte compte) => CompteList.Add(compte);

        /// <summary>
        /// cherche la liste dans mon dictionnaire de carte debit si il ne le trouve pas en crée un nouveau.
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public DebitWindow WindowFor()
            => _debit.TryGetValue(IdCarte, out var w) ? w : (_debit[IdCarte] = new DebitWindow());

    }

}

