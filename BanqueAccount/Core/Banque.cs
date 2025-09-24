using BanqueAccount.Enum;

namespace BanqueAccount.Core
{
    public class Banque
    {
        private List<Compte> accounts = new();
        private List<Transaction> transactions = new();
        private readonly List<Gestionnaire> managers = new();

        private DebitWindow debit = new();  
        public Banque()
        {

        }

        /// <summary>
        /// ajoute un compte a notre liste et vérifie qu'il est conforme au attente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardNumber"></param>
        /// <param name="type"></param>
        /// <param name="initialBalance"></param>
        /// <returns></returns>
        public bool AddAccount(int id, AccountType type, decimal initialBalance)
        {
            if (id <= 0 || initialBalance < 0) return false;
            if (GetCompteById(id) is not null) return false;        // id compte déjà pris ?

            var acc = new Compte(id, type, initialBalance );
            accounts.Add(acc);                                      
            return true;
        }

        /// <summary>
        /// affiche les comptes
        /// </summary>
        public void afficheAccounts()
        {
            foreach (var account in accounts)
            {
                Console.WriteLine(account.idCpt + " " + account.type + " " + account.solde);
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

            var results = new List<string>();
            var seen = new HashSet<int>();
            //fabrique la ligne pour le csv (a retiré pour le final)
             string Fmt(Transaction t, bool ok, string reason = "")
                => $"{t.idTransact}:{(ok ? "OK" : "KO")};{t.date:dd/MM/yyyy HH:mm:ss};" +
                   $"{t.montant};{t.senderId};{t.recipientId};{reason}";
                //=> $"{t.idTransact}:{(ok ? "OK" : "KO")};";


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
                    // 0003 - RETRAIT: Compte -> 0 (solde + plafond 10t)
                    if (!compteById.TryGetValue(transaction.senderId, out var send))
                        reason = "Expéditeur inexistant";
                    else if (!send.CanWithdraw(transaction.montant))
                        reason = "Solde insuffisant";
                    else if ( send.CanDebitPlafonds(transaction.montant))
                    {
                        reason = "Plafond dépassé";
                    }
                    else
                    {
                        send.Withdraw(transaction.montant);
                        send.RecordDebit(transaction.montant);
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
                    else
                    {
                        if (!sAcc.CanWithdraw(transaction.montant))
                            reason = "Solde insuffisant";
                        else if (!sAcc.CanDebitPlafonds(transaction.montant))
                            reason = "Plafond dépassé";
                        else
                        {
                            sAcc.Withdraw(transaction.montant);
                            rAcc.Deposit(transaction.montant);
                            sAcc.RecordDebit(transaction.montant);
                            ok = true;
                        }
                    }
                }

                results.Add(Fmt(transaction, ok, reason));

                //CsvIo.WriteFile(Fmt(transaction, ok, reason), pathSortie);
            }
            
            return results;
        }

        public bool AddManager(int id, CompteType type, int nCap)
        {
            if (id <= 0 || nCap <= 0) return false;
            if (GetManager(id) is not null) return false;
            managers.Add(new Gestionnaire(id, type, nCap));
            return true;
        }

        /// <summary>
        /// regarde avec l'id du compte si il existe, retourne le compte sinon null
        /// </summary>
        /// <param name="id">idCpt</param>
        /// <returns></returns>
        public Compte? GetCompteById(int id)
         => accounts.Find(c => c.idCpt == id);
        public Gestionnaire? GetManager(int id) 
         => managers.FirstOrDefault(m => m.Id == id);

    }
}
