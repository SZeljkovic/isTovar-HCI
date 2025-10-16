using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System.Windows.Media;

namespace Projekat_B_isTovar.Views
{
    public partial class MyTours : Window
    {
        private UserSettings settings;
        private MyToursStrings loc;
        private int driverId = SessionManager.CurrentUserId;

        public MyTours(UserSettings currentSettings, DriverHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new MyToursStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTours();
        }

        private void LoadTours()
        {
            string query = @"
                SELECT idTure, vrijemePolaska, vrijemeDolaska, status, ukupnaCijenaTure
                FROM tura
                WHERE idVozaca = @driverId
                ORDER BY vrijemePolaska DESC";

            var upcomingTours = new List<object>();
            var pastTours = new List<object>();
            var currentDateTime = DateTime.Now; // Current date and time for comparison

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
                            while (reader.Read())
                            {
                                string status = reader["status"].ToString() == "1"
                                    ? Strings.Completed
                                    : Strings.InProgress;

                                var tour = new
                                {
                                    TourId = reader["idTure"],
                                    Header = $"{Strings.Tour} #{reader["idTure"]}",
                                    Departure = $"{loc.DepartureLabel}: {Convert.ToDateTime(reader["vrijemePolaska"])}",
                                    Arrival = $"{loc.ArrivalLabel}: {Convert.ToDateTime(reader["vrijemeDolaska"])}",
                                    Status = $"{loc.StatusLabel}: {status}",
                                    Price = $"{loc.PriceLabel}: {reader["ukupnaCijenaTure"]} KM",
                                    DetailsLabel = loc.DetailsButton,
                                    IsUpcoming = Convert.ToDateTime(reader["vrijemePolaska"]) > currentDateTime
                                };

                                if (tour.IsUpcoming)
                                    upcomingTours.Add(tour);
                                else
                                    pastTours.Add(tour);
                            }
                        }
                    }
                }

                // Combine tours into a grouped collection
                var groupedTours = new List<object>
                {
                    new { GroupName = loc.UpcomingToursLabel, Tours = upcomingTours },
                    new { GroupName = loc.PastToursLabel, Tours = pastTours }
                };

                if (upcomingTours.Count == 0 && pastTours.Count == 0)
                {
                    MessageBox.Show(loc.NoDataMessage, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ToursList.ItemsSource = groupedTours;
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

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is object tourId)
            {
                var detailsWindow = new TourDetails(settings, loc, Convert.ToInt32(tourId));
                detailsWindow.ShowDialog();
            }
        }
    }
}