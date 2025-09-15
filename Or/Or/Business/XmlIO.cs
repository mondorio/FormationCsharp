using Or.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static Or.Business.XmlIO;


namespace Or.Business
{
    public static class XmlIO
    {
        static string filePath = "C:\\INTM\\FormationCsharp\\Or\\Or\\Files\\";

        [XmlRoot("Compte")]
        public class XMLCompte
        {
            [XmlAttribute("Identifiant")]
            public int Id { get; set; }

            [XmlAttribute("Type")]
            public TypeCompte Type { get; set; }

            [XmlAttribute("Solde")]
            public string Solde { get; set; }

            [XmlArray("Transactions")]
            [XmlArrayItem("Transaction")]
            public List<XMLTransaction> Transactions { get; set; } = new List<XMLTransaction>();
        }

        public class XMLTransaction
        {
            [XmlAttribute("Identifiant")]
            public int Id { get; set; }

            // format: DD/MM/YYYY HH:mm:ss
            [XmlAttribute("Date")]
            public DateTime Date { get; set; }

            [XmlAttribute("Type")]
            public string Type { get; set; }

            // présents ou non selon le type
            [XmlAttribute("CompteExpediteur")]
            public int CompteExpediteur { get; set; }

            [XmlAttribute("CompteDestinataire")]
            public int CompteDestinataire { get; set; }

            // Montant absolu
            [XmlAttribute("Montant")]
            public string Montant { get; set; }
        }

        private static string FormatMoney(decimal amountAbs)
            => Math.Abs(amountAbs).ToString("C2");

        private static bool TryParseMoney(string s, out decimal valueAbs)
        {
            if (decimal.TryParse(s, NumberStyles.Currency, CultureInfo.CurrentCulture, out var v))
            {
                valueAbs = Math.Abs(v);
                return true;
            }
            valueAbs = 0m;
            return false;
        }


        /// <summary>
        /// Sérialise les comptes et leurs 10 dernières transactions vers un fichier XML.
        /// </summary>
        /// <param name="numCarte">numero de la carte courante</param>
        /// <returns></returns>
        public static CodeResultat SerializeComptesTransactions(long numCarte)
        {
            List<Compte> comptes = SqlRequests.ListeComptesAssociesCarte(numCarte);
            if (comptes == null) return CodeResultat.CompteIntrouvable;


            List<XMLCompte> exportList = new List<XMLCompte>();



            //on parcour nos compte
            foreach (Compte c in comptes)
            {
                XMLCompte cptXML = new XMLCompte
                {
                    Id = c.Id,
                    Type = c.TypeDuCompte,
                    Solde = c.Solde.ToString("C2")
                };

                List<Transaction> Transactions = SqlRequests.ListeTransactionsAssociesCompte(c.Id);
                if (Transactions == null) return CodeResultat.ErreurInconnue;

                var triTransac = Transactions.OrderByDescending(t => t.Horodatage).Take(10); //GetRange(0,10) prend les 10 premiers après trie
                //on parcours nos transaction pour les lier à nos comptes
                foreach (var t in triTransac)
                {
                    //creation de notre transaction
                    var TransactionXML = new XMLTransaction
                    {
                        Id = t.IdTransaction,
                        Date = t.Horodatage,
                        Type = t.Type.ToString(),
                        Montant = FormatMoney(t.Montant)
                    };

                    // Présence des attributs expéditeur/destinataire retrait ou virement
                    if (t.Type == Operation.RetraitSimple || t.Type == Operation.InterCompte)
                        TransactionXML.CompteExpediteur = t.Expediteur;

                    if (t.Type == Operation.DepotSimple || t.Type == Operation.InterCompte)
                        TransactionXML.CompteDestinataire = t.Destinataire;

                    cptXML.Transactions.Add(TransactionXML);
                }

                exportList.Add(cptXML);
            }
            try
            {
                filePath += numCarte.ToString();


                var serializer = new XmlSerializer(typeof(List<XMLCompte>));

                using (var writer = new StreamWriter(filePath, append: false, Encoding.UTF8))
                {
                    serializer.Serialize(writer, exportList);

                }
                return CodeResultat.Ok;

            }
            catch (Exception ex)
            {
                return CodeResultat.XMLExportFail;
            }
        }

        // Récupère une valeur soit en attribut, soit en élément enfant (souple)
        private static string GetAttrOrElem(XElement parent, string name)
        {
            // attribut
            var attr = parent.Attribute(name);
            if (attr != null) return attr.Value?.Trim();

            // élément enfant
            var elem = parent.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
            return elem?.Value?.Trim();
        }

