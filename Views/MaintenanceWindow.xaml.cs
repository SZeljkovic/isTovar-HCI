using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using MySql.Data.MySqlClient;

namespace Projekat_B_isTovar.Views
{
    public partial class MaintenanceWindow : Window
    {
        private UserSettings settings;
        private MaintenanceStrings loc;

        public MaintenanceWindow(UserSettings settings, DriverHomeStrings parentLoc)
        {
            InitializeComponent();

            this.settings = settings;
            loc = new MaintenanceStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    // Dohvati ID kamiona i prikolice za trenutnog vozača
                    int idKamiona = -1;
                    int idPrikolice = -1;

                    var cmd = new MySqlCommand("SELECT idKamiona FROM vozac WHERE idKorisnika=@id", conn);
                    cmd.Parameters.AddWithValue("@id", SessionManager.CurrentUserId);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        idKamiona = Convert.ToInt32(result);

                    if (idKamiona > 0)
                    {
                        cmd = new MySqlCommand("SELECT idPrikolice FROM kamion WHERE idKamiona=@idK", conn);
                        cmd.Parameters.AddWithValue("@idK", idKamiona);
                        result = cmd.ExecuteScalar();
                        if (result != null)
                            idPrikolice = Convert.ToInt32(result);
                    }

                    // Učitaj odrzavanje za kamion
                    if (idKamiona > 0)
                    {
                        var kamionList = new List<object>();
                        cmd = new MySqlCommand("SELECT datum, opis FROM odrzavanje WHERE KAMION_idKamiona=@idK", conn);
                        cmd.Parameters.AddWithValue("@idK", idKamiona);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                kamionList.Add(new
                                {
                                    Datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy"),
                                    Opis = reader.GetString("opis")
                                });
                            }
                        }
                        dgKamion.ItemsSource = kamionList;
                    }

                    // Učitaj odrzavanje za prikolicu
                    if (idPrikolice > 0)
                    {
                        var prikolicaList = new List<object>();
                        cmd = new MySqlCommand("SELECT datum, opis FROM odrzavanje WHERE PRIKOLICA_idPrikolice=@idP", conn);
                        cmd.Parameters.AddWithValue("@idP", idPrikolice);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                prikolicaList.Add(new
                                {
                                    Datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy"),
                                    Opis = reader.GetString("opis")
                                });
                            }
                        }
                        dgPrikolica.ItemsSource = prikolicaList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Greška prilikom učitavanja održavanja: " + ex.Message);
                MessageBox.Show($"{Strings.Error}: " + ex.Message);
            }
        }
    }
}
