using MySql.Data.MySqlClient;
using System.Windows;
using Projekat_B_isTovar.Resources;

namespace Projekat_B_isTovar.Views
{
    public partial class MyProfileWindow : Window
    {
        private int userId;

        private MyProfileStrings loc;

        public MyProfileWindow(int userId)
        {
            InitializeComponent();
            loc = new MyProfileStrings();
            DataContext = loc;

            this.userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                string query = @"SELECT k.korisnickoIme, k.ime, k.prezime, k.email, k.lozinka, k.brojTelefona, d.status
                                 FROM korisnik k
                                 LEFT JOIN dispecer d ON k.idKorisnika = d.idKorisnika
                                 WHERE k.idKorisnika=@id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtUsername.Text = reader.GetString("korisnickoIme");
                    txtIme.Text = reader.GetString("ime");
                    txtPrezime.Text = reader.GetString("prezime");
                    txtEmail.Text = reader.GetString("email");
                    txtPassword.Password = reader.GetString("lozinka");
                    txtTelefon.Text = reader.GetString("brojTelefona");
                    txtStatus.Text = reader.IsDBNull(reader.GetOrdinal("status")) ? "" : reader.GetString("status");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.Error}: " + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateUserInDatabase(userId, txtUsername.Text.Trim(), txtIme.Text.Trim(),
                                   txtPrezime.Text.Trim(), txtEmail.Text.Trim(),
                                   txtPassword.Password.Trim(), txtTelefon.Text.Trim());
                MessageBox.Show($"{Strings.DataChanged}");
                this.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.Error}: " + ex.Message);
            }
        }

        private void UpdateUserInDatabase(int id, string username, string ime, string prezime,
                                        string email, string password, string telefon)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string updateQuery = @"UPDATE korisnik SET korisnickoIme=@user, ime=@ime, prezime=@prezime,
                           email=@email, lozinka=@pass, brojTelefona=@tel WHERE idKorisnika=@id";

            MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.Parameters.AddWithValue("@ime", ime);
            cmd.Parameters.AddWithValue("@prezime", prezime);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@pass", password);
            cmd.Parameters.AddWithValue("@tel", telefon);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
    }
}
