using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
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
using WpfApp.Model;

namespace WpfApp
{
    /// <summary>
    /// Logique d'interaction pour PageDataBinding.xaml
    /// </summary>
    public partial class PageDataBinding : Page
    {

        public ModelPageBinding ModelePage { get; set; }


        public PageDataBinding()
        {
            InitializeComponent();

            ModelePage = new ModelPageBinding();
            ModelePage.Titulaire = new Titulaire { Nom = "Montand", Prenom="Yves", DateAjout= new DateTime(2000,9,10) };
            ModelePage.ListePersonnes = new ObservableCollection<Titulaire>()
            {
                new Titulaire {Nom = "Bonnaz", Prenom="Aymeric", DateAjout= new DateTime(2018,4,23)},
                new Titulaire {Nom = "Masse", Prenom="Robin", DateAjout= new DateTime(2019,6,14)},
                new Titulaire {Nom = "Man", Prenom="Emma", DateAjout= new DateTime(2022,1,12)}
            };

            

            DataContext = ModelePage; // On lie toute la fenêtre à Titulaire
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            ModelePage.Titulaire.Nom = "Réponse Valide";
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridView = ListView.View as GridView;
            if (gridView != null)
            {
                double totalWidth = ListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                gridView.Columns[0].Width = totalWidth * 0.40; // 40%
                gridView.Columns[1].Width = totalWidth * 0.40; // 40%
                gridView.Columns[2].Width = totalWidth * 0.20; // 20%
            }
        }

        private void Titulaire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string prenom = Choix.SelectedItem is not null ? (Choix.SelectedItem as Titulaire).Prenom : string.Empty;

            MessageBox.Show($"Prénom du titulaire : {prenom}", "Titulaire label",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
