using Or.Business;
using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour AjoutBenéficiaire.xaml
    /// </summary>
    public partial class AjoutBenéficiaire : PageFunction<long>
    {
        private long NumCarte;
        public AjoutBenéficiaire( long NumCarte)
        {
            this.NumCarte = NumCarte;
            InitializeComponent();

        }
        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(null);
        }


        private void Ajouter_click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputCPT.Text.Trim(), out int cpt))
            {
                CodeResultat result = SqlRequests.AjoutBeneficiaire(NumCarte, cpt);
                if (result  == CodeResultat.Ok)
                {
                    MessageBox.Show(ResultLabels.Label(result));
                    OnReturn(null);
                }
                else
                {
                    MessageBox.Show(ResultLabels.Label(result));
                }                
            }
            else 
                MessageBox.Show(ResultLabels.Label(CodeResultat.CompteIntrouvable));
        }
    }
}
