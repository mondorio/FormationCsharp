using Or.Business;
using Or.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Or.Pages
{
    /// <summary>
    /// Logique d'interaction pour HistoriqueTransactions.xaml
    /// </summary>
    public partial class HistoriqueTransactions : PageFunction<long>
    {
        public HistoriqueTransactions(long numCarte)
        {
            InitializeComponent();

            listView.ItemsSource = SqlRequests.ListeTransactionsAssociesCarte(numCarte);
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