        public static CodeResultat DeSerialiserTransactions(string path, long numCarte)
        {
            var listTransaction = new List<Transaction>();
            if (!File.Exists(path)) return CodeResultat.XMLNotFound;

            var fr = CultureInfo.GetCultureInfo("fr-FR");

            // charge en gardant les infos de ligne
            var xdoc = XDocument.Load(path, LoadOptions.SetLineInfo);

            // tous les comptes, peu importe les namespaces
            var cxNodes = xdoc
                .Descendants()
                .Where(e => e.Name.LocalName.Equals("XMLCompte", StringComparison.OrdinalIgnoreCase));

            foreach (var node in cxNodes)
            {
                var ligne = (IXmlLineInfo)node;

                // --- ATTRIBUTS DU COMPTE ---
                string idStr = (string)node.Attribute("Identifiant");
                string typeStr = (string)node.Attribute("Type");
                string soldeStr = (string)node.Attribute("Solde");

                // id compte (optionnel: si absent on ignore le compte)
                if (idStr != null && !int.TryParse(idStr, out int _))
                    continue;

                // type de compte
                if (typeStr != null && !TypeCompte.TryParse(typeStr, out TypeCompte _))
                    continue;

                // solde (ex: "10 732,00 €" avec espace insécable)
                if (soldeStr != null)
                {
                    var cleanedSolde = soldeStr.Replace("€", "").Trim();
                    // remplace l’insécable par un espace normal
                    cleanedSolde = cleanedSolde.Replace('\u00A0', ' ');
                    if (!decimal.TryParse(cleanedSolde, NumberStyles.Number, fr, out decimal _))
                        continue;
                }

                // --- TRANSACTIONS ---
                var txNodes = node
                    .Descendants()
                    .Where(e => e.Name.LocalName.Equals("Transaction", StringComparison.OrdinalIgnoreCase));

                foreach (var tx in txNodes)
                {
                    // Valeurs par défaut
                    int idt = 0;
                    DateTime dateT = DateTime.Now;
                    Operation typeT = Operation.InterCompte;
                    int compteExp = 0;
                    int compteDest = 0;
                    decimal montant = 0m;

                    // tous en ATTRIBUTS
                    string idTStr = (string)tx.Attribute("Identifiant");
                    string dateTStr = (string)tx.Attribute("Date");                 // ex: 2025-09-12T01:18:49
                    string typeTStr = (string)tx.Attribute("Type");
                    string expStr = (string)tx.Attribute("CompteExpediteur");
                    string destStr = (string)tx.Attribute("CompteDestinataire");
                    string montantStr = (string)tx.Attribute("Montant");              // ex: "22,00 €"

                    // id transaction
                    if (idTStr != null && !int.TryParse(idTStr, out idt))
                        continue;

                    // date ISO
                    if (dateTStr != null &&
                        !DateTime.TryParseExact(dateTStr, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out dateT))
                        continue;

                    // type opération
                    if (typeTStr != null && !Operation.TryParse(typeTStr, out typeT))
                        continue;

                    // expéditeur : si attribut absent ou "0" => 0
                    if (!string.IsNullOrEmpty(expStr) && expStr != "0")
                    {
                        if (!int.TryParse(expStr, out compteExp))
                            continue;
                    }

                    // destinataire : si attribut absent ou "0" => 0
                    if (!string.IsNullOrEmpty(destStr) && destStr != "0")
                    {
                        if (!int.TryParse(destStr, out compteDest))
                            continue;
                    }

                    // montant
                    if (!string.IsNullOrWhiteSpace(montantStr))
                    {
                        var cleaned = montantStr.Replace("€", "").Trim().Replace('\u00A0', ' ');
                        if (!decimal.TryParse(cleaned, NumberStyles.Number, fr, out montant))
                            continue;
                    }

                    listTransaction.Add(new Transaction(idt, dateT, typeT, montant, compteExp, compteDest));
                }
            }

            if (!TraitementTransactionsImportees(listTransaction, numCarte))
                return CodeResultat.XMLImportFail;

            return CodeResultat.Ok;
        }
        public static bool TraitementTransactionsImportees(List<Transaction> transactions, long numCarte)
        {
            if (transactions == null || transactions.Count == 0) return false;

            // Tri
            transactions = transactions.OrderBy(t => t.Horodatage).ToList();

            // Récupération des comptes 
            List<Compte> cpts = SqlRequests.ListeTousLesComptes();
            var byId = cpts.ToDictionary(c => c.Id, c => c);

            int nbIntegrees = 0;

            foreach (var t in transactions)
            {
                try
                {
                    // Montant strictement positif
                    if (t.Montant <= 0) continue;

                    //  Déduire l'opération si absente 
                    if (!Enum.IsDefined(typeof(Operation), t.Type) || t.Type == 0)
                    {
                        if (t.Expediteur == 0 && t.Destinataire > 0) t.Type = Operation.DepotSimple;
                        else if (t.Expediteur > 0 && t.Destinataire == 0) t.Type = Operation.RetraitSimple;
                        else if (t.Expediteur > 0 && t.Destinataire > 0) t.Type = Operation.InterCompte;
                        else continue;
                    }

                    // Même compte
                    if (t.Type == Operation.InterCompte && t.Expediteur == t.Destinataire) continue;

                    // Existence des comptes
                    Compte ex = null, de = null;
                    if (t.Expediteur > 0 && !byId.TryGetValue(t.Expediteur, out ex)) continue;
                    if (t.Destinataire > 0 && !byId.TryGetValue(t.Destinataire, out de)) continue;

                    // meme compte 
                    if (t.Destinataire == t.Expediteur) continue;

                    // Solde suffisant expéditeur 
                    bool debit = (t.Type == Operation.RetraitSimple || t.Type == Operation.InterCompte);
                    if (debit && ex != null && ex.Solde < t.Montant) continue;

                    nbIntegrees++;

                    // crée la conection 
                    SqlRequests.EffectuerModificationOperationSimple(t, numCarte);
                }
                catch
                {
                    continue;
                }
            }
 
            return nbIntegrees > 0;
        }
    }

}
