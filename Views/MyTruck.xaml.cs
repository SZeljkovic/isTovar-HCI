using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class MyTruck : Window
    {
        private UserSettings settings;
        private MyTruckStrings loc;
        private int driverId = SessionManager.CurrentUserId; // trenutni vozač

        public MyTruck(UserSettings currentSettings, DriverHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new MyTruckStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTruckInfo();
        }

        private void LoadTruckInfo()
        {
            string query = @"
                SELECT k.tip, k.marka, k.konjskeSnage, k.vrstaGoriva, 
                       k.godinaProizvodnje, k.registarskaOznaka, k.kilometraza
                FROM kamion k
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
                                TypeValue.Text = reader["tip"].ToString();
                                BrandValue.Text = reader["marka"].ToString();
                                HpValue.Text = reader["konjskeSnage"].ToString();
                                FuelValue.Text = reader["vrstaGoriva"].ToString();
                                YearValue.Text = reader["godinaProizvodnje"].ToString();
                                LicenseValue.Text = reader["registarskaOznaka"].ToString();
                                MileageValue.Text = reader["kilometraza"].ToString() + " km" ;
                            }
                            else
                            {
                                MessageBox.Show($"{Strings.NoTruckData}", "Info",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}", "DB Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
