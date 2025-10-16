using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Projekat_B_isTovar.Views
{
    public partial class RegisterWindow : Window
    {
        private UserSettings settings;
        private RegisterStrings loc;

        public RegisterWindow()
        {
            InitializeComponent();

            // Učitavamo postavke (tema i jezik)
            settings = UserSettings.Load();

            // Lokalizacija
            loc = new RegisterStrings();
            DataContext = loc;

            // Primjena teme i jezika
            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            cmbRole.SelectionChanged += CmbRole_SelectionChanged;
        }

        private void CmbRole_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((cmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Tag.ToString() == "1")
                driverFields.Visibility = Visibility.Visible;
            else
                driverFields.Visibility = Visibility.Collapsed;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string ime = txtIme.Text.Trim();
            string prezime = txtPrezime.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();
            string telefon = txtTelefon.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || cmbRole.SelectedItem == null)
            {
                lblError.Text = loc.EmptyFieldsMessage;
                return;
            }

            int role = int.Parse((cmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem).Tag.ToString());

            try
            {
                RegisterUserInDatabase(username, ime, prezime, email, password, telefon, role);
                MessageBox.Show($"{Strings.RegistrationSuccessful}");
                new LoginWindow().Show();
                this.Close();
            }
            catch (MySqlException ex)
            {
                lblError.Text = loc.DbErrorMessage + ": " + ex.Message;
            }
        }

        private void RegisterUserInDatabase(string username, string ime, string prezime, string email,
                                           string password, string telefon, int role)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                //string hashedPassword = HashPasswordSHA256(password);


                //Ovdje ubaciti hashedPassword umjesto password u InsertKorisnik
                long idKorisnika = InsertKorisnik(conn, username, ime, prezime, email, password, telefon, role);

                if (role == 0)
                {
                    InsertDispecer(conn, idKorisnika);
                }
                else if (role == 1)
                {
                    InsertVozac(conn, idKorisnika, txtBrojDozvole.Text.Trim(), txtLicenca.Text.Trim());
                }
            }
        }

        private long InsertKorisnik(MySqlConnection conn, string username, string ime, string prezime,
                                   string email, string password, string telefon, int role)
        {
            string insertUser = @"INSERT INTO korisnik(korisnickoIme, ime, prezime, email, lozinka, uloga, brojTelefona)
                          VALUES(@user, @ime, @prezime, @mail, @pass, @role, @tel)";
            MySqlCommand cmd = new MySqlCommand(insertUser, conn);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.Parameters.AddWithValue("@ime", ime);
            cmd.Parameters.AddWithValue("@prezime", prezime);
            cmd.Parameters.AddWithValue("@mail", email);
            cmd.Parameters.AddWithValue("@pass", password);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.Parameters.AddWithValue("@tel", telefon);
            cmd.ExecuteNonQuery();

            return cmd.LastInsertedId;
        }

        private void InsertDispecer(MySqlConnection conn, long idKorisnika)
        {
            string insertDispatcher = "INSERT INTO dispecer(idKorisnika, status) VALUES(@id, 1)";
            MySqlCommand cmd = new MySqlCommand(insertDispatcher, conn);
            cmd.Parameters.AddWithValue("@id", idKorisnika);
            cmd.ExecuteNonQuery();
        }

        private void InsertVozac(MySqlConnection conn, long idKorisnika, string brojDozvole, string licenca)
        {
            string insertDriver = @"INSERT INTO vozac(idKorisnika, idKamiona, brojDozvole, licenca, dostupnost)
                            VALUES(@id, NULL, @dozvola, @licenca, 1)";
            MySqlCommand cmd = new MySqlCommand(insertDriver, conn);
            cmd.Parameters.AddWithValue("@id", idKorisnika);
            cmd.Parameters.AddWithValue("@dozvola", brojDozvole);
            cmd.Parameters.AddWithValue("@licenca", licenca);
            cmd.ExecuteNonQuery();
        }

        private string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Pretvori lozinku u bajtove
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                // Izračunaj SHA256 heš
                byte[] hash = sha256.ComputeHash(bytes);
                // Pretvori heš u heksadecimalni string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2")); // x2 formatira bajt kao dva heksadecimalna znaka
                }
                return builder.ToString();
            }
        }
    }
}
