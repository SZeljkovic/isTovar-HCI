using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class TrucksWindow : Window
    {
        public ObservableCollection<Truck> Trucks { get; set; } = new();
        public ObservableCollection<Trailer> Trailers { get; set; } = new();
        private Truck selectedTruck;

        private UserSettings settings;
        private TrucksStrings loc;

        public TrucksWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new TrucksStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTrucksFromDatabase();
            LoadTrailersFromDatabase();

            TrucksDataGrid.ItemsSource = Trucks;
            cmbTruck.ItemsSource = Trucks;
            cmbTrailer.ItemsSource = Trailers;
        }

        // ------------------ KAMIONI ------------------

        private void LoadTrucksFromDatabase()
        {
            Trucks.Clear();
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT idKamiona, tip, marka, konjskeSnage, idPrikolice, vrstaGoriva,
                                          godinaProizvodnje, registarskaOznaka, kilometraza
                                   FROM kamion";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Trucks.Add(new Truck
                            {
                                IdKamiona = reader.GetInt32("idKamiona"),
                                Tip = reader.GetString("tip"),
                                Brend = reader.GetString("marka"),
                                KonjskeSnage = reader.GetInt32("konjskeSnage"),
                                IdPrikolice = reader["idPrikolice"] != DBNull.Value
                                    ? reader.GetInt32("idPrikolice")
                                    : (int?)null,
                                Gorivo = reader.GetString("vrstaGoriva"),
                                GodinaProizvodnje = reader.GetInt32("godinaProizvodnje"),
                                RegistarskaOznaka = reader.GetString("registarskaOznaka"),
                                Kilometraza = reader.GetInt32("kilometraza")
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.TruckLoadError}: {ex.Message}");
            }
        }

        private void InsertTruck()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                string sql = @"INSERT INTO kamion (tip, marka, konjskeSnage, vrstaGoriva, godinaProizvodnje, registarskaOznaka, kilometraza)
                               VALUES (@tip, @marka, @hp, @gorivo, @godina, @oznaka, @km)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tip", txtType.Text);
                    cmd.Parameters.AddWithValue("@marka", txtBrand.Text);
                    cmd.Parameters.AddWithValue("@hp", int.Parse(txtHP.Text));
                    cmd.Parameters.AddWithValue("@gorivo", txtFuel.Text);
                    cmd.Parameters.AddWithValue("@godina", int.Parse(txtYear.Text));
                    cmd.Parameters.AddWithValue("@oznaka", txtPlate.Text);
                    cmd.Parameters.AddWithValue("@km", int.Parse(txtMileage.Text));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTruck(int id, string tip, string marka, int hp, string gorivo, int godina, string oznaka, int km)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE kamion 
                               SET tip=@tip, marka=@marka, konjskeSnage=@hp, 
                                   vrstaGoriva=@gorivo, godinaProizvodnje=@godina, 
                                   registarskaOznaka=@oznaka, kilometraza=@km
                               WHERE idKamiona=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tip", tip);
                    cmd.Parameters.AddWithValue("@marka", marka);
                    cmd.Parameters.AddWithValue("@hp", hp);
                    cmd.Parameters.AddWithValue("@gorivo", gorivo);
                    cmd.Parameters.AddWithValue("@godina", godina);
                    cmd.Parameters.AddWithValue("@oznaka", oznaka);
                    cmd.Parameters.AddWithValue("@km", km);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteTruck(int truckId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM kamion WHERE idKamiona=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", truckId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveTruck_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateTruckInputs(txtType.Text, txtBrand.Text, txtHP.Text, txtFuel.Text, txtYear.Text, txtPlate.Text, txtMileage.Text, out int hp, out int godina, out int km))
                return;

            try
            {
                InsertTruck();

                MessageBox.Show(Strings.TruckAdded);
                LoadTrucksFromDatabase();

                // Refresh combo
                cmbTruck.ItemsSource = Trucks;

                txtType.Clear();
                txtBrand.Clear();
                txtHP.Clear();
                txtFuel.Clear();
                txtYear.Clear();
                txtPlate.Clear();
                txtMileage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.TruckLoadError}: {ex.Message}");
            }
        }

        private void EditTruck_Click(object sender, RoutedEventArgs e)
        {
            selectedTruck = TrucksDataGrid.SelectedItem as Truck;
            if (selectedTruck == null)
            {
                MessageBox.Show(Strings.EditChooseTruck);
                return;
            }

            editType.Text = selectedTruck.Tip;
            editBrand.Text = selectedTruck.Brend;
            editHP.Text = selectedTruck.KonjskeSnage.ToString();
            editFuel.Text = selectedTruck.Gorivo;
            editYear.Text = selectedTruck.GodinaProizvodnje.ToString();
            editPlate.Text = selectedTruck.RegistarskaOznaka;
            editMileage.Text = selectedTruck.Kilometraza.ToString();

            EditPanel.Visibility = Visibility.Visible;
        }

        private void UpdateTruck_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTruck == null)
                return;

            if (!ValidateTruckInputs(editType.Text, editBrand.Text, editHP.Text, editFuel.Text, editYear.Text, editPlate.Text, editMileage.Text, out int hp, out int godina, out int km))
                return;

            try
            {
                UpdateTruck(selectedTruck.IdKamiona, editType.Text, editBrand.Text, hp, editFuel.Text, godina, editPlate.Text, km);

                MessageBox.Show($"{Strings.TruckEdited}");
                LoadTrucksFromDatabase();
                EditPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.TruckEditError}: {ex.Message}");
            }
        }

        private bool ValidateTruckInputs(string tip, string marka, string hpStr, string gorivo, string godinaStr, string oznaka, string kmStr, out int hp, out int godina, out int km)
        {
            hp = 0;
            godina = 0;
            km = 0;

            if (string.IsNullOrWhiteSpace(tip) ||
                string.IsNullOrWhiteSpace(marka) ||
                string.IsNullOrWhiteSpace(hpStr) ||
                string.IsNullOrWhiteSpace(gorivo) ||
                string.IsNullOrWhiteSpace(godinaStr) ||
                string.IsNullOrWhiteSpace(oznaka) ||
                string.IsNullOrWhiteSpace(kmStr))
            {
                MessageBox.Show($"{Strings.EnterAllFields}");
                return false;
            }

            if (!int.TryParse(hpStr, out hp))
            {
                MessageBox.Show($"{Strings.HpError}");
                return false;
            }

            if (!int.TryParse(godinaStr, out godina))
            {
                MessageBox.Show($"{Strings.YpError}");
                return false;
            }

            if (!int.TryParse(kmStr, out km))
            {
                MessageBox.Show($"{Strings.MileageError}");
                return false;
            }

            return true;
        }

        // ------------------ PRIKOLICE ------------------

        private void LoadTrailersFromDatabase()
        {
            Trailers.Clear();
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT idPrikolice, vrsta, nosivost, godinaProizvodnje, registarskaOznaka
                                   FROM prikolica
                                   WHERE idPrikolice NOT IN (SELECT idPrikolice FROM kamion WHERE idPrikolice IS NOT NULL)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Trailers.Add(new Trailer
                            {
                                IdPrikolice = reader.GetInt32("idPrikolice"),
                                Vrsta = reader.GetString("vrsta"),
                                Nosivost = reader.GetInt32("nosivost"),
                                GodinaProizvodnje = reader.GetInt32("godinaProizvodnje"),
                                RegistarskaOznaka = reader.GetString("registarskaOznaka")
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.TrailerLoadError}: {ex.Message}");
            }
        }

        private void AssignTrailerToTruck(int truckId, int trailerId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE kamion SET idPrikolice=@prikolica WHERE idKamiona=@kamion";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@prikolica", trailerId);
                    cmd.Parameters.AddWithValue("@kamion", truckId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AssignTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTruck.SelectedValue == null || cmbTrailer.SelectedValue == null)
            {
                MessageBox.Show($"{Strings.ChooseTruckTrailer}");
                return;
            }

            try
            {
                int truckId = (int)cmbTruck.SelectedValue;
                int trailerId = (int)cmbTrailer.SelectedValue;

                AssignTrailerToTruck(truckId, trailerId);

                MessageBox.Show($"{Strings.TrailerAssigned}");
                LoadTrucksFromDatabase();
                LoadTrailersFromDatabase();

                cmbTruck.ItemsSource = Trucks;
                cmbTrailer.ItemsSource = Trailers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.AssignTrailerError}: {ex.Message}");
            }
        }

        private void DeleteTruck_Click(object sender, RoutedEventArgs e)
        {
            if (TrucksDataGrid.SelectedItem is Truck selectedTruck)
            {
                var result = MessageBox.Show(Strings.TruckConfirmDelete,
                                             Strings.DeleteConfirmation,
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DeleteTruck(selectedTruck.IdKamiona);

                        MessageBox.Show($"{Strings.TruckDeleted}");
                        LoadTrucksFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{Strings.TruckDeleteError}: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show($"{Strings.ChooseTruckDelete}");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}