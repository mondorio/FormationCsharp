using Or.Business;
using Or.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Or.Pages
{
    /// <summary>
    /// Logique d'interaction pour DetailsBeneficiaires.xaml
    /// </summary>

    public partial class DetailsBeneficiaires : PageFunction<long>
    {
        public long NumCarte { get; set; }
        public DetailsBeneficiaires(long numCarte, Compte compte)
        {
            InitializeComponent();

            NumCarte = numCarte;
            IdCompte.Text = compte.IdentifiantCarte.ToString();
            TypeCompte.Text = compte.TypeDuCompte.ToString();
            Solde.Text = compte.Solde.ToString("C2");


            Refresh();
        }
        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }



        public void Refresh()
        {
            //Items.Clear();
            listView.Items.Clear();
            // récupère comptes bénéficiaires
            var beneficiaires = SqlRequests.ListeBeneficiairesPourCarte(NumCarte);

            foreach (var b in beneficiaires)
            {
                listView.Items.Add(new BeneficiaireRow
                {
                    Id = b.Id,
                    Nom = b.Nom,
                    Prenom = b.Prenom,
                    NumeroCompte = b.NumeroCompte
                }); 
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        void PageFunctionNavigate(PageFunction<long> page)
        {
            page.Return += new ReturnEventHandler<long>(PageFunction_Return);
            NavigationService.Navigate(page);
        }
        void PageFunction_Return(object sender, ReturnEventArgs<long> e)
        {
            listView.ItemsSource = SqlRequests.ListeComptesAssociesCarte(NumCarte);
        }


        private void AjouterBenef_click(object sender, RoutedEventArgs e)
        {
            PageFunctionNavigate(new AjoutBenéficiaire(NumCarte));
        }

        private void SuprimerBenef_click(object sender, RoutedEventArgs e)
        {
            SqlRequests.SuppressionBeneficiaire(NumCarte, (int)(sender as Button).CommandParameter);
            Refresh();
        }
    }

    public class BeneficiaireRow
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public long NumeroCompte { get; set; }
    }
}

