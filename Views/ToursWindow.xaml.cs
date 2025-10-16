using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Projekat_B_isTovar.Views
{
    public partial class ToursWindow : Window
    {
        public List<Tour> Tours { get; set; } = new();
        private List<Tour> allTours = new(); // Store all tours for filtering
        private UserSettings settings;
        private ToursStrings loc;

        public ToursWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new ToursStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadToursFromDatabase();
            ToursDataGrid.ItemsSource = Tours;

            // Set up search bar event
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void LoadToursFromDatabase()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT t.idTure, t.vrijemePolaska, t.vrijemeDolaska, t.status, 
                               CONCAT(kv.ime, ' ', kv.prezime) AS driverName, 
                               CONCAT(kd.ime, ' ', kd.prezime) AS dispatcherName, 
                               t.ukupnaCijenaTure
                        FROM tura t
                        LEFT JOIN vozac v ON t.idVozaca = v.idKorisnika
                        LEFT JOIN dispecer d ON t.idDispecera = d.idKorisnika
                        LEFT JOIN korisnik kv ON v.idKorisnika = kv.idKorisnika
                        LEFT JOIN korisnik kd ON d.idKorisnika = kd.idKorisnika";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tour = new Tour
                            {
                                IdTure = reader.GetInt32("idTure"),
                                VrijemePolaska = reader.GetDateTime("vrijemePolaska"),
                                VrijemeDolaska = reader.GetDateTime("vrijemeDolaska"),
                                Status = reader.GetString("status"),
                                DriverName = reader.IsDBNull(reader.GetOrdinal("driverName")) ? "N/A" : reader.GetString("driverName"),
                                DispatcherName = reader.IsDBNull(reader.GetOrdinal("dispatcherName")) ? "N/A" : reader.GetString("dispatcherName"),
                                UkupnaCijenaTure = reader.GetInt64("ukupnaCijenaTure")
                            };
                            Tours.Add(tour);
                            allTours.Add(tour); // Store in allTours for filtering
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ToursDataGrid.ItemsSource = allTours; // Show all tours if search is empty
            }
            else
            {
                var filteredTours = allTours.Where(t =>
                    t.IdTure.ToString().Contains(searchText) ||
                    t.VrijemePolaska.ToString("yyyy-MM-dd HH:mm:ss").ToLower().Contains(searchText) ||
                    t.VrijemeDolaska.ToString("yyyy-MM-dd HH:mm:ss").ToLower().Contains(searchText) ||
                    t.Status.ToLower().Contains(searchText) ||
                    (t.DriverName?.ToLower().Contains(searchText) ?? false) ||
                    (t.DispatcherName?.ToLower().Contains(searchText) ?? false) ||
                    t.UkupnaCijenaTure.ToString().Contains(searchText)).ToList();
                ToursDataGrid.ItemsSource = filteredTours;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}