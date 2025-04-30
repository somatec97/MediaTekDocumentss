using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Abonnement : Commande
    {
        [JsonProperty("dateFinAbonnement")]
        public DateTime DateFinAbonnement { get; set; }
        [JsonProperty("idRevue")]
        public string IdRevue { get; set; }
        [JsonProperty("titre")]
        public string Titre { get; set; }
        
        public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue, string titre) : base(id, dateCommande, montant)
        {
            this.DateFinAbonnement = dateFinAbonnement;
            this.IdRevue = idRevue;
            this.Titre = titre;
           
        }
        public Abonnement(): base()  { }


    }
}
