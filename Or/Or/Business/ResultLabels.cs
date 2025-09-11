using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Or.Business
{
    public enum CodeResultat
    {
        Ok = 0,

        // Entrées
        MontantInvalide,
        MontantNegatifOuZero,

        // Contraintes métier
        PlafondMaxDepasse,
        SoldeInsuffisant,
        MemeCompteInterdit,
        VirementVersLivretAutreCarteInterdit,

        // Techniques
        CompteIntrouvable,
        CarteIntrouvable,

        ErreurInconnue
    }

    public static class ResultLabels
    {
        public static string Label(CodeResultat code)  
        {
            switch (code)
            {
                case CodeResultat.Ok: return  "Opération réalisée avec succès.";
                case CodeResultat.MontantInvalide: return "Montant invalide.";
                case CodeResultat.MontantNegatifOuZero: return "Le montant doit être strictement positif.";
                case CodeResultat.PlafondMaxDepasse : return "Plafond maximal autorisé dépassé.";
                case CodeResultat.SoldeInsuffisant: return "Solde insuffisant.";
                case CodeResultat.MemeCompteInterdit: return "Impossible d’effectuer un virement vers le même compte.";
                case CodeResultat.VirementVersLivretAutreCarteInterdit: return "Virement vers Livret d’une autre carte interdit.";
                case CodeResultat.CompteIntrouvable: return "Compte introuvable.";
                case CodeResultat.CarteIntrouvable: return "Carte introuvable.";
                default: return "Erreur inconnue.";
            };
        }
    }
}
