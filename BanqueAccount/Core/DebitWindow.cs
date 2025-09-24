using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanqueAccount.Core
{
    public sealed class DebitWindow
    {
        private const int WindowSize = 7;
        private readonly List<(DateTime date, decimal amount)> debits = new();
        private readonly Queue<decimal> lastDebits = new(); //liste par first in first out
        private decimal sum = 0m;     // somme des débits dans la fenêtre de dates

        /// <summary>
        /// verifie que l'on peut faire une transaction
        /// </summary>
        /// <param name="now"></param>
        /// <param name="amount"></param>
        /// <param name="plafond"></param>
        /// <returns></returns>
        public bool CanDebit(DateTime dateEffet, decimal amount, int nDernieres)
        {
            if (amount <= 0) return false;

            // 
            decimal sumN = debits
                .OrderByDescending(d => d.date)
                .Take(nDernieres > 0 ? nDernieres - 1 : 0)
                .Sum(d => d.amount);
            if (sumN + amount > 1000m) return false;

            // 
            var debutSemaine = dateEffet.AddDays(-7);
            decimal sumWeek = debits
                .Where(d => d.date > debutSemaine && d.date <= dateEffet)
                .Sum(d => d.amount);
            if (sumWeek + amount > 2000m) return false;

            return true;
        }

        /// <summary>
        ///  on enregistre uniquement les transaction OK
        /// </summary>
        /// <param name="date">date transaction</param>
        /// <param name="amount">montant de la transaction</param>
        public void Record(DateTime dateEffet, decimal amount)
        {
            if (amount <= 0) return;
            debits.Add((dateEffet, amount));
        }

        /// <summary>
        /// récupére le plafond encore possible en fonction des 10 dérnier jour
        /// </summary>
        /// <param name="now"></param>
        public (decimal resteN, decimal resteSemaine) Reste(DateTime dateEffet, int nDernieres)
        {
            decimal sumN = debits.OrderByDescending(d => d.date).Take(nDernieres > 0 ? nDernieres : 0).Sum(d => d.amount);
            var debutSemaine = dateEffet.AddDays(-7);
            decimal sumW = debits.Where(d => d.date > debutSemaine && d.date <= dateEffet).Sum(d => d.amount);
            return (Math.Max(0, 1000m - sumN), Math.Max(0, 2000m - sumW));
        }

        public void Reset()
        {
            debits.Clear();
        }
    }
}
