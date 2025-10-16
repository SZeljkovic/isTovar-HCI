using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class DriverHome : Window
    {
        private UserSettings settings;
        private DriverHomeStrings loc;

        public DriverHome()
        {
            InitializeComponent();

            Debug.WriteLine("DriverHome initialized");

            // Učitaj korisnička podešavanja
            settings = UserSettings.Load();

            // Inicijalizuj lokalizovane stringove
            loc = new DriverHomeStrings();
            DataContext = loc;

            // Primijeni temu i jezik pri startu
            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language,loc);

            // Postavi izabrane stavke u ComboBox-ovima
            cbTheme.SelectedItem = cbTheme.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => (string)i.Tag == settings.Theme);

            cbLanguage.SelectedItem = cbLanguage.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => (string)i.Tag == settings.Language);
        }


        private void CbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTheme.SelectedItem is ComboBoxItem item)
            {
                string selectedTheme = item.Tag.ToString();
                settings.Theme = selectedTheme;
                settings.Save();
                ApplyManager.ApplyTheme(selectedTheme);
            }
        }

        private void CbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLanguage.SelectedItem is ComboBoxItem item)
            {
                string langCode = item.Tag.ToString();
                settings.Language = langCode;
                settings.Save();
                ApplyManager.ApplyLanguage(langCode,loc);
            }
        }

        // Event handleri za klikove na kartice
        private void CardSchedule_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyTours win = new MyTours(settings, loc);
            win.ShowDialog();
        }

        private void CardMyTruck_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyTruck win = new MyTruck(settings, loc);
            win.ShowDialog();
        }

        private void CardMyTrailer_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyTrailer win = new MyTrailer(settings, loc);
            win.ShowDialog();
        }

        private void CardReportIssue_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReportIssue win = new ReportIssue(settings, loc);
            win.ShowDialog();
        }

        private void CardMyProfile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyProfileWindow win = new MyProfileWindow(SessionManager.CurrentUserId);
            win.ShowDialog();
        }

        private void CardServices_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MaintenanceWindow win = new MaintenanceWindow(settings, loc);
            win.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentUserId = 0;

            LoginWindow login = new LoginWindow();

            this.Close();

            login.Show();
        }

    }
}
