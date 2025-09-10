using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    public class Carte
    {
        public long idCarte { get; }
        public int plafond { get; private set; }
        public List<Compte> compteList { get; private set; } = new();

        private readonly List<(DateTime date, decimal amount)> debitList = new();
        public Carte(long idCarte, int plafond)
        {
            this.idCarte = idCarte;
            this.plafond = plafond;
        }

        public void AddCpt(Compte compte) => compteList.Add(compte);

        public void AddDebit(DateTime date, decimal amount) => debitList.Add((date, amount));

        public decimal SumDebitsInLast10Days(DateTime now)
        {
            decimal sum = 0;
            if (debitList.Count > 0)
            {
                DateTime min = now.AddDays(-10);

                foreach (var debit in debitList)
                {
                    if(debit.date >= min && debit.date <= now)
                    {
                        sum += debit.amount;
                    }
                }
                return sum;
            }
            else return -1;

        }

    }

}

