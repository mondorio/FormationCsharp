using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Or.Pages
{
    /// <summary>
    /// Logique d'interaction pour Accueil.xaml
    /// </summary>
    public partial class Accueil : Page
    {
        public Accueil()
        {
            InitializeComponent();
        }

        public void GoConsultationCarte(object sender, RoutedEventArgs e)
        {
            long.TryParse(NumeroCarte.Text, out long result);
            NavigationService.Navigate(new ConsultationCarte(result));
        }
    }
}
