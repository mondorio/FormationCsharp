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

        [XmlRoot("Comptes")]
        public class ListXMLCompte
        {
            [XmlElement("Compte", typeof(XMLCompte))]
            public List<XMLCompte> Liste { get; set; }
        }

        [XmlType("Compte")]
        public class XMLCompte
        {
            [XmlElement("Identifiant")]
            public int Id { get; set; }

            [XmlElement("Type")]
            public TypeCompte Type { get; set; }

            [XmlElement("Solde")]
            public string Solde { get; set; }

            [XmlArray("Transactions")]
            [XmlArrayItem("Transaction")]
            public List<XMLTransaction> Transactions { get; set; } = new List<XMLTransaction>();
        }

        public class XMLTransaction
        {
            [XmlElement("Identifiant")]
            public int Id { get; set; }

            // format: DD/MM/YYYY HH:mm:ss
            [XmlIgnore]
            public DateTime Date { get; set; }

            [XmlElement("Date")]
            public string DateString
            {
                get => Date.ToString("dd/MM/yyyy HH:mm:ss");
                set => DateTime.Parse(value);
            }

            [XmlElement("Type")]
            public string Type { get; set; }

            // présents ou non selon le type
            [XmlIgnore]
            public int CompteExpediteur { get; set; }

            // Bonne façon de faire
            [XmlElement("CompteExpediteur")]
            public string CompteExpediteurString
            {
                get => (CompteExpediteur == 0) ? null : CompteExpediteur.ToString();
                set { CompteExpediteur = (value == null) ? 0 : int.Parse(value); }
            }

            [XmlIgnore]
            public int CompteDestinataire { get; set; }

            [XmlElement("CompteDestinataire")]
            public string CompteDestinataireString
            {
                get => (CompteDestinataire == 0) ? null : CompteDestinataire.ToString();
                set { CompteDestinataire = (value == null) ? 0 : int.Parse(value); }
            }

            // Montant absolu
            [XmlElement("Montant")]
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


            ListXMLCompte exportList = new ListXMLCompte() { Liste = new List<XMLCompte>()};


            // on parcourt nos comptes
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
                //on parcourt nos transaction pour les lier à nos comptes
                foreach (var t in triTransac)
                {
                    //creation de notre transaction
                    var TransactionXML = new XMLTransaction
                    {
                        Id = t.IdTransaction,
                        Date = t.Horodatage,
                        Type = t.Type == Operation.DepotSimple ? "Dépôt" : t.Type == Operation.RetraitSimple ? "Retrait" : "Virement",
                        Montant = FormatMoney(t.Montant)
                    };

                    // Présence des attributs expéditeur/destinataire retrait ou virement
                    if (t.Type == Operation.RetraitSimple || t.Type == Operation.InterCompte)
                        TransactionXML.CompteExpediteur = t.Expediteur;

                    if (t.Type == Operation.DepotSimple || t.Type == Operation.InterCompte)
                        TransactionXML.CompteDestinataire = t.Destinataire;

                    cptXML.Transactions.Add(TransactionXML);
                }

                exportList.Liste.Add(cptXML);
            }
            try
            {
                filePath += numCarte.ToString();


                var serializer = new XmlSerializer(typeof(ListXMLCompte));

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

        public static CodeResultat DeSerialiserTransactions(string path, long numCarte)
        {
            var listTransaction = new List<Transaction>();
            if (!File.Exists(path)) return CodeResultat.XMLNotFound;

            var fr = CultureInfo.GetCultureInfo("fr-FR");

            // charge le doc
            var xdoc = XDocument.Load(path, LoadOptions.SetLineInfo);

            var cxNodes = xdoc.Descendants().Where(e => e.Name.LocalName.Equals("XMLCompte", StringComparison.OrdinalIgnoreCase));

            foreach (var node in cxNodes)
            {
                var ligne = (IXmlLineInfo)node;

                // --- ATTRIBUTS DU COMPTE ---
                string idStr = (string)node.Attribute("Identifiant");
                string typeStr = (string)node.Attribute("Type");
                string soldeStr = (string)node.Attribute("Solde");

                // id compte 
                if (idStr != null && !int.TryParse(idStr, out int _))
                    continue;

                // type de compte
                if (typeStr != null && !TypeCompte.TryParse(typeStr, out TypeCompte _))
                    continue;

                // solde ex: 10 732,00 €
                if (soldeStr != null)
                {
                    var cleanedSolde = soldeStr.Replace("€", "").Trim();
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
                    Operation typeT = 0;
                    int compteExp = 0;
                    int compteDest = 0;
                    decimal montant = 0m;

                    // tous en ATTRIBUTS
                    string idTStr = (string)tx.Attribute("Identifiant");
                    string dateTStr = (string)tx.Attribute("Date");                 
                    string typeTStr = (string)tx.Attribute("Type");
                    string expStr = (string)tx.Attribute("CompteExpediteur");
                    string destStr = (string)tx.Attribute("CompteDestinataire");
                    string montantStr = (string)tx.Attribute("Montant");              

                    // id transaction
                    if (idTStr != null && !int.TryParse(idTStr, out idt))
                        continue;

                    // dateTime
                    if (dateTStr != null && 
                        !DateTime.TryParseExact(dateTStr, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateT))
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
            List<Compte> cpts = SqlRequests.ListeTousLesComptesCarte();
            var byId = cpts.ToDictionary(c => c.Id, c => c);

            int nbIntegrees = 0;

            foreach (var t in transactions)
            {
                try
                {

                    // Existence des comptes
                    Compte cpt = null, de = null;
                    if (t.Expediteur > 0 && !byId.TryGetValue(t.Expediteur, out cpt)) continue;
                    if (t.Destinataire > 0 && !byId.TryGetValue(t.Destinataire, out de)) continue;

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

                    // Solde suffisant expéditeur 
                    bool debit = (t.Type == Operation.RetraitSimple || t.Type == Operation.InterCompte);
                    if (debit && cpt != null && cpt.Solde < t.Montant) continue;

                    nbIntegrees++;

                    // Ajoute la transaction
                    if (!(cpt is null) && !(de is null) &&  cpt.IdentifiantCarte != de.IdentifiantCarte) 
                        SqlRequests.EffectuerModificationOperationInterCompte(t, cpt.IdentifiantCarte, de.IdentifiantCarte);
                    else 
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
