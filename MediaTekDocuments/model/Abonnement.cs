using MediaTekDocuments.model;
using Newtonsoft.Json;
using System;

public class Abonnement : Commande
{
    [JsonProperty("dateFinAbonnement")]
    public DateTime DateFinAbonnement { get; set; }

    [JsonProperty("idRevue")]
    public string IdRevue { get; set; }  // Utilise int si c'est censé être un entier

    [JsonProperty("titre")]
    public string Titre { get; set; }

    // Assurer que 'dateCommande' et 'montant' ne sont pas dans le JSON
    public DateTime DateCommande { get; set; }  // Valeur par défaut pour DateCommande
    public double Montant { get; set; } // Valeur par défaut pour Montant

    public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue, string titre)
        : base(id, dateCommande, montant)
    {
        this.DateFinAbonnement = dateFinAbonnement;
        this.IdRevue = idRevue;
        this.Titre = titre;
    }

    public Abonnement() : base() {
        this.DateCommande = DateTime.Now;
        this.Montant = 0;
    }
}
