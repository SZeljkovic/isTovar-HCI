using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class DriversWindow : Window
    {
        public ObservableCollection<Driver> Drivers { get; set; } = new();
        private Driver selectedDriver;

        private UserSettings settings;
        private DriversStrings loc;

        public DriversWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new DriversStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadDriversFromDatabase();
            DriversDataGrid.ItemsSource = Drivers;
        }

        private void LoadDriversFromDatabase()
        {
            Drivers.Clear();
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT k.idKorisnika, k.korisnickoIme, k.ime, k.prezime, k.email, k.brojTelefona,
                               v.brojDozvole, v.licenca, v.idKamiona, v.dostupnost
                        FROM korisnik k
                        JOIN vozac v ON k.idKorisnika = v.idKorisnika";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Drivers.Add(new Driver
                            {
                                IdKorisnika = reader.GetInt32("idKorisnika"),
                                KorisnickoIme = reader.GetString("korisnickoIme"),
                                Ime = reader.GetString("ime"),
                                Prezime = reader.GetString("prezime"),
                                Email = reader.GetString("email"),
                                BrojTelefona = reader.GetString("brojTelefona"),
                                BrojDozvole = reader.GetString("brojDozvole"),
                                Licenca = reader.GetString("licenca"),
                                IdKamiona = reader["idKamiona"] != DBNull.Value ? reader.GetInt32("idKamiona") : (int?)null,
                                Dostupnost = reader["dostupnost"] != DBNull.Value
                                    ? (Convert.ToSByte(reader["dostupnost"]) == 1
                                        ? Strings.Available
                                        : Strings.Unavailable)
                                    : Strings.Unknown
                            });
                        }
                    }
                }

                DriversDataGrid.Items.Refresh();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.DriverLoadError}: {ex.Message}");
            }
        }

        private void EditDriver_Click(object sender, RoutedEventArgs e)
        {
            selectedDriver = DriversDataGrid.SelectedItem as Driver;
            if (selectedDriver == null)
            {
                MessageBox.Show(Strings.PlsChooseDriver);
                return;
            }

            editUsername.Text = selectedDriver.KorisnickoIme;
            editFirstName.Text = selectedDriver.Ime;
            editLastName.Text = selectedDriver.Prezime;
            editEmail.Text = selectedDriver.Email;
            editPhone.Text = selectedDriver.BrojTelefona;
            editLicenseNo.Text = selectedDriver.BrojDozvole;
            editLicense.Text = selectedDriver.Licenca;

            EditPanel.Visibility = Visibility.Visible;
            ActionButtonsPanel.Visibility = Visibility.Collapsed;
        }

        private void UpdateDriverInDatabase()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE korisnik k
                               JOIN vozac v ON k.idKorisnika = v.idKorisnika
                               SET k.korisnickoIme=@username, k.ime=@ime, k.prezime=@prezime, 
                                   k.email=@email, k.brojTelefona=@telefon,
                                   v.brojDozvole=@dozvola, v.licenca=@licenca
                               WHERE k.idKorisnika=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", editUsername.Text);
                    cmd.Parameters.AddWithValue("@ime", editFirstName.Text);
                    cmd.Parameters.AddWithValue("@prezime", editLastName.Text);
                    cmd.Parameters.AddWithValue("@email", editEmail.Text);
                    cmd.Parameters.AddWithValue("@telefon", editPhone.Text);
                    cmd.Parameters.AddWithValue("@dozvola", editLicenseNo.Text);
                    cmd.Parameters.AddWithValue("@licenca", editLicense.Text);
                    cmd.Parameters.AddWithValue("@id", selectedDriver.IdKorisnika);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateDriver_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDriver == null) return;

            try
            {
                UpdateDriverInDatabase();
                MessageBox.Show(Strings.DriverEdited);
                LoadDriversFromDatabase();
                EditPanel.Visibility = Visibility.Collapsed;
                ActionButtonsPanel.Visibility = Visibility.Visible;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.DriverEditError}: {ex.Message}");
            }
        }

        private void DeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            var driver = DriversDataGrid.SelectedItem as Driver;
            if (driver == null)
            {
                MessageBox.Show(Strings.ChooseDeleteDriver);
                return;
            }

            if (MessageBox.Show(Strings.DriverDeleteSure,
                                Strings.Submit, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                DeleteDriverFromDatabase(driver.IdKorisnika);
                MessageBox.Show(Strings.DriverDeleted);
                LoadDriversFromDatabase();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // foreign key constraint
                    MessageBox.Show(Strings.CantDeleteDriver);
                else
                    MessageBox.Show($"{Strings.DriverDeleteError}: {ex.Message}");
            }
        }

        private void DeleteDriverFromDatabase(int driverId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                string deleteVozacSql = "DELETE FROM vozac WHERE idKorisnika = @id";
                using (var cmdVozac = new MySqlCommand(deleteVozacSql, conn))
                {
                    cmdVozac.Parameters.AddWithValue("@id", driverId);
                    cmdVozac.ExecuteNonQuery();
                }

                string deleteKorisnikSql = "DELETE FROM korisnik WHERE idKorisnika = @id";
                using (var cmdKorisnik = new MySqlCommand(deleteKorisnikSql, conn))
                {
                    cmdKorisnik.Parameters.AddWithValue("@id", driverId);
                    cmdKorisnik.ExecuteNonQuery();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditPanel.Visibility == Visibility.Visible)
            {
                EditPanel.Visibility = Visibility.Collapsed;
                ActionButtonsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.Close();
            }
        }
    }
}