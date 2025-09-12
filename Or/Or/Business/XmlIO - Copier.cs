/*using Or.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        public class ExportCompte
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


            List<ExportCompte> exportList = new List<ExportCompte>();



            //on parcour nos compte
            foreach (Compte c in comptes)
            {
                ExportCompte cptXML = new ExportCompte
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


                var serializer = new XmlSerializer(typeof(List<ExportCompte>));

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

        public static CodeResultat DeSerialiserTransactions(string path)
        {

            List<XMLTransaction> xMLtransactions;
            if (!File.Exists(path)) return CodeResultat.XMLNotFound;

            // Conserver infos de lignes
            var xdoc = XDocument.Load(path, LoadOptions.SetLineInfo);

            // attrape tous les nœuds "Transaction", peu importe les namespaces
            var txNodes = xdoc
                .Descendants()
                .Where(e => e.Name.LocalName.Equals("Transaction", StringComparison.OrdinalIgnoreCase));

            var results = new List<Transaction>();
            var errors = new List<string>();

            foreach (var node in txNodes)
            {
                var ligne = (IXmlLineInfo)node;

                //string idStr = GetAttrOrElem(node, "Identifiant"); 
                string idStr = node.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "Identifiant", StringComparison.OrdinalIgnoreCase)).Value;
                if (idStr != null && !int.TryParse(idStr, out int id)) continue;

                string typeStr = node.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "Type", StringComparison.OrdinalIgnoreCase)).Value;
                if (typeStr != null && !TypeCompte.TryParse(typeStr, out TypeCompte type)) continue;

                string soldeStr = node.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "Solde", StringComparison.OrdinalIgnoreCase)).Value;
                if (soldeStr != null && !decimal.TryParse(idStr, out decimal solde)) continue;


            }

            //validation 
            /*var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(null, "modele.xsd"); // ton XSD*/




            //deserialise
            /*var serializer = new XmlSerializer(typeof(List<XMLTransaction>));
            using (var reader = new StreamReader(path))
            {
                xMLtransactions = (List<XMLTransaction>)serializer.Deserialize(reader);
            }

            if (xMLtransactions.Count == 0) return CodeResultat.XMLEmpty;

            var listTransaction = new List<Transaction>();

            foreach (XMLTransaction xml in xMLtransactions)
            {
                if (decimal.TryParse(xml.Montant, out decimal mont)) continue;

                Transaction t = new Transaction(xml.Id, xml.Date, mont, xml.CompteExpediteur, xml.CompteDestinataire);
                listTransaction.Add(t);
            }

            if (!TraitementTransactionsImportees(listTransaction)) return CodeResultat.XMLImportFail;

            return CodeResultat.Ok;
        }

        public static bool TraitementTransactionsImportees(List<Transaction> transactions)
        {
            if (transactions.Count == 0) return false;

            foreach (Transaction transaction in transactions)
            {

            }

            return true;
        }
    }

}
*/