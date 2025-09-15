using Or.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

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
        private static readonly TimeSpan Window = TimeSpan.FromDays(10);

        public Carte(long id, string prenom, string nom, decimal plafondMax = 0)
        {
            Id = id;
            PrenomClient = prenom;
            NomClient = nom;
            Plafond = plafondMax == 0 ? 500 : plafondMax;
            ListComptesId = new List<int>();
            Historique = new List<Transaction>();
        }

        public decimal SoldeCarteActuel(DateTime now, long numCarte)
        {
            var carte = SqlRequests.InfosCarte(numCarte);
            List<Transaction> trans = SqlRequests.ListeTransactionsAssociesCarte(numCarte);
            var min = now - Window;
            decimal plafondcarte = carte.Plafond; 
            foreach (Transaction t in trans) {
                if (t.Horodatage >= min && t.Horodatage <= now && t.Destinataire == 0)
                {
                    plafondcarte -= t.Montant;   
                }
            }
            return plafondcarte;
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
        public CodeResultat EstRetraitAutoriseNiveauCarte(Transaction transaction, Compte Expediteur, Compte Destinataire)
        {
            CodeResultat result = EstOperationAutoriseeContraintesComptes(Expediteur, Destinataire);
            if (result == CodeResultat.Ok)
            {
                return EstEligibleMaximumRetraitHebdomadaire(transaction.Montant, transaction.Horodatage);
            }
            else return result;
            
        }

        /// <summary>
        /// Test d'éligibilité par rapport au plafond maximal de la carte
        /// </summary>
        /// <param name="montant"></param>
        /// <param name="dateEffet"></param>
        /// <returns></returns>
        /// <summary>
        /// Test d'éligibilité par rapport au plafond maximal de la carte
        /// </summary>
        /// <param name="montant"></param>
        /// <param name="dateEffet"></param>
        /// <returns></returns>
        private CodeResultat EstEligibleMaximumRetraitHebdomadaire(decimal montant, DateTime dateEffet)
        {
            List<Transaction> retraitsHisto = Historique.Where(x => (x.Horodatage > dateEffet.AddDays(-10)) && ListComptesId.Contains(x.Expediteur)).Select(x => x).ToList();
            decimal sommeHisto = montant + retraitsHisto.Sum(x => x.Montant);
            if (sommeHisto < Plafond)
            {
                return CodeResultat.Ok; 
            }else return CodeResultat.PlafondMaxDepasse;
             
        }

        /// <summary>
        /// Est-ce que les contraintes sur les comptes bancaires sont respectées ? 
        /// </summary>
        /// <param name="Expediteur"></param>
        /// <param name="Destinataire"></param>
        /// <returns></returns>
        private CodeResultat EstOperationAutoriseeContraintesComptes(Compte Expediteur, Compte Destinataire)
        {
            // Est-ce que la transaction demandée est possible ?
            if (Tools.EstTransactionExterieure(Expediteur.Id, Destinataire.Id))
            {
                return CodeResultat.MemeCompteInterdit;
            }

            // Opération Interne 
            if (EstOperationInterne(Expediteur.Id, Destinataire.Id))
            {
                return CodeResultat.Ok;
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
        private CodeResultat EstOperationExterneAutorise(Compte Expediteur, Compte Destinataire)
        {
            Operation operation = Tools.TypeTransaction(Expediteur.Id, Destinataire.Id);

            if (operation == Operation.InterCompte && Expediteur.TypeDuCompte == TypeCompte.Courant && Destinataire.TypeDuCompte == TypeCompte.Courant)
            {
                return CodeResultat.Ok;
            }
            else return CodeResultat.VirementVersLivretAutreCarteInterdit;
        }

    }
}