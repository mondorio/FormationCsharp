using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanqueAccount.Core
{
    public class Transaction
    {
        public int idTransact { get; }
        public DateTime date { get; private set; }
        public decimal montant {  get; private set; }
        public int senderId { get; } 
        public int recipientId { get; }
        public Transaction(int idTransact, DateTime date, decimal montant, int senderId, int recipientId)
        {
            this.idTransact = idTransact;
            this.date = date;
            this.montant = montant;
            this.senderId = senderId;
            this.recipientId = recipientId;
        }

    }
}
