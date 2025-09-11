using Argent.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    public class Compte
    {
        public int idCpt {  get; }
        public long numcarte { get; private set; }
        public AccountType type { get; private set; }
        public decimal solde { get; private set; } = 0;

        public Compte(int idCpt, long numcarte, AccountType type, decimal solde)
        {
            this.idCpt = idCpt;
            this.numcarte = numcarte;
            this.type = type;
            this.solde = solde;
        }
        //verif que l'on peut bien retirer de l'argent
        public bool CanWithdraw(decimal amount) => amount > 0 && solde >= amount;
        //ajoute d'argent
        public void Deposit(decimal amount) => solde += amount;
        //retrait
        public void Withdraw(decimal amount) => solde -= amount;



    }
}
