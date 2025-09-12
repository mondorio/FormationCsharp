using Or.Business;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Or.Models
{
    public class Transaction
    {
        public int IdTransaction { get; set; }
        public DateTime Horodatage { get; set; }
        public decimal Montant { get; set; }
        public int Expediteur { get; set; }
        public int Destinataire { get; set; }
        public Operation Type { get; set; }

        public Transaction(int idTransaction, DateTime horodatage, decimal montant, int expediteur, int destinataire)
        {
            IdTransaction = idTransaction;
            Horodatage = horodatage;
            Montant = montant;
            Expediteur = expediteur;
            Destinataire = destinataire;
            Type = Tools.TypeTransaction(expediteur,destinataire);
        }

        public Transaction(int idTransaction, DateTime horodatage, Operation type, decimal montant, int expediteur, int destinataire)
        {
            IdTransaction = idTransaction;
            Horodatage = horodatage;
            Montant = montant;
            Expediteur = expediteur;
            Destinataire = destinataire;
            Type = type;
        }

        /*public Transaction(string idTransaction, string horodatage, string type, string montant, string expediteur, string destinataire)
        {
            if (idTransaction != null && !int.TryParse(idTransaction, out int idt))  IdTransaction = idt;
            if (horodatage != null && !DateTime.TryParseExact(horodatage, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateT)) Horodatage = dateT;
            if (type != null && !Operation.TryParse(type, out Operation typeT)) Type = typeT;
            if (montant != null && !decimal.TryParse(montant, out decimal mont)) Montant = mont;
            if (expediteur != null && !int.TryParse(expediteur, out int CompteExpediteurT)) Expediteur = CompteExpediteurT;
            if (destinataire != null && !int.TryParse(destinataire, out int ComptedestinataireT)) Destinataire = ComptedestinataireT;
        }*/
    }
}
