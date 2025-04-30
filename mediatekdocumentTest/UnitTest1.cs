using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;



namespace SupprimerAbonnementRevueTest
{
    [TestClass]
    public class TestParution
    {
        [TestMethod]
        public void TestParutionDansAbonnement()
        {
            DateTime dateCommande = new DateTime(2025, 01, 10);
            DateTime dateFinAbonnement = new DateTime(2025, 03, 17);
            DateTime dateParution = new DateTime(2025, 02, 25);
            bool resultatAttendu = true;
            bool resultatActuel = ParutionDansAbonnement(dateCommande, dateFinAbonnement, dateParution);
            Assert.AreEqual(resultatAttendu, resultatActuel);
        }
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }
    }
}