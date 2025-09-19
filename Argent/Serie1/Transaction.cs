namespace Argent.Serie1
{
    public class Transaction
    {
        // Pense à mettre une majuscule pour les propriétés publiques
        public int IdTransact { get; }
        public DateTime Date { get; private set; }
        public decimal Montant {  get; private set; }
        public int SenderId { get; } 
        public int RecipientId { get; }
        public Transaction(int idTransact, DateTime date, decimal montant, int senderId, int recipientId)
        {
            IdTransact = idTransact;
            Date = date;
            Montant = montant;
            SenderId = senderId;
            RecipientId = recipientId;
        }

    }
}
