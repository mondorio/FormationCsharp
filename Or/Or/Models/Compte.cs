using System.Collections.Generic;
using Or.Business;
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
        public CodeResultat EstRetraitValide(Transaction transaction)
        {
            return EstRetraitAutorise(transaction.Montant);
            /*if (EstRetraitAutorise(transaction.Montant))
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }

        private CodeResultat EstRetraitAutorise(decimal montant)
        {
            if (montant <= 0) return CodeResultat.MontantNegatifOuZero;
            else if  (Solde < montant) return CodeResultat.SoldeInsuffisant;
            else return CodeResultat.Ok;
        }

    }
}
