using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Windows;

namespace Projekat_B_isTovar.Views
{
    public partial class AddCompanyWindow : Window
    {
        private UserSettings settings;
        private AddCompanyStrings loc;

        public AddCompanyWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new AddCompanyStrings();
            DataContext = loc;

            // primjena teme i jezika
            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string naziv = txtNaziv.Text.Trim();
            string email = txtEmail.Text.Trim();
            string ziroRacun = txtZiroRacun.Text.Trim();
            string ulica = txtUlica.Text.Trim();
            string broj = txtBroj.Text.Trim();
            string grad = txtGrad.Text.Trim();
            string postanskiBroj = txtPostanskiBroj.Text.Trim();
            string drzava = txtDrzava.Text.Trim();
            string telefon = txtTelefon.Text.Trim();
            bool fax = chkFax.IsChecked ?? false;

            if (string.IsNullOrEmpty(naziv) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(ziroRacun))
            {
                lblMessage.Text = loc.EmptyFieldsMessage;
                return;
            }

            try
            {
                InsertCompanyIntoDatabase(naziv, email, ziroRacun, ulica, broj, grad, postanskiBroj, drzava, telefon, fax);
                lblMessage.Foreground = System.Windows.Media.Brushes.LightGreen;
                lblMessage.Text = loc.SuccessMessage;
                ClearForm();
            }
            catch (MySqlException ex)
            {
                lblMessage.Foreground = System.Windows.Media.Brushes.Tomato;
                lblMessage.Text = loc.DbErrorMessage + ": " + ex.Message;
            }
        }

        private void InsertCompanyIntoDatabase(string naziv, string email, string ziroRacun, string ulica,
                                             string broj, string grad, string postanskiBroj, string drzava,
                                             string telefon, bool fax)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                // 1. firma
                long idFirme = InsertFirma(conn, naziv, email, ziroRacun);

                // 2. adresa
                InsertAdresa(conn, ulica, broj, grad, postanskiBroj, drzava, idFirme);

                // 3. telefon
                InsertTelefon(conn, telefon, fax, idFirme);
            }
        }

        private long InsertFirma(MySqlConnection conn, string naziv, string email, string ziroRacun)
        {
            string insertFirma = @"INSERT INTO firma(naziv, mejl, ziroRacun) VALUES(@naziv, @mejl, @racun)";
            MySqlCommand cmd = new MySqlCommand(insertFirma, conn);
            cmd.Parameters.AddWithValue("@naziv", naziv);
            cmd.Parameters.AddWithValue("@mejl", email);
            cmd.Parameters.AddWithValue("@racun", ziroRacun);
            cmd.ExecuteNonQuery();

            return cmd.LastInsertedId;
        }

        private void InsertAdresa(MySqlConnection conn, string ulica, string broj, string grad,
                                 string postanskiBroj, string drzava, long idFirme)
        {
            string insertAdresa = @"INSERT INTO adresa(ulica, broj, grad, postanskiBroj, drzava, idFirme) 
                           VALUES(@ulica, @broj, @grad, @postanski, @drzava, @id)";
            MySqlCommand cmdAdresa = new MySqlCommand(insertAdresa, conn);
            cmdAdresa.Parameters.AddWithValue("@ulica", ulica);
            cmdAdresa.Parameters.AddWithValue("@broj", broj);
            cmdAdresa.Parameters.AddWithValue("@grad", grad);
            cmdAdresa.Parameters.AddWithValue("@postanski", postanskiBroj);
            cmdAdresa.Parameters.AddWithValue("@drzava", drzava);
            cmdAdresa.Parameters.AddWithValue("@id", idFirme);
            cmdAdresa.ExecuteNonQuery();
        }

        private void InsertTelefon(MySqlConnection conn, string telefon, bool fax, long idFirme)
        {
            string insertTelefon = @"INSERT INTO telefon(brojTelefona, fax, idFirme) VALUES(@broj, @fax, @id)";
            MySqlCommand cmdTelefon = new MySqlCommand(insertTelefon, conn);
            cmdTelefon.Parameters.AddWithValue("@broj", telefon);
            cmdTelefon.Parameters.AddWithValue("@fax", fax);
            cmdTelefon.Parameters.AddWithValue("@id", idFirme);
            cmdTelefon.ExecuteNonQuery();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            txtNaziv.Clear();
            txtEmail.Clear();
            txtZiroRacun.Clear();
            txtUlica.Clear();
            txtBroj.Clear();
            txtGrad.Clear();
            txtPostanskiBroj.Clear();
            txtDrzava.Clear();
            txtTelefon.Clear();
            chkFax.IsChecked = false;
            lblMessage.Text = "";
        }
    }
}
