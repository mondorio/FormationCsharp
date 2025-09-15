using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Or.Models
{
    public class Beneficiaire
    {

        public int Id { get; set; } 
        public long NumCarte { get; set; }
        public int IdCpt { get; set; }

        public string Nom {  get; set; }
        public string Prenom { get; set; }

        public Beneficiaire(int id, long numCarte, int idCpt)
        {
            Id = id;
            NumCarte = numCarte;
            IdCpt = idCpt;
        }

        public Beneficiaire(int id, long numCarte, int idCpt, string nom, string prenom)
        {
            Id = id;
            NumCarte = numCarte;
            IdCpt = idCpt;
            Nom = nom;
            Prenom = prenom;
        }


    }
}
