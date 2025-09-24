using Argent.Enum;

namespace Argent.Serie1
{
    public class Compte
    {
        public int IdCpt {  get; }
        public long Numcarte { get; private set; }
        public AccountType Type { get; private set; }
        public decimal Solde { get; private set; } = 0;

        public Compte(int idCpt, long numcarte, AccountType type, decimal solde)
        {
            IdCpt = idCpt;
            Numcarte = numcarte;
            Type = type;
            Solde = solde;
        }
        //verif que l'on peut bien retirer de l'argent
        public bool CanWithdraw(decimal amount) => amount > 0 && Solde >= amount;
        //ajoute d'argent
        public void Deposit(decimal amount) => Solde += amount;
        //retrait
        public void Withdraw(decimal amount) => Solde -= amount;



    }
}
