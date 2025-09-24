
using BanqueAccount.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanqueAccount.Core
{
    public class Compte
    {
        public int idCpt {  get; }
        public AccountType type { get; private set; }
        public decimal solde { get; private set; } = 0;
        public int plafond { get; private set; } = 1000;
        public int managerId { get; set; }

        public DateTime? DateCreation { get; private set; }
        public DateTime? DateResiliation { get; private set; }

        private DebitWindow debitWindow = new();  //liste de debit par numero de carte
        public Compte(int idCpt, AccountType type, decimal soldeInitial, DateTime? dateCreation = null)
        {
            this.idCpt = idCpt;
            this.type = type;
            this.solde = soldeInitial;
            this.DateCreation = dateCreation;
            this.managerId = managerId;
        }
        public bool EstActifA(DateTime d)
        {
            if (DateCreation is null) return false;
            if (d < DateCreation.Value) return false;
            if (DateResiliation is not null && d > DateResiliation.Value) return false;
            return true;
        }

        //verif que l'on peut bien retirer de l'argent
        public bool CanWithdraw(decimal amount) => amount > 0 && amount <= plafond && solde >= amount;
        //ajoute d'argent
        public void Deposit(decimal amount) => solde += amount;
        //retrait
        public void Withdraw(decimal amount) => solde -= amount;

        public void MarquerCreation(DateTime d, int managerId) { DateCreation = d; this.managerId = managerId; }
        public void MarquerResiliation(DateTime d) { DateResiliation = d; }

        // Plafonds 
        public bool CanDebitPlafonds(DateTime dateEffet, decimal amount, int nDernieres = 7)
            => debitWindow.CanDebit(dateEffet, amount, nDernieres);

        public void RecordDebit(DateTime dateEffet, decimal amount)
            => debitWindow.Record(dateEffet, amount);


    }
}
