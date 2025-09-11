using System.Collections.Generic;

namespace Or.Models
{
    public enum TypeCompte { Courant, Livret }

    public class Compte
    {
        public int Id { get; set; }
        public long IdentifiantCarte { get; set; }
        public TypeCompte TypeDuCompte { get; set; }
        public decimal Solde { get; private set; }

        public Compte(int id, long identifiantCarte, TypeCompte type, decimal soldeInitial)
        {
            Id = id;
            IdentifiantCarte = identifiantCarte;
            TypeDuCompte = type;
            Solde = soldeInitial;
        }

        /// <summary>
        /// Action de dépôt d'argent sur le compte bancaire
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>Statut du dépôt</returns>
        public bool EstDepotValide(Transaction transaction)
        {
            if (transaction.Montant > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Action de retrait d'argent sur le compte bancaire
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>Statut du retrait</returns>
        public bool EstRetraitValide(Transaction transaction)
        {
            if (EstRetraitAutorise(transaction.Montant))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EstRetraitAutorise(decimal montant)
        {
            return Solde >= montant && montant > 0;
        }

    }
}
