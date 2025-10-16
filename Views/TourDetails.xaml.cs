using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class TourDetails : Window
    {
        private UserSettings settings;
        private TourDetailsStrings loc;
        public static int SelectedTourId { get; set; }

        // ✅ Dodali smo tourId u konstruktor
        public TourDetails(UserSettings currentSettings, MyToursStrings parentLoc, int tourId)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new TourDetailsStrings();
            DataContext = loc;

            // Postavi ID ture
            SelectedTourId = tourId;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTourDetails();
        }

        private void LoadTourDetails()
        {
            LoadCompanies(SourceCompaniesContainer, "tura_firma_izvorista", "vrijemeUtovara");
            LoadCompanies(DestinationCompaniesContainer, "tura_firma_odredista", "vrijemeIstovara");
        }

        private void LoadCompanies(StackPanel container, string mappingTable, string timeColumn)
        {
            string query = $@"
                SELECT f.naziv, a.ulica, a.broj, a.grad, a.postanskiBroj, a.drzava, tf.{timeColumn}
                FROM {mappingTable} tf
                JOIN firma f ON tf.idFirme = f.idFirme
                JOIN adresa a ON f.idFirme = a.idFirme
                WHERE tf.idTure = @tourId";

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tourId", SelectedTourId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StackPanel card = new StackPanel
                                {
                                    Margin = new Thickness(0, 0, 0, 10)
                                };

                                card.Children.Add(new TextBlock
                                {
                                    Text = $"{loc.CompanyLabel}: {reader["naziv"]}",
                                    FontWeight = FontWeights.Bold
                                });

                                card.Children.Add(new TextBlock
                                {
                                    Text = $"{loc.AddressLabel}: {reader["ulica"]} {reader["broj"]}, {reader["grad"]} {reader["postanskiBroj"]}, {reader["drzava"]}"
                                });

                                card.Children.Add(new TextBlock
                                {
                                    Text = $"{loc.TimeLabel}: {reader[timeColumn]}"
                                });

                                container.Children.Add(card);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
