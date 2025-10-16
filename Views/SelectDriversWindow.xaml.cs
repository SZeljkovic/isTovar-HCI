using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class SelectDriversWindow : Window
    {
        public ObservableCollection<Driver> Drivers { get; set; } = new();
        public Driver SelectedDriver { get; private set; }
        private UserSettings settings;
        private DriversStrings loc;

        public SelectDriversWindow(UserSettings currentSettings, DriversStrings currentLoc)
        {
            try
            {
                InitializeComponent();

                settings = currentSettings;
                loc = currentLoc;
                DataContext = loc;

                ApplyManager.ApplyTheme(settings.Theme);
                ApplyManager.ApplyLanguage(settings.Language, loc);

                LoadDriversFromDatabase();
                DriversDataGrid.ItemsSource = Drivers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization Error -> SelectDriversWindow: {ex.Message}\nStackTrace: {ex.StackTrace}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadDriversFromDatabase()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        MessageBox.Show($"{Strings.ConnectionError}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string sql = @"
                        SELECT k.idKorisnika, k.ime, k.prezime, v.dostupnost
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
                                Ime = reader.GetString("ime"),
                                Prezime = reader.GetString("prezime"),
                                Dostupnost = reader["dostupnost"] != DBNull.Value
                                    ? (Convert.ToSByte(reader["dostupnost"]) == 1
                                        ? Strings.Available
                                        : Strings.Unavailable)
                                    : Strings.Unknown
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.DriverLoadError}: {ex.Message}\nError Code: {ex.Number}\nStackTrace: {ex.StackTrace}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DriversDataGrid.SelectedItem is Driver selectedDriver)
                {
                    SelectedDriver = selectedDriver;
                    Close();
                }
                else
                {
                    MessageBox.Show($"{Strings.ChooseSelectDriver}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.DriverSelectionError}: {ex.Message}\nStackTrace: {ex.StackTrace}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDriver = null;
            Close();
        }
    }
}