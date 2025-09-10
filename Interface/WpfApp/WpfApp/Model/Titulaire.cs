using System.ComponentModel;

namespace WpfApp.Model
{
    public class Titulaire : INotifyPropertyChanged
    {
        private string nom;
        private string prenom;
        

        public DateTime DateAjout { get; set; }


        public string Prenom
        {
            get => prenom;
            set
            {
                if (prenom != value)
                {
                    prenom = value;
                    OnPropertyChanged(nameof(Prenom));
                }
            }
        }

        public string Nom
        {
            get => nom;
            set
            {
                if (nom != value)
                {
                    nom = value;
                    OnPropertyChanged(nameof(Nom));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
