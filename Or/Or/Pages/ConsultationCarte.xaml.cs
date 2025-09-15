using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Or.Business;
using Or.Models;

namespace Or.Pages
{
    /// <summary>
    /// Logique ht'interaction pour ConsultationCarte.xaml
    /// </summary>
    public partial class ConsultationCarte : PageFunction<Int64>
    {
        long numCarte;
        List<Compte> comptes;

        public ConsultationCarte(long numCarte)
        {
            this.numCarte = numCarte;
            InitializeComponent();
            Carte c = SqlRequests.InfosCarte(numCarte);

            Numero.Text = c.Id.ToString();
            Prenom.Text = c.PrenomClient;
            Nom.Text = c.NomClient;

            comptes = SqlRequests.ListeComptesAssociesCarte(numCarte);
            listView.ItemsSource = comptes;
        }
        private void GoDetailsCompte(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new DetailsCompte(long.Parse(Numero.Text), (int)(sender as Button).CommandParameter));
        }

        private void GoHistoTransactions(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new HistoriqueTransactions(long.Parse(Numero.Text)));
        }

        private void GoVirement(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new Virement(long.Parse(Numero.Text)));
        }

        private void GoRetrait(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new Retrait(long.Parse(Numero.Text)));
        }

        private void GoDepot(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new Depot(long.Parse(Numero.Text)));
        }

        void PageFunctionNavigate(PageFunction<long> page)
        {
            page.Return += new ReturnEventHandler<long>(PageFunction_Return);
            NavigationService.Navigate(page);
        }

        void PageFunction_Return(object sender, ReturnEventArgs<long> e)
        {
            listView.ItemsSource = SqlRequests.ListeComptesAssociesCarte(long.Parse(Numero.Text));
        }

        private void GoExportXML(object sender, RoutedEventArgs e)
        {
            CodeResultat result = XmlIO.SerializeComptesTransactions(numCarte);
            if (result == CodeResultat.Ok)
                MessageBox.Show(ResultLabels.Label(result));
            else
                MessageBox.Show(ResultLabels.Label(result));

        }

        private void GoImportXML(object sender, RoutedEventArgs e)
        {
            CodeResultat result = XmlIO.DeSerialiserTransactions(InputPath.Text.Trim(), numCarte);
            if (result == CodeResultat.Ok)
                MessageBox.Show(ResultLabels.Label(result));
            else
                MessageBox.Show(ResultLabels.Label(result));
        }

        private void GoBeneficiaire(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new DetailsBeneficiaires(long.Parse(Numero.Text), comptes[0]));
        }
    }
}
