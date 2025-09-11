using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    sealed class DebitWindow
    {
        private static readonly TimeSpan Window = TimeSpan.FromDays(10);
        private readonly List<(DateTime date, decimal amount)> ops = new();
        private int start = 0;        
        private decimal sum = 0m;     // somme des débits dans la fenêtre de dates

        /// <summary>
        /// verifie que l'on peut faire une transaction
        /// </summary>
        /// <param name="now"></param>
        /// <param name="amount"></param>
        /// <param name="plafond"></param>
        /// <returns></returns>
        public bool CanDebit(DateTime now, decimal amount, int plafond)
        {
            getPlafond(now);
            return sum + amount <= plafond;
        }

        /// <summary>
        ///  on enregistre uniquement les transaction OK
        /// </summary>
        /// <param name="date">date transaction</param>
        /// <param name="amount">montant de la transaction</param>
        public void Record(DateTime date, decimal amount)
        {
            ops.Add((date, amount));   
            sum += amount;
        }
        /// <summary>
        /// récupére le plafond encore possible en fonction des 10 dérnier jour
        /// </summary>
        /// <param name="now"></param>
        private void getPlafond(DateTime now)
        {
            var min = now - Window;   
            while (start < ops.Count && ops[start].date <= min)
            {
                sum -= ops[start].amount;
                start++;
            }
        }
    }
}
