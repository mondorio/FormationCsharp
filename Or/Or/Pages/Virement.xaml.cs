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
        long NumCarte;
        public Virement(long numCarte)
        {
            NumCarte = numCarte;
            InitializeComponent();

            Montant.Text = 0M.ToString("C2");

            CartePorteur = SqlRequests.InfosCarte(numCarte);
            CartePorteur.AlimenterHistoriqueEtListeComptes(SqlRequests.ListeTransactionsAssociesCarte(numCarte), SqlRequests.ListeComptesAssociesCarte(CartePorteur.Id).Select(x => x.Id).ToList());
            ComptePorteur = SqlRequests.ListeComptesAssociesCarte(CartePorteur.Id).Find(x => x.TypeDuCompte == TypeCompte.Courant);

            var viewExpediteur = CollectionViewSource.GetDefaultView(SqlRequests.ListeComptesAssociesCarte(numCarte));
            viewExpediteur.GroupDescriptions.Add(new PropertyGroupDescription("TypeDuCompte"));
            viewExpediteur.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
            viewExpediteur.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Ascending));
            Expediteur.ItemsSource = viewExpediteur;


            var viewBeneficiaire = CollectionViewSource.GetDefaultView(SqlRequests.ListeVirementPossible(numCarte));
            viewBeneficiaire.GroupDescriptions.Add(new PropertyGroupDescription("IdentifiantCarte"));
            viewBeneficiaire.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Ascending));
            viewBeneficiaire.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
            Destinataire.ItemsSource = viewBeneficiaire;
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }

        private void ValiderVirement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (decimal.TryParse(Montant.Text.Replace(".", ",").Trim(new char[] { '€', ' ' }), out decimal montant))
                {
                    Compte ex = Expediteur.SelectedItem as Compte;
                    Compte de = Destinataire.SelectedItem as Compte;

                    Transaction t = new Transaction(0, DateTime.Now, montant, ex.Id, de.Id);

                    CodeResultat result = (Expediteur.SelectedItem as Compte).EstRetraitValide(t);
                    if (result == CodeResultat.Ok)
                    {
                        result = CartePorteur.EstRetraitAutoriseNiveauCarte(t, ex, de);
                        if (result == CodeResultat.Ok)
                        {
                            SqlRequests.EffectuerModificationOperationInterCompte(t, ex.IdentifiantCarte, de.IdentifiantCarte);
                            OnReturn(null);
                        }
                        else MessageBox.Show(ResultLabels.Label(result));
                    }
                    else MessageBox.Show(ResultLabels.Label(result));
                }
                else
                {
                    MessageBox.Show(ResultLabels.Label(CodeResultat.MontantInvalide));
                }
            }
            catch
            {
                MessageBox.Show("information manquante pour le virement");
            }
        }

        private void Expediteur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var item = SqlRequests.ListeVirementPossible(NumCarte);
            if (Expediteur.SelectedItem is Compte expediteur)
            {
                int idcpt = expediteur.Id;

                item.RemoveAll(x => x.Id == idcpt);
                var viewDestinataire = CollectionViewSource.GetDefaultView(item);
                viewDestinataire.GroupDescriptions.Add(new PropertyGroupDescription("IdentifiantCarte"));
                viewDestinataire.SortDescriptions.Add(new SortDescription("IdentifiantCarte", ListSortDirection.Descending));
                viewDestinataire.SortDescriptions.Add(new SortDescription("TypeDuCompte", ListSortDirection.Ascending));
                Destinataire.ItemsSource = viewDestinataire;
            }
        }

        private void AjoutBenef_Click(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new AjoutBenéficiaire(NumCarte));
        }

        void PageFunctionNavigate(PageFunction<long> page)
        {
            page.Return += new ReturnEventHandler<long>(PageFunction_Return);
            NavigationService.Navigate(page);
        }

        void PageFunction_Return(object sender, ReturnEventArgs<long> e)
        {

        }
    }
}
