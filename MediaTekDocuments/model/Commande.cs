using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
   public class Commande
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("dateCommande")]
        public DateTime DateCommande { get; set; }
        [JsonProperty("montant")]
        public double Montant { get; set; }
        public Commande(string id, DateTime dateCommande, double montant)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.Montant = montant;
        }
        public Commande() { }

    }
}
