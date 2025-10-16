using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Projekat_B_isTovar.Views
{
    public partial class TrailersWindow : Window
    {
        public List<Trailer> Trailers { get; set; } = new();
        private Trailer selectedTrailer;

        private UserSettings settings;
        private TrailersStrings loc;

        public TrailersWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new TrailersStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTrailersFromDatabase();
            TrailersDataGrid.ItemsSource = Trailers;
        }

        private void LoadTrailersFromDatabase()
        {
            Trailers.Clear();

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT idPrikolice, vrsta, nosivost, godinaProizvodnje, registarskaOznaka 
                                   FROM prikolica";

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

                TrailersDataGrid.Items.Refresh();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.TrailerLoadError}: {ex.Message}");
            }
        }

        private void SaveTrailer_Click(object sender, RoutedEventArgs e)
        {
            SaveTrailer();
        }

        private void SaveTrailer()
        {
            if (!ValidateTrailerInputs(txtKind.Text, txtCapacity.Text, txtYear.Text, txtPlate.Text, out int nosivost, out int godina))
                return;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO prikolica (vrsta, nosivost, godinaProizvodnje, registarskaOznaka)
                                   VALUES (@vrsta, @nosivost, @godina, @oznaka)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@vrsta", txtKind.Text);
                        cmd.Parameters.AddWithValue("@nosivost", nosivost);
                        cmd.Parameters.AddWithValue("@godina", godina);
                        cmd.Parameters.AddWithValue("@oznaka", txtPlate.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"{Strings.TrailerAdded}");
                LoadTrailersFromDatabase();

                txtKind.Clear();
                txtCapacity.Clear();
                txtYear.Clear();
                txtPlate.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.TrailerAddError}: {ex.Message}");
            }
        }

        private void EditTrailer_Click(object sender, RoutedEventArgs e)
        {
            selectedTrailer = TrailersDataGrid.SelectedItem as Trailer;
            if (selectedTrailer == null)
            {
                MessageBox.Show($"{Strings.PlsChooseTrailer}");
                return;
            }

            editKind.Text = selectedTrailer.Vrsta;
            editCapacity.Text = selectedTrailer.Nosivost.ToString();
            editYear.Text = selectedTrailer.GodinaProizvodnje.ToString();
            editPlate.Text = selectedTrailer.RegistarskaOznaka;

            EditPanel.Visibility = Visibility.Visible;
        }

        private void UpdateTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTrailer == null)
                return;

            if (!ValidateTrailerInputs(editKind.Text, editCapacity.Text, editYear.Text, editPlate.Text, out int nosivost, out int godina))
                return;

            try
            {
                UpdateTrailerInDatabase(selectedTrailer.IdPrikolice, editKind.Text, nosivost, godina, editPlate.Text);
                MessageBox.Show($"{Strings.TrailerEdited}");
                LoadTrailersFromDatabase();
                EditPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.TrailerEditError}: {ex.Message}");
            }
        }

        private void UpdateTrailerInDatabase(int idPrikolice, string vrsta, int nosivost, int godinaProizvodnje, string registarskaOznaka)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE prikolica 
                       SET vrsta=@vrsta, nosivost=@nosivost, godinaProizvodnje=@godina, registarskaOznaka=@oznaka
                       WHERE idPrikolice=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@vrsta", vrsta);
                    cmd.Parameters.AddWithValue("@nosivost", nosivost);
                    cmd.Parameters.AddWithValue("@godina", godinaProizvodnje);
                    cmd.Parameters.AddWithValue("@oznaka", registarskaOznaka);
                    cmd.Parameters.AddWithValue("@id", idPrikolice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteTrailer_Click(object sender, RoutedEventArgs e)
        {
            var trailer = TrailersDataGrid.SelectedItem as Trailer;
            if (trailer == null)
            {
                MessageBox.Show($"{Strings.ChooseDeleteTrailer}");
                return;
            }

            if (MessageBox.Show(Strings.TrailerDeleteSure,
                                Strings.Submit, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                DeleteTrailerFromDatabase(trailer.IdPrikolice);
                MessageBox.Show($"{Strings.TrailerDeleted}");
                LoadTrailersFromDatabase();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // foreign key constraint
                {
                    MessageBox.Show($"{Strings.CantDeleteTrailer}");
                }
                else
                {
                    MessageBox.Show($"{Strings.TrailerDeleteError}: {ex.Message}");
                }
            }
        }

        private void DeleteTrailerFromDatabase(int idPrikolice)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM prikolica WHERE idPrikolice=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idPrikolice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool ValidateTrailerInputs(string vrsta, string nosivostStr, string godinaStr, string oznaka, out int nosivost, out int godina)
        {
            nosivost = 0;
            godina = 0;

            if (string.IsNullOrWhiteSpace(vrsta) ||
                string.IsNullOrWhiteSpace(nosivostStr) ||
                string.IsNullOrWhiteSpace(godinaStr) ||
                string.IsNullOrWhiteSpace(oznaka))
            {
                MessageBox.Show($"{Strings.EnterAllFields}");
                return false;
            }

            if (!int.TryParse(nosivostStr, out nosivost))
            {
                MessageBox.Show($"{Strings.CapacityError}");
                return false;
            }

            if (!int.TryParse(godinaStr, out godina))
            {
                MessageBox.Show($"{Strings.YpError}");
                return false;
            }

            return true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
