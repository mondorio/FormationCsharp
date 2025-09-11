using Or.Business;
using Or.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Or.Pages
{
    /// <summary>
    /// Logique d'interaction pour Retrait.xaml
    /// </summary>
    public partial class Retrait : PageFunction<long>
    {
        Carte CartePorteur { get; set; }
        Compte ComptePorteur { get; set; }
        public Retrait(long numCarte)
        {
            InitializeComponent();
            Montant.Text = 0M.ToString("C2");

            CartePorteur = SqlRequests.InfosCarte(numCarte);
            ComptePorteur = SqlRequests.ListeComptesAssociesCarte(CartePorteur.Id).Find(x => x.TypeDuCompte == TypeCompte.Courant);

            PlafondMaxRetrait.Text = CartePorteur.Plafond.ToString("C2");
            Solde.Text = ComptePorteur.Solde.ToString("C2");
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }

        private void ValiderRetrait_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(Montant.Text.Replace(".", ",").Trim(new char[] { '€', ' ' }), out decimal montant) && montant > 0)
            {
                //Compte fictif pour permettre la transaction
                Compte compteBanque = new Compte(0, 0, TypeCompte.Courant, 0);
                Transaction t = new Transaction(0, DateTime.Now, montant, ComptePorteur.Id, compteBanque.Id);

                if (CartePorteur.EstRetraitAutoriseNiveauCarte(t, compteBanque, ComptePorteur) && ComptePorteur.EstRetraitValide(t))
                {
                    SqlRequests.EffectuerModificationOperationSimple(t, CartePorteur.Id);

                    OnReturn(null);
                }
                else
                {
                    MessageBox.Show("Opération de retrait non authorisée");
                }
            }
            else
            {
                MessageBox.Show("Montant invalide");
            }
        }
    }
}
