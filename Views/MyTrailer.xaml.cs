using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class MyTrailer : Window
    {
        private UserSettings settings;
        private MyTrailerStrings loc;
        private int driverId = SessionManager.CurrentUserId; // trenutni vozač

        public MyTrailer(UserSettings currentSettings, DriverHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new MyTrailerStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTrailerInfo();
        }

        private void LoadTrailerInfo()
        {
            string query = @"
                SELECT p.vrsta, p.nosivost, p.godinaProizvodnje, p.registarskaOznaka
                FROM prikolica p
                JOIN kamion k ON k.idPrikolice = p.idPrikolice
                JOIN vozac v ON v.idKamiona = k.idKamiona
                WHERE v.idKorisnika = @driverId";

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@driverId", driverId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TypeValue.Text = reader["vrsta"].ToString();
                                CapacityValue.Text = reader["nosivost"].ToString() + " kg";
                                YearValue.Text = reader["godinaProizvodnje"].ToString();
                                LicenseValue.Text = reader["registarskaOznaka"].ToString();
                            }
                            else
                            {
                                MessageBox.Show($"{Strings.NoTrailerData}", "Info",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.Error}: { ex.Message}", "DB Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
