using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
           // FrmAlerteFinAbonnement frmAlerteFinAbonnement = new FrmAlerteFinAbonnement(controller);
           // frmAlerteFinAbonnement.ShowDialog();
            if (Service.Libelle == "administratif" || Service.Libelle == "administrateur")
            {
                FrmAlerteFinAbonnement frmAlerteFinAbonnement = new FrmAlerteFinAbonnement(controller);
                frmAlerteFinAbonnement.ShowDialog();
            }
            else if (Service.Libelle == "prêts")
            {
                tabOngletsApplication.TabPages.Remove(TabCmdLivres);
                tabOngletsApplication.TabPages.Remove(tabCmdDvd);
                tabOngletsApplication.TabPages.Remove(tabCmdRevues);
                grpLivresInfos.Enabled = false;
                txbExemplaireLivresNumero.Enabled = false;
                dtpDateAchatExemplaireLivre.Enabled = false;
                cbxEtatLibelleExemplaireLivre.Enabled = false;
                btnEtatExemplaireLivreModifier.Enabled = false;
                btnExemplaireLivreSupprimer.Enabled = false;
                grpDvdInfos.Enabled = false;
              //  txbExemplaireDvdNumero.Enabled = false;
               // dtpDateAchatExemplaireDvd.Enabled = false;
               // cbxEtatLibelleExemplaireDvd.Enabled = false;
               // btnEtatExemplaireDvdModifier.Enabled = false;
               // btnExemplaireDvdSupprimer.Enabled = false;
                grpRevuesInfos.Enabled = false;
                txbReceptionExemplaireNumero.Enabled = false;
                dtpReceptionExemplaireDate.Enabled = false;
                txbReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireValider.Enabled = false;
               // dtpDateAchatExemplaireRevue.Enabled = false;
               // btnExemplaireRevueSupprimer.Enabled = false;
                //cbxEtatLibelleExemplaireRevue.Enabled = false;
               // btnEtatExemplaireRevueModifier.Enabled = false;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private bool modeAjoutCommande = false;


        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
            RemplirCbxNewGenreLivre();
            RemplirCbxNewPublicLivre();
            RemplirCbxNewRayonLivre();
            RemplirLivresListeComplete();
            gbxExemplairesLivre.Enabled = false;
            gbxEtatExemplaireLivre.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                    gbxExemplairesLivre.Enabled = true;
                    AfficheExemplairesLivres();
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        private string RemplirCbxNewGenreLivre()
        {
            List<Categorie> LesGenresLivres = controller.GetAllGenres();
            foreach (Categorie genre in LesGenresLivres)
            {
                cbxNewGenreLivre.Items.Add(genre.Libelle);
            }
            if (cbxNewGenreLivre.Items.Count > 0)
            {
                cbxNewGenreLivre.SelectedIndex = 0;
            }
            return cbxNewGenreLivre.SelectedItem?.ToString();
        }
        /// <summary>
        /// filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxNewGenreLivre_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemplirCbxNewGenreLivre();
        }
        private string RemplirCbxNewPublicLivre()
        {
            List<Categorie> LesPublicsLivres = controller.GetAllPublics();
            foreach (Categorie lePublic in LesPublicsLivres)
            {
                cbxNewPublicLivre.Items.Add(lePublic.Libelle);
            }
            if (cbxNewPublicLivre.Items.Count > 0)
            {
                cbxNewPublicLivre.SelectedIndex = 0;
            }
            return cbxNewPublicLivre.SelectedItem?.ToString();
        }
        private void CbxNewPublicLivre_SelectedIndexChanged(object sender, EventArgs e)
        {

            RemplirCbxNewPublicLivre();
        }
        private string RemplirCbxNewRayonLivre()
        {
            List<Categorie> LesRayonsLivres = controller.GetAllRayons();
            foreach (Categorie rayon in LesRayonsLivres)
            {
                cbxNewRayonLivre.Items.Add(rayon.Libelle);
            }
            if (cbxNewRayonLivre.Items.Count > 0)
            {
                cbxNewRayonLivre.SelectedIndex = 0;
            }
            return cbxNewRayonLivre.SelectedItem?.ToString();
        }
        private void cbxNewRayonLivre_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemplirCbxNewRayonLivre();

        }

        /// <summary>
        /// Id du genre qui correspond au genre de document
        /// </summary>
        /// <param name="genre">genre de document sélectionné</param>
        /// <returns>id du genre selectionné</returns>
        private string GetIdGenreDocument(string genre)
        {
            List<Categorie> lesGenresDocument = controller.GetAllGenres();
            foreach (Categorie cat in lesGenresDocument)
            {
                if (cat.Libelle == genre)
                {
                    return cat.Id;
                }
            }
            return null;
        }
        /// <summary>
        /// id dupublic qui correspond au public de document
        /// </summary>
        /// <param name="lePublic">lePublic du document sélectionné</param>
        /// <returns>id du public selectionné</returns>
        private string GetIdPublicDocument(string lePublic)
        {
            List<Categorie> lesPublicsDocument = controller.GetAllPublics();
            foreach (Categorie cat in lesPublicsDocument)
            {
                if (cat.Libelle == lePublic)
                {
                    return cat.Id;
                }
            }
            return null;
        }
        /// <summary>
        /// id du rayon qui correspond au rayon de document
        /// </summary>
        /// <param name="rayon">rayon du document sélectionné</param>
        /// <returns>id du public selectionné</returns>
        private string GetIdRayonDocument(string rayon)
        {
            List<Categorie> lesRayonsDocument = controller.GetAllRayons();
            foreach (Categorie cat in lesRayonsDocument)
            {
                if (cat.Libelle == rayon)
                {
                    return cat.Id;
                }
            }
            return null;
        }
        /// <summary>
        /// vider les informations d'un livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViderInfosLivre_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
        }
        /// <summary>
        /// l'ajout d'un livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterNewLivre_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumero.Text.Equals(" ") && !txbLivresTitre.Text.Equals(" ") && !txbLivresCollection.Text.Equals(" ") && !cbxNewGenreLivre.Text.Equals(" ") && !cbxNewPublicLivre.Text.Equals(" ") && !cbxNewRayonLivre.Text.Equals(" "))
            {
                try
                {
                    string id = txbLivresNumero.Text;
                    string titre = txbLivresTitre.Text;
                    string image = txbLivresImage.Text;
                    string isbn = txbLivresIsbn.Text;
                    string auteur = txbLivresAuteur.Text;
                    string collection = txbLivresCollection.Text;
                    string idGenre = GetIdGenreDocument(cbxNewGenreLivre.Text);
                    string idPublic = GetIdPublicDocument(cbxNewPublicLivre.Text);
                    string idRayon = GetIdRayonDocument(cbxNewRayonLivre.Text);
                    string genre = txbLivresGenre.Text;
                    string lePublic = txbLivresPublic.Text;
                    string rayon = txbLivresRayon.Text;
                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    var idLivreExist = controller.GetAllDocuments(id);
                    var idLivreNoExist = !idLivreExist.Any();

                    if (idLivreNoExist)
                    {
                        if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdGenre, document.IdPublic, document.IdRayon) && controller.CreerLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection))
                        {
                            lesLivres = controller.GetAllLivres();
                            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                            RemplirLivresListeComplete();
                            MessageBox.Show(" Le livre " + titre + " est bien ajouté ");
                        }
                    }
                    else
                    {
                        MessageBox.Show(" Le numéro de document existe déjà! saisir un autre numéro! ", "ERREUR!!");
                    }

                }
                catch
                {
                    MessageBox.Show(" Une erreur s'est produite!!", "ERREUR!!");
                }
            }
            else
            {
                MessageBox.Show(" Veuillez saisir tous les champs!", "INFORMATION!!");
            }
        }

        /// <summary>
        /// modification d'un livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierLivre_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count > 0)
            {
                Livre selectLivre = (Livre)dgvLivresListe.SelectedRows[0].DataBoundItem;
                string id = selectLivre.Id;
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                string idGenre = GetIdGenreDocument(cbxNewGenreLivre.Text);
                string idPublic = GetIdPublicDocument(cbxNewPublicLivre.Text);
                string idRayon = GetIdRayonDocument(cbxNewRayonLivre.Text);
                if (!txbLivresNumero.Text.Equals("") && !txbLivresTitre.Text.Equals("") && !txbLivresCollection.Text.Equals("") && !cbxNewGenreLivre.Text.Equals("") && !cbxNewPublicLivre.Text.Equals("") && !cbxNewRayonLivre.Text.Equals(""))
                {
                    if (controller.EditDocument(id, titre, image, idGenre, idPublic, idRayon) && controller.EditLivre(id, isbn, auteur, collection))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show("Le livre " + titre + " est bien modifié!");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du livre", "ERREUR!!");

                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir tous les champs!!", "INFORMATION!!");
                }

            }
            else
            {
                MessageBox.Show("Veuillez selectioner une ligne!!", "INFORMATION!!");
            }
        }

        /// <summary>
        /// suppression d'un livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerLivre_Click(object sender, EventArgs e)
        {
            Livre livre = (Livre)bdgLivresListe.Current;
            if (MessageBox.Show("êtes vous sûr de vouloir supprimer ce" + livre.Titre + "?", "CONFIRMATION!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var exmpLivre = controller.GetExemplairesDocument(livre.Id);
                var noExmpl = !exmpLivre.Any();
                var commandeLivre = controller.GetCommandeDocument(livre.Id);
                var noCommande = !commandeLivre.Any();
                if (noCommande && noExmpl)
                {
                    if (controller.DeleteLivre(livre.Id))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show("Le livre " + livre.Titre + " est supprimer!");
                    }
                    else
                    {
                        MessageBox.Show("erreur!!", "ERREUR!");
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de supprimer ce livre car il possède un ou plusieurs exemplaire(s), ou une ou plusieurs commande(s)", "ERREUR!!");
                }
            }
        }
        private readonly BindingSource bdgExemplairesLivre = new BindingSource();
        private List<Exemplaire> lesExemplairesDocument = new List<Exemplaire>();


        /// <summary>
        /// Remplit la datagrid avec la liste passée en paramètre
        /// </summary>
        /// <param name="lesExemplaires"></param>
        private void RemplirExemplairesLivre(List<Exemplaire> lesExemplaires)
        {
            if (lesExemplaires != null)
            {
                bdgExemplairesLivre.DataSource = lesExemplaires;
                dgvExemplairesLivre.DataSource = bdgExemplairesLivre;
                dgvExemplairesLivre.Columns["photo"].Visible = false;
                dgvExemplairesLivre.Columns["idEtat"].Visible = false;
                dgvExemplairesLivre.Columns["id"].Visible = false;
                dgvExemplairesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvExemplairesLivre.Columns[0].HeaderCell.Value = "Numéro";
                dgvExemplairesLivre.Columns[2].HeaderCell.Value = "Date d'achat";
                dgvExemplairesLivre.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                dgvExemplairesLivre.DataSource = null;
            }
        }

        /// <summary>
        /// Affichage des exemplaires d'un livre 
        /// </summary>
        private void AfficheExemplairesLivres()
        {
            string idDocument = txbLivresNumRecherche.Text;
            lesExemplairesDocument = controller.GetExemplairesDocument(idDocument);
            RemplirExemplairesLivre(lesExemplairesDocument);
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void dgvExemplairesLivre_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvExemplairesLivre.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Date d'achat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Numéro":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Numero).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirExemplairesLivre(sortedList);
        }

        /// <summary>
        /// Remplissage de la comboBox selon les états de l'exemplaire et le libelle correspondant
        /// </summary>
        /// <param name="etatExemplaireLivre"></param>
        private void RemplirCbxEtatLibelleExemplaireLivre(string etatExemplaireLivre)
        {
            cbxEtatLibelleExemplaireLivre.Items.Clear();
            if (etatExemplaireLivre == "neuf")
            {
                
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "usagé")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "détérioré")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "inutilisable")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
            }
          

        }


        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des états possibles de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtatExemplaireLivre_TextChanged(object sender, EventArgs e)
        {
            // Assure-toi que le texte de l'étiquette n'est pas vide avant de remplir le combo
            string etatExemplaireLivre = lblEtatExemplaireLivre.Text;
            if (!string.IsNullOrEmpty(etatExemplaireLivre))
            {
                RemplirCbxEtatLibelleExemplaireLivre(etatExemplaireLivre);
            }
            else
            {
                // Si le texte est vide, tu pourrais décider de vider le ComboBox ou de faire autre chose.
                cbxEtatLibelleExemplaireLivre.Items.Clear();
            }
        }


        /// <summary>
        /// Récupère l'id d'un état selon son libelle
        /// </summary>
        /// <param name="libelle"></param>
        /// <returns></returns>
        private string GetIdEtat(string libelle)
        {
            List<Etat> lesEtats = controller.GetAllEtatsDocument();
            foreach (Etat unEtat in lesEtats)
            {
                if (unEtat.Libelle == libelle)
                {
                    return unEtat.Id;
                }
            }
            return null;
        }

        private void dgvExemplairesLivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Sécurité : ignorer les lignes vides ou sans données utiles
            if (e.RowIndex < 0 || dgvExemplairesLivre.Rows[e.RowIndex].IsNewRow)
                return;

            DataGridViewRow row = dgvExemplairesLivre.Rows[e.RowIndex];

            // Vérifie qu'il y a bien une cellule non vide
            if (row.Cells["Id"].Value == null || string.IsNullOrWhiteSpace(row.Cells["Id"].Value.ToString()))
                return;

            // Lecture des données
            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();

            // Remplissage
            txbExemplaireLivresNumero.Text = numero;
            dtpDateAchatExemplaireLivre.Value = dateAchat;
            lblEtatExemplaireLivre.Text = libelle;
            RemplirCbxEtatLibelleExemplaireLivre(libelle); // Pour que le combo soit rempli

            // Activation des groupes si tout est ok
            gbxExemplairesLivre.Enabled = true;
            gbxEtatExemplaireLivre.Enabled = true;
        }



        /// <summary>
        /// Modification de l'état d'un exemplaire de livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtatExemplaireLivreModifier_Click_1(object sender, EventArgs e)
        {
            string idDocument = txbLivresNumRecherche.Text;
            int numero = int.Parse(txbExemplaireLivresNumero.Text);
            DateTime dateAchat = dtpDateAchatExemplaireLivre.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireLivre.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireLivre.SelectedItem.ToString();
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheExemplairesLivres();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }


        /// <summary>
        /// Suppression d'un exemplaire de livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExemplaireLivreSupprimer_Click_1(object sender, EventArgs e)
        {
            if (dgvExemplairesLivre.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " du livre " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheExemplairesLivres();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
       

        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
            RemplirCbxNewGenreDvd();
            RemplirCbxNewPublicDvd();
            RemplirCbxNewRayonDvd();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        /// <summary>
        /// remplir la combobox du genre du dvd à ajouter ou modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewGenreDvd()
        {
            List<Categorie> LesGenresDvd = controller.GetAllGenres();
            foreach (Categorie genre in LesGenresDvd)
            {
                cbxNewGenreDvd.Items.Add(genre.Libelle);
            }
            if (cbxNewGenreDvd.Items.Count > 0)
            {
                cbxNewGenreDvd.SelectedIndex = 0;
            }
            return cbxNewGenreDvd.SelectedItem?.ToString();
        }
        /// <summary>
        /// remplir la combobox du public du dvd a ajouter ou modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewPublicDvd()
        {
            List<Categorie> LesPublicsDvd = controller.GetAllPublics();
            foreach (Categorie lePublic in LesPublicsDvd)
            {
                cbxNewPublicDvd.Items.Add(lePublic.Libelle);
            }
            if (cbxNewPublicDvd.Items.Count > 0)
            {
                cbxNewPublicDvd.SelectedIndex = 0;
            }
            return cbxNewPublicDvd.SelectedItem?.ToString();
        }
        /// <summary>
        /// remplir la combobox du rayon du dvd a ajouter ou modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewRayonDvd()
        {
            List<Categorie> LesRayonsDvd = controller.GetAllRayons();
            foreach (Categorie rayon in LesRayonsDvd)
            {
                cbxNewRayonDvd.Items.Add(rayon.Libelle);
            }
            if (cbxNewRayonDvd.Items.Count > 0)
            {
                cbxNewRayonDvd.SelectedIndex = 0;
            }
            return cbxNewRayonDvd.SelectedItem?.ToString();
        }
        /// <summary>
        /// vider les information dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnViderInfosDvd_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
        }
        /// <summary>
        /// ajouter un dvd dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterNewDvd_Click_1(object sender, EventArgs e)
        {
            if (!txbDvdNumero.Text.Equals(" ") && !txbDvdTitre.Text.Equals(" ") && !txbDvdRealisateur.Text.Equals(" ") && !txbDvdSynopsis.Text.Equals("") && !cbxNewGenreDvd.Text.Equals(" ") && !cbxNewPublicDvd.Text.Equals(" ") && !cbxNewRayonDvd.Text.Equals(" "))
            {
                try
                {
                    string id = txbDvdNumero.Text;
                    string titre = txbDvdTitre.Text;
                    string image = txbDvdImage.Text;
                    int duree = int.Parse(txbDvdDuree.Text);
                    string realisateur = txbDvdRealisateur.Text;
                    string synopsis = txbDvdSynopsis.Text;
                    string idGenre = GetIdGenreDocument(cbxNewGenreDvd.Text);
                    string idPublic = GetIdPublicDocument(cbxNewPublicDvd.Text);
                    string idRayon = GetIdRayonDocument(cbxNewRayonDvd.Text);
                    string genre = txbDvdGenre.Text;
                    string lePublic = txbDvdPublic.Text;
                    string rayon = txbDvdRayon.Text;
                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    var idDvdExist = controller.GetAllDocuments(id);
                    var idDvdNoExist = !idDvdExist.Any();

                    if (idDvdNoExist)
                    {
                        if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdGenre, document.IdPublic, document.IdRayon) && controller.CreerDvd(dvd.Id, dvd.Duree, dvd.Realisateur, dvd.Synopsis))
                        {
                            lesDvd = controller.GetAllDvd();
                            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                            RemplirDvdListeComplete();
                            MessageBox.Show(" Le dvd " + titre + " est bien ajouté ");
                        }
                    }
                    else
                    {
                        MessageBox.Show(" Le numéro de document existe déjà! saisir un autre numéro! ", "ERREUR!!");
                    }

                }
                catch
                {
                    MessageBox.Show(" Une erreur s'est produite!!", "ERREUR!!");
                }
            }
            else
            {
                MessageBox.Show(" Veuillez saisir tous les champs!", "INFORMATION!!");
            }

        }

        /// <summary>
        /// modifier un dvd dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierDvd_Click_1(object sender, EventArgs e)
        {
            if (dgvDvdListe.SelectedRows.Count > 0)
            {
                Dvd selectDvd = (Dvd)dgvDvdListe.SelectedRows[0].DataBoundItem;
                string id = selectDvd.Id;
                string titre = txbDvdTitre.Text;
                string image = txbDvdImage.Text;
                int duree = int.Parse(txbDvdDuree.Text);
                string realisateur = txbDvdRealisateur.Text;
                string synopsis = txbDvdSynopsis.Text;
                string idGenre = GetIdGenreDocument(cbxNewGenreDvd.Text);
                string idPublic = GetIdPublicDocument(cbxNewPublicDvd.Text);
                string idRayon = GetIdRayonDocument(cbxNewRayonDvd.Text);
                if (!txbDvdNumero.Text.Equals("") && !txbDvdTitre.Text.Equals("") && !txbDvdRealisateur.Text.Equals(" ") && !txbDvdSynopsis.Text.Equals("") && !cbxNewGenreDvd.Text.Equals("") && !cbxNewPublicDvd.Text.Equals("") && !cbxNewRayonDvd.Text.Equals(""))
                {
                    if (controller.EditDocument(id, titre, image, idGenre, idPublic, idRayon) && controller.EditDvd(id, duree, realisateur, synopsis))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + titre + " est bien modifié!");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du dvd", "ERREUR!!");

                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir tous les champs!!", "INFORMATION!!");
                }

            }
            else
            {
                MessageBox.Show("Veuillez selectioner une ligne!!", "INFORMATION!!");
            }

        }

        /// <summary>
        /// supprimer un dvd dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerDvd_Click_1(object sender, EventArgs e)
        {
            Dvd dvd = (Dvd)bdgDvdListe.Current;
            if (MessageBox.Show("êtes vous sûr de vouloir supprimer ce" + dvd.Titre + "?", "CONFIRMATION!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var exmpDvd = controller.GetExemplairesDocument(dvd.Id);
                var noExmpl = !exmpDvd.Any();
                var commandeDvd = controller.GetCommandeDocument(dvd.Id);
                var noCommande = !commandeDvd.Any();
                if (noCommande && noExmpl)
                {
                    if (controller.DeleteDvd(dvd.Id))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + dvd.Titre + " est supprimer!");
                    }
                    else
                    {
                        MessageBox.Show("erreur!!", "ERREUR!");
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de supprimer ce dvd car il possède un ou plusieurs exemplaire(s), ou une ou plusieurs commande(s)", "ERREUR!!");
                }
            }

        }

        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
            RemplirCbxNewPublicRevue();
            RemplirCbxNewRayonRevue();
            RemplirCbxNewGenreRevue();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        /// <summary>
        /// remplir la combobox du genre de la revue a ajouter ou a modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewGenreRevue()
        {
            List<Categorie> LesGenresRevue = controller.GetAllGenres();
            foreach (Categorie genre in LesGenresRevue)
            {
                cbxNewGenreRevue.Items.Add(genre.Libelle);
            }
            if (cbxNewGenreRevue.Items.Count > 0)
            {
                cbxNewGenreRevue.SelectedIndex = 0;
            }
            return cbxNewGenreRevue.SelectedItem?.ToString();
        }
        /// <summary>
        /// remplir la combobox du public de la revue a ajouter ou modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewPublicRevue()
        {
            List<Categorie> LesPublicsRevue = controller.GetAllPublics();
            foreach (Categorie lePublic in LesPublicsRevue)
            {
                cbxNewPublicRevue.Items.Add(lePublic.Libelle);
            }
            if (cbxNewPublicRevue.Items.Count > 0)
            {
                cbxNewPublicRevue.SelectedIndex = 0;
            }
            return cbxNewPublicRevue.SelectedItem?.ToString();
        }
        /// <summary>
        /// remplir la combobox du rayon de la revue a ajouter ou modifier
        /// </summary>
        /// <returns></returns>
        private string RemplirCbxNewRayonRevue()
        {
            List<Categorie> LesRayonsRevue = controller.GetAllRayons();
            foreach (Categorie rayon in LesRayonsRevue)
            {
                cbxNewRayonRevue.Items.Add(rayon.Libelle);
            }
            if (cbxNewRayonRevue.Items.Count > 0)
            {
                cbxNewRayonRevue.SelectedIndex = 0;
            }
            return cbxNewRayonRevue.SelectedItem?.ToString();
        }
        /// <summary>
        /// vider revues informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnViderInfosRevue_Click(object sender, EventArgs e)
        {
            VideRevuesInfos();
        }
        /// <summary>
        /// ajouter une revue 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterNewRevue_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumero.Text.Equals(" ") && !txbRevuesTitre.Text.Equals(" ") && !txbRevuesPeriodicite.Text.Equals(" ") && !txbRevuesDateMiseADispo.Text.Equals("") && !cbxNewGenreRevue.Text.Equals(" ") && !cbxNewPublicRevue.Text.Equals(" ") && !cbxNewRayonRevue.Text.Equals(" "))
            {
                try
                {
                    string id = txbRevuesNumero.Text;
                    string titre = txbRevuesTitre.Text;
                    string image = txbRevuesImage.Text;
                    string periodicite = txbRevuesPeriodicite.Text;
                    int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                    string idGenre = GetIdGenreDocument(cbxNewGenreRevue.Text);
                    string idPublic = GetIdPublicDocument(cbxNewPublicRevue.Text);
                    string idRayon = GetIdRayonDocument(cbxNewRayonRevue.Text);
                    string genre = txbRevuesGenre.Text;
                    string lePublic = txbRevuesPublic.Text;
                    string rayon = txbRevuesRayon.Text;
                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);
                    var idRevueExist = controller.GetAllDocuments(id);
                    var idRevueNoExist = !idRevueExist.Any();

                    if (idRevueNoExist)
                    {
                        if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdGenre, document.IdPublic, document.IdRayon) && controller.CreerRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo))
                        {
                            lesRevues = controller.GetAllRevues();
                            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                            RemplirRevuesListeComplete();
                            MessageBox.Show(" La revue " + titre + " est bien ajouté ");
                        }
                    }
                    else
                    {
                        MessageBox.Show(" Le numéro de document existe déjà! saisir un autre numéro! ", "ERREUR!!");
                    }

                }
                catch
                {
                    MessageBox.Show(" Une erreur s'est produite!!", "ERREUR!!");
                }
            }
            else
            {
                MessageBox.Show(" Veuillez saisir tous les champs!", "INFORMATION!!");
            }
        }

        /// <summary>
        /// modifier une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierRevue_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.SelectedRows.Count > 0)
            {
                Revue selectRevue = (Revue)dgvRevuesListe.SelectedRows[0].DataBoundItem;
                string id = selectRevue.Id;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string periodidcite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                string idGenre = GetIdGenreDocument(cbxNewGenreRevue.Text);
                string idPublic = GetIdPublicDocument(cbxNewPublicRevue.Text);
                string idRayon = GetIdRayonDocument(cbxNewRayonRevue.Text);
                if (!txbRevuesNumero.Text.Equals("") && !txbRevuesTitre.Text.Equals("") && !txbRevuesPeriodicite.Text.Equals("") && !txbRevuesDateMiseADispo.Text.Equals("") && !cbxNewGenreRevue.Text.Equals("") && !cbxNewPublicRevue.Text.Equals("") && !cbxNewRayonRevue.Text.Equals(""))
                {
                    if (controller.EditDocument(id, titre, image, idGenre, idPublic, idRayon) && controller.EditRevue(id, periodidcite, delaiMiseADispo))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + titre + " est bien modifié!");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification de la revue", "ERREUR!!");

                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir tous les champs!!", "INFORMATION!!");
                }

            }
            else
            {
                MessageBox.Show("Veuillez selectioner une ligne!!", "INFORMATION!!");
            }
        }

        /// <summary>
        /// supprimer une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerRevue_Click(object sender, EventArgs e)
        {
            Revue revue = (Revue)bdgRevuesListe.Current;
            if (MessageBox.Show("êtes vous sûr de vouloir supprimer cette" + revue.Titre + "?", "CONFIRMATION!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var exmpRevue = controller.GetExemplairesDocument(revue.Id);
                var noExmpl = !exmpRevue.Any();
                var commandeRevue = controller.GetCommandeDocument(revue.Id);
                var noCommande = !commandeRevue.Any();
                if (noCommande && noExmpl)
                {
                    if (controller.DeleteRevue(revue.Id))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + revue.Titre + " est supprimer!");
                    }
                    else
                    {
                        MessageBox.Show("erreur!!", "ERREUR!");
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de supprimer cette revue car il possède un ou plusieurs exemplaire(s), ou une ou plusieurs commande(s)", "ERREUR!!");
                }
            }

        }

        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    string libelle = "";
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                    //if (controller.CreerExemplaire(exemplaire))
                    if (controller.CreerExemplaireRevue(idDocument, numero, dateAchat, photo, idEtat))

                        {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }








        #endregion

        #region Onglet CommandesLivres
        private readonly BindingSource bdgCommandesLivre = new BindingSource();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private List<Suivi> lesSuivis = new List<Suivi>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de livres :
        /// appel des méthodes pour remplir le datagrid des commandes de livre et du combo "suivi"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCmdLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesSuivis = controller.GetAllSuivis();
            gbxInfosCommandeLivre.Enabled = false;
            gbxEtapeSuivi.Enabled = false;
            // Initialisation du ComboBox avec "en cours" par défaut
            RemplirCbxCommandeLivreLibelleSuivi(null);
        }
        
        private void RemplirCommandesLivresListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesLivre.DataSource = lesCommandesDocument;
                dgvListeCmdLivre.DataSource = bdgCommandesLivre;
                dgvListeCmdLivre.Columns["id"].Visible = false;
                dgvListeCmdLivre.Columns["idLivreDvd"].Visible = false;
                dgvListeCmdLivre.Columns["idSuivi"].Visible = false;
                dgvListeCmdLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvListeCmdLivre.Columns["dateCommande"].DisplayIndex = 4;
                dgvListeCmdLivre.Columns["montant"].DisplayIndex = 1;
                dgvListeCmdLivre.Columns[5].HeaderCell.Value = "Date de commande";
                dgvListeCmdLivre.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvListeCmdLivre.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesLivre.DataSource = null;
            }
        }
        /// <summary>
        /// Mise à jour de la liste des commandes de livre
        /// </summary>
       
        private void AfficheReceptionCommandesLivre()
        {
            string idDocument = txtbNumCmdLivreRecherche.Text.Trim();
            lesCommandesDocument = controller.GetCommandeDocument(idDocument);

            MessageBox.Show($"ID recherché : {idDocument}\nCommandes trouvées : {lesCommandesDocument?.Count}");

            if (lesCommandesDocument == null || lesCommandesDocument.Count == 0)
            {
                MessageBox.Show("Aucune commande trouvée pour ce document.");
            }

            RemplirCommandesLivresListe(lesCommandesDocument);
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNumCmdLivreRecherche_Click(object sender, EventArgs e)
        {
            if (!txtbNumCmdLivreRecherche.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txtbNumCmdLivreRecherche.Text));
                if (livre != null)
                {
                    AfficheReceptionCommandesLivre();
                    gbxInfosCmdLivre.Enabled = true;
                    AfficheReceptionCommandesLivreInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }
        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">Le livre</param>
        private void AfficheReceptionCommandesLivreInfos(Livre livre)
        {
            txbTitreCmdLivre.Text = livre.Titre;
            txbAuteurCmdLivre.Text = livre.Auteur;
            txbIsbnCmdLivre.Text = livre.Isbn;
            txbCollectionCmdLivre.Text = livre.Collection;
            txbGenreCmdLivre.Text = livre.Genre;
            txbPublicCmdLivre.Text = livre.Public;
            txbRayonCmdLivre.Text = livre.Rayon;
            txbCheminImgCmdLivre.Text = livre.Image;
            string image = livre.Image;
            try
            {
                pictureBoxCmdLivre.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBoxCmdLivre.Image = null;
            }
            AfficheReceptionCommandesLivre();
        }
       
        /// <summary>
        /// Remplissage de la comboBox selon les étapes de suivi et le libelle correspondant
        /// </summary>
        /// <param name="etapeSuivi"></param>
       
        private void RemplirCbxCommandeLivreLibelleSuivi(string etapeSuivi)
        {
            cbxEtapeSuiviCmdLivre.Items.Clear();

            if (modeAjoutCommande)
            {
                cbxEtapeSuiviCmdLivre.Items.Add("en cours");
                cbxEtapeSuiviCmdLivre.SelectedIndex = 0;
                cbxEtapeSuiviCmdLivre.Enabled = false;
                return;
            }

            List<Suivi> tousLesSuivis = controller.GetAllSuivis();

            if (!string.IsNullOrEmpty(etapeSuivi))
                etapeSuivi = etapeSuivi.Trim().ToLower();

            List<string> transitionsPossibles = new List<string>();

            switch (etapeSuivi)
            {
                case "en cours":
                    transitionsPossibles.AddRange(new[] { "relancée", "livrée" });
                    break;
                case "relancée":
                    transitionsPossibles.AddRange(new[] { "en cours", "livrée" });

                    break;
                case "livrée":
                    transitionsPossibles.Add("réglée");
                    break;
            }

            foreach (var suivi in tousLesSuivis)
            {
                if (transitionsPossibles.Contains(suivi.Libelle.Trim().ToLower()))
                {
                    cbxEtapeSuiviCmdLivre.Items.Add(suivi.Libelle);
                }
            }

            if (cbxEtapeSuiviCmdLivre.Items.Count > 0)
            {
                cbxEtapeSuiviCmdLivre.SelectedIndex = 0;
                cbxEtapeSuiviCmdLivre.Enabled = true;
            }
            else
            {
                cbxEtapeSuiviCmdLivre.Items.Add("Aucune transition possible");
                cbxEtapeSuiviCmdLivre.SelectedIndex = 0;
                cbxEtapeSuiviCmdLivre.Enabled = false;
            }
        }

       
        private void dgvListeCmdLivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Vérifie qu'une ligne valide est sélectionnée
            {
                DataGridViewRow row = dgvListeCmdLivre.Rows[e.RowIndex];

                string id = row.Cells["Id"].Value.ToString();
                DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
                double montant = double.Parse(row.Cells["Montant"].Value.ToString());
                int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
                string libelle = row.Cells["Libelle"].Value.ToString();

                txbNumNewCmdLivre.Text = id;
                txbNbExemplCmdLivre.Text = nbExemplaire.ToString();
                txbMontantCmdLivre.Text = montant.ToString();
                dateTimePickerCmdLivre.Value = dateCommande;
                labelEtapeSuiviCmdLivre.Text = libelle;

                if (GetIdSuivi(libelle) == "003") // "réglée"
                {
                    cbxEtapeSuiviCmdLivre.Enabled = false;
                    btnModifierEtapeSuiviCmdLivre.Enabled = false;
                }
                else
                {
                    cbxEtapeSuiviCmdLivre.Enabled = true;
                    btnModifierEtapeSuiviCmdLivre.Enabled = true;
                    RemplirCbxCommandeLivreLibelleSuivi(libelle);
                }
            }
            else
            {
                // Aucune ligne sélectionnée - réinitialise avec les valeurs par défaut
                RemplirCbxCommandeLivreLibelleSuivi(null);
            }
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeCmdLivre_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvListeCmdLivre.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxInfosCommandeLivre_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
        }

      
        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxEtapeSuivi_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeLivre.Enabled = false;
            txtbNumCmdLivreRecherche.Enabled = false;
        }
        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEtapeSuiviAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
            gbxInfosCommandeLivre.Enabled = true;
            txtbNumCmdLivreRecherche.Enabled = true;
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierEtapeSuiviCmdLivre_Click(object sender, EventArgs e)
        {
            if (dgvListeCmdLivre.CurrentRow == null)
            {
                MessageBox.Show("Veuillez sélectionner une commande dans la liste.");
                return;
            }

            string id = dgvListeCmdLivre.CurrentRow.Cells["id"].Value.ToString();

            if (cbxEtapeSuiviCmdLivre.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une nouvelle étape de suivi.");
                return;
            }

            string idSuivi = GetIdSuivi(cbxEtapeSuiviCmdLivre.Text);
            string libelle = cbxEtapeSuiviCmdLivre.Text;

            if (MessageBox.Show($"Voulez-vous modifier le suivi de la commande {id} en {libelle} ?",
                                "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // ✅ Appel simplifié du controller
                bool ok = controller.EditSuiviCommandeDocument(id, idSuivi);

                if (ok)
                {
                    MessageBox.Show($"L'étape de suivi de la commande {id} a bien été modifiée.", "Information");
                    AfficheReceptionCommandesLivre();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la mise à jour du suivi.");
                }

                cbxEtapeSuiviCmdLivre.Items.Clear();
                cbxEtapeSuiviCmdLivre.Text = "";
            }
        }
        /// <summary>
        /// Suppression d'une commande dans la base de données
        /// Si elle n'a pas encore été livrée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSuppCmdLivre_Click(object sender, EventArgs e)
        {
            if (dgvListeCmdLivre.SelectedRows.Count > 0)
            {
                CommandeDocument commandedocument = (CommandeDocument)bdgCommandesLivre.List[bdgCommandesLivre.Position];
                if (commandedocument.Libelle == "en cours" || commandedocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandedocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.DeleteCommandeDocument(commandedocument);
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée, elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
       
       
        /// <summary>
        /// Récupère l'ID du suivi à partir de son libellé
        /// </summary>
        /// <param name="libelle">Libellé de l'étape de suivi (ex: "en cours")</param>
        /// <returns>ID correspondant ou chaîne vide si non trouvé</returns>
        private string GetIdSuivi(string libelle)
        {
            List<Suivi> lesSuivis = controller.GetAllSuivis();

            foreach (Suivi suivi in lesSuivis)
            {
                if (suivi.Libelle.ToLower() == libelle.ToLower())
                {
                    return suivi.Id;
                }
            }

            MessageBox.Show("Impossible de trouver l'ID pour le libellé de suivi : " + libelle, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return "";
        }
      
        private void btnEnregistrerCommandeLivre_Click_1(object sender, EventArgs e)
        {
            string id = txbNumNewCmdLivre.Text.Trim();
            string idLivreDvd = txtbNumCmdLivreRecherche.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(idLivreDvd))
            {
                MessageBox.Show("Veuillez saisir tous les champs obligatoires.");
                return;
            }

            if (!int.TryParse(txbNbExemplCmdLivre.Text, out int nbExemplaire) ||
                !double.TryParse(txbMontantCmdLivre.Text, out double montant))
            {
                MessageBox.Show("Montant ou nombre d'exemplaires invalide.");
                return;
            }

            DateTime dateCommande = dateTimePickerCmdLivre.Value;

            // Étape de suivi forcée à "en cours"
            string libelle = "en cours";
            string idSuivi = GetIdSuivi(libelle);

            // Création de la commande (table "commande")
            Commande commande = new Commande(id, dateCommande, montant);
            bool commandeCreee = controller.CreerCommande(commande);

            if (!commandeCreee)
            {
                MessageBox.Show("❌ Erreur lors de la création de la commande.");
                return;
            }

            // Création du lien vers le document (table "commandedocument")
            bool docCreee = controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);

            if (docCreee)
            {
                MessageBox.Show("✅ Commande enregistrée avec succès !");
                AfficheReceptionCommandesLivre(); // mise à jour affichage
                cbxEtapeSuiviCmdLivre.Items.Clear(); // reset dropdown
            }
            else
            {
                MessageBox.Show("⚠️ Commande créée, mais erreur lors de l'enregistrement du document.");
            }
            txtbNumCmdLivreRecherche.Text = idLivreDvd;
            AfficheReceptionCommandesLivre();

        }
       
        private void btnNouvelleCommandeLivre_Click_1(object sender, EventArgs e)
        {
            modeAjoutCommande = true;

            RemplirCbxCommandeLivreLibelleSuivi(""); // Va forcer "en cours"
            lblEtapeSuivi.Text = "Suivi : en cours";
            lblEtapeSuivi.ForeColor = Color.Orange;
            lblEtapeSuivi.Visible = true;
        }


        private void dgvListeCmdLivre_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("✅ Sélection changée !");

            if (dgvListeCmdLivre.CurrentRow == null)
                return;

            modeAjoutCommande = false;

            string libelleSuivi = "";

            try
            {
                if (dgvListeCmdLivre.Columns.Contains("libelle") &&
                    dgvListeCmdLivre.CurrentRow.Cells["libelle"].Value != null)
                {
                    libelleSuivi = dgvListeCmdLivre.CurrentRow.Cells["libelle"].Value.ToString();
                }
                else
                {
                    Console.WriteLine("⚠️ 'libelle' manquant ou vide");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur lors de la lecture de 'libelle' : " + ex.Message);
            }

            RemplirCbxCommandeLivreLibelleSuivi(libelleSuivi);
            AfficherEtapeSuivi(libelleSuivi);
        }

        private void AfficherEtapeSuivi(string etape)
        {
            if (lblEtapeSuivi == null)
                return;

            lblEtapeSuivi.Visible = true;

            if (string.IsNullOrEmpty(etape))
            {
                lblEtapeSuivi.Text = "Suivi : inconnu";
                lblEtapeSuivi.ForeColor = Color.Gray;
                return;
            }

            lblEtapeSuivi.Text = "Suivi : " + etape;

            switch (etape.ToLower().Trim())
            {
                case "en cours":
                    lblEtapeSuivi.ForeColor = Color.Orange;
                    break;
                case "relancée":
                    lblEtapeSuivi.ForeColor = Color.DarkOrange;
                    break;
                case "livrée":
                    lblEtapeSuivi.ForeColor = Color.Green;
                    break;
                case "réglée":
                    lblEtapeSuivi.ForeColor = Color.Blue;
                    break;
                default:
                    lblEtapeSuivi.ForeColor = Color.Gray;
                    break;
            }
        }

        #endregion
        #region Onglet CommandesDvd
        private readonly BindingSource bdgCommandesDvd = new BindingSource();
       
        /// <summary>
        /// Ouverture de l'onglet Commandes de livres :
        /// appel des méthodes pour remplir le datagrid des commandes de livre et du combo "suivi"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCmdDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            lesSuivis = controller.GetAllSuivis();
            gbxInfosCommandeDvd.Enabled = false;
            gbxEtapeSuivi.Enabled = false;
            // Initialisation du ComboBox avec "en cours" par défaut
            RemplirCbxCommandeDvdLibelleSuivi(null);
        }

        private void RemplirCommandesDvdListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesDvd.DataSource = lesCommandesDocument;
                dgvListeCmdDvd.DataSource = bdgCommandesDvd;
                dgvListeCmdDvd.Columns["id"].Visible = false;
                dgvListeCmdDvd.Columns["idLivreDvd"].Visible = false;
                dgvListeCmdDvd.Columns["idSuivi"].Visible = false;
                dgvListeCmdDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvListeCmdDvd.Columns["dateCommande"].DisplayIndex = 4;
                dgvListeCmdDvd.Columns["montant"].DisplayIndex = 1;
                dgvListeCmdDvd.Columns[5].HeaderCell.Value = "Date de commande";
                dgvListeCmdDvd.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvListeCmdDvd.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesDvd.DataSource = null;
            }
        }
        /// <summary>
        /// Mise à jour de la liste des commandes de livre
        /// </summary>
        private void AfficheReceptionCommandesDvd()
        {
            string idDocument = txtbNumCmdDvdRecherche.Text.Trim();
            lesCommandesDocument = controller.GetCommandeDocument(idDocument);

            // 💡 Filtrage uniquement des commandes DVD
            if (lesCommandesDocument != null)
            {
                lesCommandesDocument = lesCommandesDocument
                    .Where(c => lesDvd.Any(d => d.Id == c.IdLivreDvd))
                    .ToList();
            }

            RemplirCommandesDvdListe(lesCommandesDocument);
        }


        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnNumCmdDvdRecherche_Click_1(object sender, EventArgs e)
        {
            if (!txtbNumCmdDvdRecherche.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txtbNumCmdDvdRecherche.Text));
                if (dvd != null)
                {
                    AfficheReceptionCommandesDvd();
                    gbxInfosCmdDvd.Enabled = true;
                    AfficheReceptionCommandesDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }

        }
        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">Le livre</param>
        private void AfficheReceptionCommandesDvdInfos(Dvd dvd)
        {
            txbTitreCmdDvd.Text = dvd.Titre;
            txbRealisateurDvdCmd.Text = dvd.Realisateur;
            txbDureeDvdCmd.Text = dvd.Duree.ToString();
            txbSynopsisDvdCmd.Text = dvd.Synopsis;
            txbGenreDvdCmd.Text = dvd.Genre;
            txbPublicDvdCmd.Text = dvd.Public;
            txbRayonDvdCmd.Text = dvd.Rayon;
            txbCheminImageDvdCmd.Text = dvd.Image;
            string image = dvd.Image;
            try
            {
                pictBoxCmdDvd.Image = Image.FromFile(image);
            }
            catch
            {
                pictBoxCmdDvd.Image = null;
            }
            AfficheReceptionCommandesDvd();
        }

        /// <summary>
        /// Remplissage de la comboBox selon les étapes de suivi et le libelle correspondant
        /// </summary>
        /// <param name="etapeSuivi"></param>

        private void RemplirCbxCommandeDvdLibelleSuivi(string etapeSuivi)
        {
            cbxEtapeSuiviCmdDvd.Items.Clear();

            if (modeAjoutCommande)
            {
                cbxEtapeSuiviCmdDvd.Items.Add("en cours");
                cbxEtapeSuiviCmdDvd.SelectedIndex = 0;
                cbxEtapeSuiviCmdDvd.Enabled = false;
                return;
            }

            List<Suivi> tousLesSuivis = controller.GetAllSuivis();

            if (!string.IsNullOrEmpty(etapeSuivi))
                etapeSuivi = etapeSuivi.Trim().ToLower();

            List<string> transitionsPossibles = new List<string>();

            switch (etapeSuivi)
            {
                case "en cours":
                    transitionsPossibles.AddRange(new[] { "relancée", "livrée" });
                    break;
                case "relancée":
                    transitionsPossibles.AddRange(new[] { "en cours", "livrée" });

                    break;
                case "livrée":
                    transitionsPossibles.Add("réglée");
                    break;
            }

            foreach (var suivi in tousLesSuivis)
            {
                if (transitionsPossibles.Contains(suivi.Libelle.Trim().ToLower()))
                {
                    cbxEtapeSuiviCmdDvd.Items.Add(suivi.Libelle);
                }
            }

            if (cbxEtapeSuiviCmdDvd.Items.Count > 0)
            {
                cbxEtapeSuiviCmdDvd.SelectedIndex = 0;
                cbxEtapeSuiviCmdDvd.Enabled = true;
            }
            else
            {
                cbxEtapeSuiviCmdDvd.Items.Add("Aucune transition possible");
                cbxEtapeSuiviCmdDvd.SelectedIndex = 0;
                cbxEtapeSuiviCmdDvd.Enabled = false;
            }
        }


        private void dgvListeCmdDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Vérifie qu'une ligne valide est sélectionnée
            {
                DataGridViewRow row = dgvListeCmdDvd.Rows[e.RowIndex];

                string id = row.Cells["Id"].Value.ToString();
                DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
                double montant = double.Parse(row.Cells["Montant"].Value.ToString());
                int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
                string libelle = row.Cells["Libelle"].Value.ToString();

                txbNumNewCmdDvd.Text = id;
                txbNbExemplaireCmdDvd.Text = nbExemplaire.ToString();
                txbMontantCmdDvd.Text = montant.ToString();
                dateTimePickerCmdDvd.Value = dateCommande;
                lblEtapeSuiviDvd.Text = libelle;

                if (GetIdSuivi(libelle) == "003") // "réglée"
                {
                    cbxEtapeSuiviCmdDvd.Enabled = false;
                    btnModifierEtapeSuiviCmdDvd.Enabled = false;
                }
                else
                {
                    cbxEtapeSuiviCmdDvd.Enabled = true;
                    btnModifierEtapeSuiviCmdDvd.Enabled = true;
                    RemplirCbxCommandeDvdLibelleSuivi(libelle);
                }
            }
            else
            {
                // Aucune ligne sélectionnée - réinitialise avec les valeurs par défaut
                RemplirCbxCommandeDvdLibelleSuivi(null);
            }
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeCmdDvd_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvListeCmdDvd.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxInfosCommandeDvd_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
        }


        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxEtapeSuiviDvd_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeDvd.Enabled = false;
            txtbNumCmdDvdRecherche.Enabled = false;
        }
        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      
        private void btnRetourEtapeSuiviCmdDvd_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
            gbxInfosCommandeDvd.Enabled = true;
            txtbNumCmdDvdRecherche.Enabled = true;
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierEtapeSuiviCmdDvd_Click_1(object sender, EventArgs e)
        {
            if (dgvListeCmdDvd.CurrentRow == null)
            {
                MessageBox.Show("Veuillez sélectionner une commande dans la liste.");
                return;
            }

            string id = dgvListeCmdDvd.CurrentRow.Cells["id"].Value.ToString();

            if (cbxEtapeSuiviCmdDvd.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une nouvelle étape de suivi.");
                return;
            }

            string idSuivi = GetIdSuivi(cbxEtapeSuiviCmdDvd.Text);
            string libelle = cbxEtapeSuiviCmdDvd.Text;

            if (MessageBox.Show($"Voulez-vous modifier le suivi de la commande {id} en {libelle} ?",
                                "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // ✅ Appel simplifié du controller
                bool ok = controller.EditSuiviCommandeDocument(id, idSuivi);

                if (ok)
                {
                    MessageBox.Show($"L'étape de suivi de la commande {id} a bien été modifiée.", "Information");
                    AfficheReceptionCommandesDvd();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la mise à jour du suivi.");
                }

                cbxEtapeSuiviCmdDvd.Items.Clear();
                cbxEtapeSuiviCmdDvd.Text = "";
            }

        }
       
        /// <summary>
        /// Suppression d'une commande dans la base de données
        /// Si elle n'a pas encore été livrée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      
        private void btnSupprimerCommandeDvd_Click_1(object sender, EventArgs e)
        {
            if (dgvListeCmdDvd.SelectedRows.Count > 0)
            {
                CommandeDocument commandedocument = (CommandeDocument)bdgCommandesDvd.List[bdgCommandesDvd.Position];
                if (commandedocument.Libelle == "en cours" || commandedocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandedocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.DeleteCommandeDocument(commandedocument);
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée, elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
        private void btnEnregistrerCommandeDvd_Click(object sender, EventArgs e)
        {
            string id = txbNumNewCmdDvd.Text.Trim();
            string idLivreDvd = txtbNumCmdDvdRecherche.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(idLivreDvd))
            {
                MessageBox.Show("Veuillez saisir tous les champs obligatoires.");
                return;
            }

            if (!int.TryParse(txbNbExemplaireCmdDvd.Text, out int nbExemplaire) ||
                !double.TryParse(txbMontantCmdDvd.Text, out double montant))
            {
                MessageBox.Show("Montant ou nombre d'exemplaires invalide.");
                return;
            }

            DateTime dateCommande = dateTimePickerCmdDvd.Value;

            // Étape de suivi forcée à "en cours"
            string libelle = "en cours";
            string idSuivi = GetIdSuivi(libelle);

            // Création de la commande (table "commande")
            Commande commande = new Commande(id, dateCommande, montant);
            bool commandeCreee = controller.CreerCommande(commande);

            if (!commandeCreee)
            {
                MessageBox.Show(" Erreur lors de la création de la commande.");
                return;
            }

            // Création du lien vers le document (table "commandedocument")
            bool docCreee = controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);

            if (docCreee)
            {
                MessageBox.Show(" Commande enregistrée avec succès !");
                AfficheReceptionCommandesDvd(); // mise à jour affichage
                cbxEtapeSuiviCmdDvd.Items.Clear(); // reset dropdown
            }
            else
            {
                MessageBox.Show(" Commande créée, mais erreur lors de l'enregistrement du document.");
            }
            txtbNumCmdDvdRecherche.Text = idLivreDvd;
            AfficheReceptionCommandesDvd();

        }
      
        private void btnNouvelleCommandeDvd_Click(object sender, EventArgs e)
        {
            modeAjoutCommande = true;

            RemplirCbxCommandeDvdLibelleSuivi(""); // Va forcer "en cours"
            lblEtapeSuiviDvd.Text = "Suivi : en cours";
            lblEtapeSuiviDvd.ForeColor = Color.Orange;
            lblEtapeSuiviDvd.Visible = true;
        }


        private void dgvListeCmdDvd_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Sélection changée !");

            if (dgvListeCmdDvd.CurrentRow == null)
                return;

            modeAjoutCommande = false;

            string libelleSuivi = "";

            try
            {
                if (dgvListeCmdDvd.Columns.Contains("libelle") &&
                    dgvListeCmdDvd.CurrentRow.Cells["libelle"].Value != null)
                {
                    libelleSuivi = dgvListeCmdDvd.CurrentRow.Cells["libelle"].Value.ToString();
                }
                else
                {
                    Console.WriteLine(" 'libelle' manquant ou vide");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Erreur lors de la lecture de 'libelle' : " + ex.Message);
            }

            RemplirCbxCommandeDvdLibelleSuivi(libelleSuivi);
            AfficherEtapeSuiviDvd(libelleSuivi);
        }

        private void AfficherEtapeSuiviDvd(string etape)
        {
            if (lblEtapeSuiviDvd == null)
                return;

            lblEtapeSuiviDvd.Visible = true;

            if (string.IsNullOrEmpty(etape))
            {
                lblEtapeSuiviDvd.Text = "Suivi : inconnu";
                lblEtapeSuiviDvd.ForeColor = Color.Gray;
                return;
            }

            lblEtapeSuiviDvd.Text = "Suivi : " + etape;

            switch (etape.ToLower().Trim())
            {
                case "en cours":
                    lblEtapeSuiviDvd.ForeColor = Color.Orange;
                    break;
                case "relancée":
                    lblEtapeSuiviDvd.ForeColor = Color.DarkOrange;
                    break;
                case "livrée":
                    lblEtapeSuiviDvd.ForeColor = Color.Green;
                    break;
                case "réglée":
                    lblEtapeSuiviDvd.ForeColor = Color.Blue;
                    break;
                default:
                    lblEtapeSuiviDvd.ForeColor = Color.Gray;
                    break;
            }
        }


        #endregion
        #region Onglet CommandesRevues

        private readonly BindingSource bdgAbonnementsRevue = new BindingSource();
        private List<Abonnement> lesAbonnementsRevue = new List<Abonnement>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de revues :
        /// appel des méthodes pour remplir le datagrid des abonnements d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            gbxInfosCommandeRevue.Enabled = false;
            dateTimePickerCmdDvd.Value = DateTime.Now;
        }

        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesAbonnements"></param>
        private void RemplirAbonnementsRevueListe(List<Abonnement> lesAbonnementsRevue)
        {
            if (lesAbonnementsRevue != null)
            {
                bdgAbonnementsRevue.DataSource = lesAbonnementsRevue;
                dgvAbonnementsRevue.DataSource = bdgAbonnementsRevue;
                dgvAbonnementsRevue.Columns["id"].Visible = false;
                dgvAbonnementsRevue.Columns["idRevue"].Visible = false;
                dgvAbonnementsRevue.Columns["titre"].Visible = false;
                dgvAbonnementsRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAbonnementsRevue.Columns["dateCommande"].DisplayIndex = 0;
                dgvAbonnementsRevue.Columns["montant"].DisplayIndex = 1;
                dgvAbonnementsRevue.Columns[4].HeaderCell.Value = "Date de commande";
                dgvAbonnementsRevue.Columns[0].HeaderCell.Value = "Date de fin d'abonnement";
                

            }
            else
            {
                bdgAbonnementsRevue.DataSource = null;
            }
            foreach (DataGridViewColumn col in dgvAbonnementsRevue.Columns)
            {
                Console.WriteLine("Colonne DGV : " + col.Name);
            }
            try
            {
                bdgAbonnementsRevue.DataSource = lesAbonnementsRevue;
                dgvAbonnementsRevue.DataSource = bdgAbonnementsRevue;
                Console.WriteLine("✔ DGV alimentée avec " + lesAbonnementsRevue.Count + " éléments.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur DGV : " + ex.Message);
            }


        }

        /// <summary>
        /// Affiche la liste des abonnements d'une revue
        /// </summary>
        //private void AfficheReceptionAbonnementsRevue()
        //{
        //    string idDocument = txtbNumCmdRevueRecherche.Text;
        //    lesAbonnementsRevue = controller.GetAbonnementRevue(idDocument);
        //    RemplirAbonnementsRevueListe(lesAbonnementsRevue);
        //    foreach (var abo in lesAbonnementsRevue)
        //    {
        //        Console.WriteLine($"[DEBUG ABO] id={abo.Id}, dateCommande={abo.DateCommande}, montant={abo.Montant}, dateFin={abo.DateFinAbonnement}, idRevue={abo.IdRevue}");
        //    }

        //}
        private void AfficheReceptionAbonnementsRevue()
        {
            string idRevue = txtbNumCmdRevueRecherche.Text;
            lesAbonnementsRevue = controller.GetAbonnementRevue(idRevue);

            dgvAbonnementsRevue.DataSource = null;
            dgvAbonnementsRevue.DataSource = lesAbonnementsRevue;
        }


        /// <summary>
        /// Recherche d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnNumCmdRevueRecherche_Click(object sender, EventArgs e)
        {
            if (!txtbNumCmdRevueRecherche.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txtbNumCmdRevueRecherche.Text));
                if (revue != null)
                {
                    AfficheReceptionAbonnementsRevue();
                    gbxInfosCommandeRevue.Enabled = true;
                    AfficheReceptionAbonnementsRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Ce numéro de revue n'existe pas.");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de revue est obligatoire.");
            }
        }

        /// <summary>
        /// Affichage des informations d'une revue
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionAbonnementsRevueInfos(Revue revue)
        {
            txbTitreCmdRevue.Text = revue.Titre;
            txbPeriodiciteCmdRevue.Text = revue.Periodicite;
            txbDelaiMiseADispoCmdRevue.Text = revue.DelaiMiseADispo.ToString();
            txbGenreCmdRevue.Text = revue.Genre;
            txbPublicCmdRevue.Text = revue.Public;
            txbRayonCmdRevue.Text = revue.Rayon;
            txbCheminImageCmdRevue.Text = revue.Image;
            string image = revue.Image;
            try
            {
                pictureBoxCmdRevue.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBoxCmdRevue.Image = null;
            }
            AfficheReceptionAbonnementsRevue();
            
        }
        /// <summary>
        /// Affichage des informations de l'abonnement sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvAbonnementsRevue.Rows[e.RowIndex];
            string id = row.Cells["Id"].Value.ToString();
           // DateTime dateCommande = (DateTime)row.Cells["DateCommande"].Value;
            if (DateTime.TryParse(row.Cells["DateCommande"].Value?.ToString(), out DateTime dateCommande))
            {
                dateTimePickerCmdRevue.Value = dateCommande;
            }

            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            DateTime dateFinAbonnement = (DateTime)row.Cells["DateFinAbonnement"].Value;
            txbNumNewCmdRevue.Text = id;
            txbMontantCmdRevue.Text = montant.ToString();
            dateTimePickerCmdRevue.Value = dateCommande;
            dateTimePickerDateFinAbnmntCmdRevue.Value = dateFinAbonnement;
            foreach (DataGridViewColumn col in dgvAbonnementsRevue.Columns)
            {
                Console.WriteLine($"Col: {col.Index} -> {col.Name}");
            }

        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnementsRevue.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.Montant).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementsRevueListe(sortedList);
        }
        /// <summary>
        /// Enregistrement d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      
        private void btnEnregistrerCommandeRevue_Click(object sender, EventArgs e)
        {
            if (!txbNumNewCmdRevue.Text.Equals("") && !txbMontantCmdRevue.Text.Equals(""))
            {
                string idRevue = txtbNumCmdRevueRecherche.Text;
                string titre = txbTitreCmdRevue.Text;
                string id = txbNumNewCmdRevue.Text;
                double montant = double.Parse(txbMontantCmdRevue.Text);
                DateTime dateCommande = dateTimePickerCmdRevue.Value;
                DateTime dateFinAbonnement = dateTimePickerDateFinAbnmntCmdRevue.Value;
                Commande commande = new Commande(id, dateCommande, montant);
                Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue, titre);
                if (controller.CreerCommande(commande))
                {
                    controller.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
                    MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                    AfficheReceptionAbonnementsRevue();
                }
                else
                {
                    MessageBox.Show("numéro de commande déjà existant", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Information");
            }

        }
            /// <summary>
            /// Retourne vrai si la date de parution est entre les 2 autres dates
            /// </summary>
            /// <param name="dateCommande"></param>
            /// <param name="dateFinAbonnement"></param>
            /// <param name="dateParution"></param>
            /// <returns></returns>
            public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }
        /// <summary>
        /// Vérifie si aucun exemplaire n'est rattaché à un abonnement de revue
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool VerificationExemplaire(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplaires = controller.GetExemplairesRevue(abonnement.IdRevue);
            bool datedeparution = false;
            foreach (Exemplaire exemplaire in lesExemplaires.Where(exemplaires => ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, exemplaires.DateAchat)))
            {
                datedeparution = true;
            }
            return !datedeparution;
        }
        /// <summary>
        /// Suppression d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      
        private void btnSupprimerCommandeRevue_Click(object sender, EventArgs e)
        {
            if (dgvAbonnementsRevue.SelectedRows.Count > 0)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnementsRevue.Current;
                if (MessageBox.Show("Souhaitez-vous confirmer la suppression de l'abonnement " + abonnement.Id + " ?", "Confirmation de la suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (VerificationExemplaire(abonnement))
                    {
                        if (controller.SupprimerAbonnementRevue(abonnement))
                        {
                            AfficheReceptionAbonnementsRevue();
                        }
                        else
                        {
                            MessageBox.Show("Une erreur s'est produite.", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cet abonnement contient un ou plusieurs exemplaires, il ne peut donc pas être supprimé.", "Information");

                    }
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }






        #endregion

        private void dgvAbonnementsRevue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmMediatek_Load(object sender, EventArgs e)
        {

        }

       
    }
}

