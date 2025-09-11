using Or.Business;
using Or.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Or.Pages
{
    /// <summary>
    /// Logique d'interaction pour Virement.xaml
    /// </summary>
    public partial class Virement : PageFunction<long>
    {

        Carte CartePorteur { get; set; }
        Compte ComptePorteur { get; set; }
        public Virement(long numCarte)
        {
            InitializeComponent();

            Montant.Text = 0M.ToString("C2");

            CartePorteur = SqlRequests.InfosCarte(numCarte);
            CartePorteur.AlimenterHistoriqueEtListeComptes(SqlRequests.ListeTransactionsAssociesCarte(numCarte), SqlRequests.ListeComptesAssociesCarte(CartePorteur.Id).Select(x=>x.Id).ToList());
            ComptePorteur = SqlRequests.ListeComptesAssociesCarte(CartePorteur.Id).Find(x => x.TypeDuCompte == TypeCompte.Courant);

            var viewExpediteur = CollectionViewSource.GetDefaultView(SqlRequests.ListeComptesAssociesCarte(numCarte));
            viewExpediteur.GroupDescriptions.Add(new PropertyGroupDescription("TypeDuCompte"));
            viewExpediteur.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
            viewExpediteur.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Ascending));
            Expediteur.ItemsSource = viewExpediteur;

            var viewDestinataire = CollectionViewSource.GetDefaultView(SqlRequests.ListeComptesDispo(ComptePorteur.Id));
            viewDestinataire.GroupDescriptions.Add(new PropertyGroupDescription("IdentifiantCarte"));
            viewDestinataire.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Ascending));
            viewDestinataire.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
            Destinataire.ItemsSource = viewDestinataire;
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }

        private void ValiderVirement_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(Montant.Text.Replace(".", ",").Trim(new char[] { '€', ' ' }), out decimal montant))
            {
                Compte ex = Expediteur.SelectedItem as Compte;
                Compte de = Destinataire.SelectedItem as Compte;

                Transaction t = new Transaction(0, DateTime.Now, montant, ex.Id, de.Id);

                if ((Expediteur.SelectedItem as Compte).EstRetraitValide(t) && CartePorteur.EstRetraitAutoriseNiveauCarte(t, ex, de))
                {
                    SqlRequests.EffectuerModificationOperationInterCompte(t, ex.IdentifiantCarte, de.IdentifiantCarte);
                    OnReturn(null);
                }
                else
                {
                    MessageBox.Show("Opération de virement non autorisé");
                }
            }
            else
            {
                MessageBox.Show("Montant invalide");
            }

        }

        private void Expediteur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewDestinataire = CollectionViewSource.GetDefaultView(SqlRequests.ListeComptesDispo((Expediteur.SelectedItem as Compte).Id));
            viewDestinataire.GroupDescriptions.Add(new PropertyGroupDescription("IdentifiantCarte"));
            viewDestinataire.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Descending));
            viewDestinataire.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
            Destinataire.ItemsSource = viewDestinataire;
        }
    }
}
