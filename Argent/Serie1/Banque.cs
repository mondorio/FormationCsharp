using Argent.Enum;

namespace Argent.Serie1
{
    public class Banque
    {
        // Pour les types complexes, par référence on préférera les initialiser dans le constructeur
        private List<Carte> _cards;   
        private List<Compte> _accounts;
        private List<Transaction> _transactions;
        //private Dictionary<long, DebitWindow> debit = new(); //liste de debit par numero de carte

        public Banque()
        {
            _cards = new List<Carte>();
            _accounts = new List<Compte>();
            _transactions = new List<Transaction>();
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
            if (_cards.Contains(card)) return false;
            else _cards.Add(card);
            return true;
        }
        /// <summary>
        /// affiche nos carte 
        /// </summary>
        public void AfficheCarte()
        {
            foreach(var carte in _cards)
            {
                Console.WriteLine(carte.IdCarte + " " + carte.Plafond);
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
            _accounts.Add(acc);
            carte.AddCpt(acc);                                      
            return true;
        }

        /// <summary>
        /// affiche les comptes
        /// </summary>
        public void AfficheAccounts()
        {
            foreach (var account in _accounts)
            {
                Console.WriteLine(account.IdCpt + " " + account.Numcarte + " " + account.Type + " " + account.Solde);
            }
        }

        /// <summary>
        /// Ajoute une transaction à notre liste et vérifie qu'il est conforme au attente .
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
            _transactions.Add(transac);
            return true;
        }
        /// <summary>
        /// affichage transac
        /// </summary>
        public void AfficheTransactions() // pourquoi pas DisplayTransactions() ? On évite le mélange francais anglais
        {
            foreach (var transac in _transactions)
            {
                Console.WriteLine(transac.IdTransact + " " + transac.Date + " " + transac.Montant + " " + transac.RecipientId + " " + transac.SenderId);
            }
        }
        /// <summary>
        /// gestion des transaction 
        /// </summary>
        /// <param name="pathSortie">chemain du fichier de sortie</param>
        /// <returns></returns>
        public List<string> TraiterTransactions(string pathSortie)
        {   //pour optimiser la recherche on crée un dictionnaire avec les listes key : id
            var compteById = _accounts.ToDictionary(a => a.IdCpt);
            var carteByNum = _cards.ToDictionary(c => c.IdCarte);

            var results = new List<string>();
            var seen = new HashSet<int>();
            //fabrique la ligne pour le csv (a retiré pour le final)
             string Fmt(Transaction t, bool ok, string reason = "")
                //=> $"{t.idTransact}:{(ok ? "OK" : "KO")};{t.date:dd/MM/yyyy HH:mm:ss};" +
                //   $"{t.montant};{t.senderId};{t.recipientId};{reason}";
                // Correction, tu dois avoir ID;OK ou ID;KO
                => $"{t.IdTransact};{(ok ? "OK" : "KO")}";

            // Sécurité sur l'horodatage. OK
            foreach (var transaction in _transactions.OrderBy(t => t.Date))
            {
                bool ok = false;
                string reason = "";

                // 0001 - validations pour être sur
                //doublon
                if (!seen.Add(transaction.IdTransact))
                { results.Add(Fmt(transaction, false, "Id dupliqué")); continue; }
                //montant sup a 0
                if (transaction.Montant <= 0m)
                { results.Add(Fmt(transaction, false, "Montant <= 0")); continue; }
                //pas de double 0
                if (transaction.SenderId == 0 && transaction.RecipientId == 0)
                { results.Add(Fmt(transaction, false, "0->0 interdit")); continue; }

                if (transaction.SenderId == 0)
                {
                    // 0002 - DEPOT: 0 -> Compte
                    if (compteById.TryGetValue(transaction.RecipientId, out var recp))
                    {
                        recp.Deposit(transaction.Montant);
                        ok = true;
                    }
                    else reason = "Destinataire inexistant";
                }
                else if (transaction.RecipientId == 0)
                {
                    // 0003 - RETRAIT: Compte -> 0 (solde + plafond 10j)
                    if (!compteById.TryGetValue(transaction.SenderId, out var send))
                        reason = "Expéditeur inexistant";
                    else if (!carteByNum.TryGetValue(send.Numcarte, out var sCard))
                        reason = "Carte expéditeur introuvable";
                    else if (!send.CanWithdraw(transaction.Montant))
                        reason = "Solde insuffisant";
                    else if (!sCard.WindowFor().CanDebit(transaction.Date, transaction.Montant, sCard.Plafond))
                        reason = "Plafond 10j dépassé";
                    else
                    {
                        send.Withdraw(transaction.Montant);
                        sCard.WindowFor().Record(transaction.Date, transaction.Montant);
                        ok = true;
                    }
                }
                else
                {
                    // 0004 - VIREMENT: Compte -> Compte 
                    if (!compteById.TryGetValue(transaction.SenderId, out var sAcc))
                        reason = "Expéditeur inexistant";
                    else if (!compteById.TryGetValue(transaction.RecipientId, out var rAcc))
                        reason = "Destinataire inexistant";
                    else if (!carteByNum.TryGetValue(sAcc.Numcarte, out var sCard))
                        reason = "Carte expéditeur introuvable";
                    else if (!carteByNum.TryGetValue(rAcc.Numcarte, out var rCard))
                        reason = "Carte destinataire introuvable";
                    else
                    {
                        bool sameCard = sCard.IdCarte == rCard.IdCarte;
                        if (!sameCard && !(sAcc.Type == AccountType.Courant && rAcc.Type == AccountType.Courant)) //si c'est la meme carte ok sinon doit étre courant -> courant
                            reason = "Transaction autorisé uniquement entre comptes Courants";
                        else if (!sAcc.CanWithdraw(transaction.Montant))
                            reason = "Solde insuffisant";
                        else if (!sCard.WindowFor().CanDebit(transaction.Date, transaction.Montant, sCard.Plafond))
                            reason = "Plafond 10j dépassé";
                        else
                        {
                            sAcc.Withdraw(transaction.Montant);
                            rAcc.Deposit(transaction.Montant);
                            sCard.WindowFor().Record(transaction.Date, transaction.Montant);
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
         => _accounts.Find(c => c.IdCpt == id);

        /// <summary>
        /// regarde avec l'id de la carte si il existe, retourne le compte sinon null
        /// </summary>
        /// <param name="numero">idCarte</param>
        /// <returns></returns>
        public Carte? GetCarteByNumero(long numero)
            => _cards.Find(c => c.IdCarte == numero);

    }
}
