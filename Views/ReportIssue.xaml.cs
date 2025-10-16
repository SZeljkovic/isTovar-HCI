using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class ReportIssue : Window
    {
        private UserSettings settings;
        private ReportIssueStrings loc;
        private int driverId = SessionManager.CurrentUserId; // ko prijavljuje problem

        public ReportIssue(UserSettings currentSettings, DriverHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;

            loc = new ReportIssueStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string content = IssueDescriptionBox.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show(loc.EmptyErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                InsertProblemIntoDatabase(driverId, content, DateTime.Now);
                MessageBox.Show(loc.SuccessMessage, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($": {ex.Message}", "DB Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertProblemIntoDatabase(int idKorisnika, string tekstProblema, DateTime datum)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO problem (idKorisnika, tekstProblema, datum, status) VALUES (@id, @tekst, @datum, 0)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idKorisnika);
                    cmd.Parameters.AddWithValue("@tekst", tekstProblema);
                    cmd.Parameters.AddWithValue("@datum", datum);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}