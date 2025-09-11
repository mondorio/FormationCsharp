using Argent.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Argent.Serie1
{
    public class Banque
    {
        private List<Carte> cards = new();   
        private List<Compte> accounts = new();
        private List<Transaction> transactions = new();
        //private Dictionary<long, DebitWindow> debit = new(); //liste de debit par numero de carte

        public Banque()
        {

        }

        /// <summary>
        /// ajoute les carte a notre liste et verifie qu'il n'est pas déja crée
        /// </summary>
        /// <param name="number"></param>
        /// <param name="plafond"></param>
        /// <returns></returns>
        public bool AddCard(long number, int plafond)
        {
            Carte card = new Carte(number, plafond);
            if (cards.Contains(card)) return false;
            else cards.Add(card);
            return true;
        }
        /// <summary>
        /// affiche nos carte 
        /// </summary>
        public void afficheCarte()
        {
            foreach(var carte in cards)
            {
                Console.WriteLine(carte.idCarte + " " + carte.plafond);
            }
        }
        /// <summary>
        /// ajoute un compte a notre liste et vérifie qu'il est conforme au attente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardNumber"></param>
        /// <param name="type"></param>
        /// <param name="initialBalance"></param>
        /// <returns></returns>
        public bool AddAccount(int id, long cardNumber, AccountType type, decimal initialBalance)
        {
            if (id <= 0 || initialBalance < 0) return false;
            if (GetCompteById(id) is not null) return false;        // id compte déjà pris ?

            var carte = GetCarteByNumero(cardNumber);               // retrouver la carte par son numéro
            if (carte is null) return false;                        // la carte doit exister

            var acc = new Compte(id, cardNumber, type, initialBalance );
            accounts.Add(acc);
            carte.AddCpt(acc);                                      
            return true;
        }

        /// <summary>
        /// affiche les comptes
        /// </summary>
        public void afficheAccounts()
        {
            foreach (var account in accounts)
            {
                Console.WriteLine(account.idCpt + " " + account.numcarte + " " + account.type + " " + account.solde);
            }
        }

        /// <summary>
        /// ajoute une transaction a notre liste et vérifie qu'il est conforme au attente .
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="montant"></param>
        /// <param name="senderId"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        public bool AddTransaction(int id, DateTime date, decimal montant, int senderId, int recipientId)
        {
            if (id <= 0) return false;
            // si sender recip 0 ou trouver true
            bool senderOk = senderId == 0 || GetCompteById(senderId) is not null;  
            bool recipientOk = recipientId == 0 || GetCompteById(recipientId) is not null;
            if (!senderOk || !recipientOk) return false;

            var transac = new Transaction(id, date, montant, senderId, recipientId);
            transactions.Add(transac);
            return true;
        }
        /// <summary>
        /// affichage transac
        /// </summary>
        public void afficheTransactios()
        {
            foreach (var transac in transactions)
            {
                Console.WriteLine(transac.idTransact + " " + transac.date + " " + transac.montant + " " + transac.recipientId + " " + transac.senderId);
            }
        }
        /// <summary>
        /// gestion des transaction 
        /// </summary>
        /// <param name="pathSortie">chemain du fichier de sortie</param>
        /// <returns></returns>
        public List<string> TraiterTransactions(string pathSortie)
        {   //pour optimiser la recherche on crée un dictionnaire avec les listes key : id
            var compteById = accounts.ToDictionary(a => a.idCpt);
            var carteByNum = cards.ToDictionary(c => c.idCarte);

            var results = new List<string>();
            var seen = new HashSet<int>();
            //fabrique la ligne pour le csv (a retiré pour le final)
             string Fmt(Transaction t, bool ok, string reason = "")
                //=> $"{t.idTransact}:{(ok ? "OK" : "KO")};{t.date:dd/MM/yyyy HH:mm:ss};" +
                //   $"{t.montant};{t.senderId};{t.recipientId};{reason}";
                => $"{t.idTransact}:{(ok ? "OK" : "KO")};";


            foreach (var transaction in transactions.OrderBy(t => t.date))
            {
                bool ok = false;
                string reason = "";

                // 0001 - validations pour être sur
                //doublon
                if (!seen.Add(transaction.idTransact))
                { results.Add(Fmt(transaction, false, "Id dupliqué")); continue; }
                //montant sup a 0
                if (transaction.montant <= 0m)
                { results.Add(Fmt(transaction, false, "Montant <= 0")); continue; }
                //pas de double 0
                if (transaction.senderId == 0 && transaction.recipientId == 0)
                { results.Add(Fmt(transaction, false, "0->0 interdit")); continue; }

                if (transaction.senderId == 0)
                {
                    // 0002 - DEPOT: 0 -> Compte
                    if (compteById.TryGetValue(transaction.recipientId, out var recp))
                    {
                        recp.Deposit(transaction.montant);
                        ok = true;
                    }
                    else reason = "Destinataire inexistant";
                }
                else if (transaction.recipientId == 0)
                {
                    // 0003 - RETRAIT: Compte -> 0 (solde + plafond 10j)
                    if (!compteById.TryGetValue(transaction.senderId, out var send))
                        reason = "Expéditeur inexistant";
                    else if (!carteByNum.TryGetValue(send.numcarte, out var sCard))
                        reason = "Carte expéditeur introuvable";
                    else if (!send.CanWithdraw(transaction.montant))
                        reason = "Solde insuffisant";
                    else if (!sCard.WindowFor().CanDebit(transaction.date, transaction.montant, sCard.plafond))
                        reason = "Plafond 10j dépassé";
                    else
                    {
                        send.Withdraw(transaction.montant);
                        sCard.WindowFor().Record(transaction.date, transaction.montant);
                        ok = true;
                    }
                }
                else
                {
                    // 0004 - VIREMENT: Compte -> Compte 
                    if (!compteById.TryGetValue(transaction.senderId, out var sAcc))
                        reason = "Expéditeur inexistant";
                    else if (!compteById.TryGetValue(transaction.recipientId, out var rAcc))
                        reason = "Destinataire inexistant";
                    else if (!carteByNum.TryGetValue(sAcc.numcarte, out var sCard))
                        reason = "Carte expéditeur introuvable";
                    else if (!carteByNum.TryGetValue(rAcc.numcarte, out var rCard))
                        reason = "Carte destinataire introuvable";
                    else
                    {
                        bool sameCard = sCard.idCarte == rCard.idCarte;
                        if (!sameCard && !(sAcc.type == AccountType.Courant && rAcc.type == AccountType.Courant)) //si c'est la meme carte ok sinon doit étre courant -> courant
                            reason = "Transaction autorisé uniquement entre comptes Courants";
                        else if (!sAcc.CanWithdraw(transaction.montant))
                            reason = "Solde insuffisant";
                        else if (!sCard.WindowFor().CanDebit(transaction.date, transaction.montant, sCard.plafond))
                            reason = "Plafond 10j dépassé";
                        else
                        {
                            sAcc.Withdraw(transaction.montant);
                            rAcc.Deposit(transaction.montant);
                            sCard.WindowFor().Record(transaction.date, transaction.montant);
                            ok = true;
                        }
                    }
                }

                results.Add(Fmt(transaction, ok, reason));

                //CsvIo.WriteFile(Fmt(transaction, ok, reason), pathSortie);
            }
            
            return results;
        }

        /// <summary>
        /// regarde avec l'id du compte si il existe, retourne le compte sinon null
        /// </summary>
        /// <param name="id">idCpt</param>
        /// <returns></returns>
        public Compte? GetCompteById(int id)
         => accounts.Find(c => c.idCpt == id);

        /// <summary>
        /// regarde avec l'id de la carte si il existe, retourne le compte sinon null
        /// </summary>
        /// <param name="numero">idCarte</param>
        /// <returns></returns>
        public Carte? GetCarteByNumero(long numero)
            => cards.Find(c => c.idCarte == numero);

    }
}
