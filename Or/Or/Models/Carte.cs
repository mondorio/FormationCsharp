using Or.Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Or.Models
{
    public class Carte
    {
        public long Id { get; set; }
        public decimal Plafond { get; set; }
        public string PrenomClient { get; set; }
        public string NomClient { get; set; }
        public List<int> ListComptesId { get; set; }
        public List<Transaction> Historique { get; private set; }
        
        public Carte(long id, string prenom, string nom, decimal plafondMax = 0)
        {
            Id = id;
            PrenomClient = prenom;
            NomClient = nom;
            Plafond = plafondMax == 0 ? 500 : plafondMax;
            ListComptesId = new List<int>();
            Historique = new List<Transaction>();
        }

        public void AlimenterHistoriqueEtListeComptes(List<Transaction> hist, List<int> comptesId)
        {
            ListComptesId = comptesId;
            Historique = hist;
        }

        public void AjoutTransactionValidee(Transaction transac)
        {
            Historique.Add(transac);
        }

        // -------------------------------------------------------------------------------------------------------------
        //                              Contraintes sur les retraits et virements
        // -------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Est-ce que le retrait (retrait simple, virement) est il autorisé au niveau de la carte ? 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="Expediteur"></param>
        /// <param name="Destinataire"></param>
        /// <returns></returns>
        public bool EstRetraitAutoriseNiveauCarte(Transaction transaction, Compte Expediteur, Compte Destinataire)
        {
            return EstOperationAutoriseeContraintesComptes(Expediteur, Destinataire) && EstEligibleMaximumRetraitHebdomadaire(transaction.Montant, transaction.Horodatage);
        }

        /// <summary>
        /// Test d'éligibilité par rapport au plafond maximal de la carte
        /// </summary>
        /// <param name="montant"></param>
        /// <param name="dateEffet"></param>
        /// <returns></returns>
        private bool EstEligibleMaximumRetraitHebdomadaire(decimal montant, DateTime dateEffet)
        {
            List<Transaction> retraitsHisto =
                Historique.Where(x => x.Expediteur == Id && x.Horodatage > dateEffet.AddDays(-10)).Select(x => x).ToList();
            decimal sommeHisto = montant + retraitsHisto.Sum(x => x.Montant);
            return sommeHisto < Plafond;
        }

        /// <summary>
        /// Est-ce que les contraintes sur les comptes bancaires sont respectées ? 
        /// </summary>
        /// <param name="Expediteur"></param>
        /// <param name="Destinataire"></param>
        /// <returns></returns>
        private bool EstOperationAutoriseeContraintesComptes(Compte Expediteur, Compte Destinataire)
        {
            // Est-ce que la transaction demandée est possible ?
            if (Tools.EstTransactionExterieure(Expediteur.Id, Destinataire.Id))
            {
                return false;
            }

            // Opération Interne 
            if (EstOperationInterne(Expediteur.Id, Destinataire.Id))
            {
                return true;
            }
            // Opération externe
            else
            {
                return EstOperationExterneAutorise(Expediteur, Destinataire);
            }
        }

        /// <summary>
        /// Le compte appartient-il à la carte ? 
        /// </summary>
        /// <param name="idtCpt"></param>
        /// <returns></returns>
        private bool EstComptePresent(int idtCpt)
        {
            return ListComptesId.Exists(x => x == idtCpt);
        }

        /// <summary>
        /// Est ce qu'il s'agit d'une opération interne possible en principe ? 
        /// </summary>
        /// <param name="cptExt"></param>
        /// <param name="cptDest"></param>
        /// <returns></returns>
        private bool EstOperationInterne(int cptExt, int cptDest)
        {
            Operation operation = Tools.TypeTransaction(cptExt, cptDest);
            return 
               (
                operation == Operation.DepotSimple || 
                operation == Operation.RetraitSimple || 
               (operation == Operation.InterCompte && EstComptePresent(cptExt) && EstComptePresent(cptDest))
               );
        }

        /// <summary>
        /// S'agit il d'une opération inter-compte externe possible ? 
        /// </summary>
        /// <param name="Expediteur"></param>
        /// <param name="Destinataire"></param>
        /// <returns></returns>
        private bool EstOperationExterneAutorise(Compte Expediteur, Compte Destinataire)
        {
            Operation operation = Tools.TypeTransaction(Expediteur.Id, Destinataire.Id);

            return operation == Operation.InterCompte &&
                   Expediteur.TypeDuCompte == TypeCompte.Courant && Destinataire.TypeDuCompte == TypeCompte.Courant ;
        }

    }
}