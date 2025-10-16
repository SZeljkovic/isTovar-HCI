using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Models;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_B_isTovar.Views
{
    public partial class IssuesWindow : Window
    {
        public List<Issue> Issues { get; set; } = new();
        private Issue selectedIssue;

        private UserSettings settings;
        private IssuesStrings loc;

        public IssuesWindow(UserSettings currentSettings, DispatcherHomeStrings currentLoc)
        {
            InitializeComponent();

            settings = currentSettings;
            loc = new IssuesStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadIssuesFromDatabase();
            IssuesDataGrid.ItemsSource = Issues;
        }

        private void LoadIssuesFromDatabase()
        {
            Issues.Clear();
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT idProblem, idKorisnika, tekstProblema, datum, status FROM problem";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Issues.Add(new Issue
                            {
                                IdProblema = reader.GetInt32("idProblem"),
                                IdKorisnika = reader.GetInt32("idKorisnika"),
                                TekstProblema = reader.GetString("tekstProblema"),
                                Datum = DateOnly.FromDateTime(reader.GetDateTime("datum")),
                                Status = reader.GetString("status")
                            });
                        }
                    }
                }

                IssuesDataGrid.Items.Refresh();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($": {ex.Message}");
            }
        }

        private void EditIssue_Click(object sender, RoutedEventArgs e)
        {
            selectedIssue = IssuesDataGrid.SelectedItem as Issue;
            if (selectedIssue == null)
            {
                MessageBox.Show($"{Strings.ChooseIssueEdit}");
                return;
            }

            // Popuni polja sa postojećim podacima
            editUserId.Text = selectedIssue.IdKorisnika.ToString();
            editContent.Text = selectedIssue.TekstProblema;
            editDate.SelectedDate = new DateTime(selectedIssue.Datum.Year, selectedIssue.Datum.Month, selectedIssue.Datum.Day);

            // Postavi status direktno na tekst
            editStatus.Text = selectedIssue.Status;

            // Prikaži edit panel i sakrij dugmad
            EditPanel.Visibility = Visibility.Visible;
            ActionButtonsPanel.Visibility = Visibility.Collapsed;
        }

        private void UpdateIssueInDatabase()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE problem 
                       SET idKorisnika=@userId, tekstProblema=@content, 
                           datum=@date, status=@status 
                       WHERE idProblem=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", int.Parse(editUserId.Text));
                    cmd.Parameters.AddWithValue("@content", editContent.Text);
                    cmd.Parameters.AddWithValue("@date", editDate.SelectedDate);
                    cmd.Parameters.AddWithValue("@status", editStatus.Text); // Direktno tekst
                    cmd.Parameters.AddWithValue("@id", selectedIssue.IdProblema);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateIssue_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIssue == null) return;

            try
            {
                // Validacija
                if (string.IsNullOrWhiteSpace(editContent.Text))
                {
                    MessageBox.Show($"{Strings.ProblemText}");
                    return;
                }

                if (!int.TryParse(editUserId.Text, out int userId) || userId <= 0)
                {
                    MessageBox.Show($"{Strings.EditUserId}");
                    return;
                }

                UpdateIssueInDatabase();
                MessageBox.Show($"{Strings.ProblemEdited}");
                LoadIssuesFromDatabase();

                // Vrati se na normalan prikaz
                EditPanel.Visibility = Visibility.Collapsed;
                ActionButtonsPanel.Visibility = Visibility.Visible;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{Strings.ErrorEditProblem}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}");
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            // Zatvori edit panel bez čuvanja promjena
            EditPanel.Visibility = Visibility.Collapsed;
            ActionButtonsPanel.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}