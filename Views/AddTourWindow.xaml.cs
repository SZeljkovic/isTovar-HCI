using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Data;

namespace Projekat_B_isTovar.Views
{
    public partial class AddTourWindow : Window
    {
        private UserSettings settings;
        private AddTourStrings loc;
        private int selectedDriverId;
        private string selectedDriverName;
        private int dispatcherId = SessionManager.CurrentUserId;

        public AddTourWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new AddTourStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);
            loc.Refresh();

            InitTimePickers();
        }

        private void InitTimePickers()
        {
            DepartureDatePicker.DisplayDateStart = DateTime.Today;
            ArrivalDatePicker.DisplayDateStart = DateTime.Today;  

            for (int h = 0; h < 24; h++) DepartureHourBox.Items.Add(h);
            for (int m = 0; m < 60; m += 1)
            {
                DepartureMinuteBox.Items.Add(m);
                ArrivalMinuteBox.Items.Add(m);
            }
            for (int h = 0; h < 24; h++) ArrivalHourBox.Items.Add(h);

            DepartureHourBox.SelectedIndex = 0;
            DepartureMinuteBox.SelectedIndex = 0;
            ArrivalHourBox.SelectedIndex = 0;
            ArrivalMinuteBox.SelectedIndex = 0;
        }

        private void SelectDriverButton_Click(object sender, RoutedEventArgs e)
        {
            var selectDriversWindow = new SelectDriversWindow(settings, new DriversStrings());
            selectDriversWindow.ShowDialog();

            if (selectDriversWindow.SelectedDriver != null)
            {
                selectedDriverId = selectDriversWindow.SelectedDriver.IdKorisnika;
                selectedDriverName = $"{selectDriversWindow.SelectedDriver.Ime} {selectDriversWindow.SelectedDriver.Prezime}";
                DriverInfoLabel.Text = $"{Strings.ChoosenDriver} {selectedDriverName} (ID: {selectedDriverId})";
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (!DepartureDatePicker.SelectedDate.HasValue || DepartureHourBox.SelectedItem == null || DepartureMinuteBox.SelectedItem == null ||
                    !ArrivalDatePicker.SelectedDate.HasValue || ArrivalHourBox.SelectedItem == null || ArrivalMinuteBox.SelectedItem == null)
                {
                    MessageBox.Show($"{Strings.PleaseValidTime}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedDriverId <= 0)
                {
                    MessageBox.Show($"{Strings.PleaseValidDriver}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!long.TryParse(PriceTextBox.Text, out long price))
                {
                    MessageBox.Show($"{Strings.PleaseValidPrice}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime departure = DepartureDatePicker.SelectedDate.Value
                    .AddHours((int)DepartureHourBox.SelectedItem)
                    .AddMinutes((int)DepartureMinuteBox.SelectedItem);

                DateTime arrival = ArrivalDatePicker.SelectedDate.Value
                    .AddHours((int)ArrivalHourBox.SelectedItem)
                    .AddMinutes((int)ArrivalMinuteBox.SelectedItem);

                if (departure >= arrival)
                {
                    MessageBox.Show($"{Strings.DepartureArrivalWarning}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!CheckDriverAvailability(selectedDriverId, departure, arrival))
                {
                    MessageBox.Show($"{Strings.Driver} {selectedDriverName} {Strings.IsBusy}", $"{Strings.Warning}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string status = StatusTextBox.Text;

                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string insert = @"INSERT INTO tura (vrijemePolaska, vrijemeDolaska, status, idVozaca, idDispecera, ukupnaCijenaTure)
                                      VALUES (@polazak, @dolazak, @status, @vozac, @dispecer, @cijena)";
                    using (var cmd = new MySqlCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@polazak", departure).MySqlDbType = MySqlDbType.DateTime;
                        cmd.Parameters.AddWithValue("@dolazak", arrival).MySqlDbType = MySqlDbType.DateTime;
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@vozac", selectedDriverId).MySqlDbType = MySqlDbType.Int32;
                        cmd.Parameters.AddWithValue("@dispecer", dispatcherId).MySqlDbType = MySqlDbType.Int32;
                        cmd.Parameters.AddWithValue("@cijena", price).MySqlDbType = MySqlDbType.Int64;

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show($"{Strings.TourAdded}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show($"{Strings.TourGetError}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}\nStackTrace: {ex.StackTrace}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckDriverAvailability(int driverId, DateTime start, DateTime end)
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        MessageBox.Show($"{Strings.DbError}", $"{Strings.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    using (var cmd = new MySqlCommand("ProvjeriDostupnostVozaca", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_idVozaca", driverId).MySqlDbType = MySqlDbType.Int32;
                        cmd.Parameters.AddWithValue("@p_vrijemePolaska", start).MySqlDbType = MySqlDbType.DateTime;
                        cmd.Parameters.AddWithValue("@p_vrijemeDolaska", end).MySqlDbType = MySqlDbType.DateTime;

                        var output = new MySqlParameter("@p_dostupan", MySqlDbType.Byte)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(output);

                        cmd.ExecuteNonQuery();
                        return Convert.ToInt32(output.Value) == 1;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.DriverAvailabilityError}: {ex.Message}\nError Code: {ex.Number}\nStackTrace: {ex.StackTrace}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ArrivalHourBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}