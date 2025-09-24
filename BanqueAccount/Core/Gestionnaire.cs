using BanqueAccount.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanqueAccount.Core
{
    public class Gestionnaire
    {

        public int Id { get; }
        public CompteType Type { get; }
        ///nombre de transactions max
        public int NbrTransactionsCap { get; }
        /// Total cumulé des frais de gestion perçus 
        public decimal FraisTotal { get; private set; }

        private readonly HashSet<int> comptes = new();
        public Gestionnaire(int id, CompteType type, int nbrTransactionsCap )
        {
            Id = id;
            Type = type;
            NbrTransactionsCap = nbrTransactionsCap;
            FraisTotal = 0;
        }
        public bool Possede(int compteId) => comptes.Contains(compteId);
        public void AjouterCompte(Compte c) { comptes.Add(c.idCpt); c.managerId = Id; }
        public void RetirerCompte(Compte c) { comptes.Remove(c.idCpt); }

        public decimal CalculerFrais(decimal montant)
            => Type == CompteType.Particulier ? decimal.Round(montant * 0.01m, 2) : 10m;

        public void EnregistrerFrais(decimal f) { if (f > 0) FraisTotal += f; }

    }
}
