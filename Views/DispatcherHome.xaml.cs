using MaterialDesignThemes.Wpf;
using Projekat_B_isTovar.Resources;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Projekat_B_isTovar.UserThemes;

namespace Projekat_B_isTovar.Views
{
    public partial class DispatcherHome : Window
    {
        private UserSettings settings;
        private DispatcherHomeStrings loc;

        public DispatcherHome()
        {
            InitializeComponent();

            Debug.WriteLine("DispatcherHome initialized");

            settings = UserSettings.Load();

            loc = new DispatcherHomeStrings();
            DataContext = loc;

            // Primijeni temu i jezik
            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language,loc);

            // Postavi ComboBox selekcije
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
        private void CardDrivers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DriversWindow win = new DriversWindow(settings, loc);
            win.ShowDialog();
        }
        private void CardTrucks_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TrucksWindow win = new TrucksWindow(settings, loc);
            win.ShowDialog();
        }
        private void CardTrailers_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TrailersWindow win = new TrailersWindow(settings, loc);
            win.ShowDialog();
        }
        private void CardIssues_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            IssuesWindow win = new IssuesWindow(settings, loc);
            win.ShowDialog();
        }
        private void CardTours_Click(object sender, System.Windows.Input.MouseButtonEventArgs e){
            ToursWindow win = new ToursWindow(settings, loc);
            win.ShowDialog();
        }
        private void CardAddTour_Click(object sender, System.Windows.Input.MouseButtonEventArgs e){
            AddTourWindow win = new AddTourWindow(settings, loc);
            win.ShowDialog();
        }

        private void CardMyProfile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyProfileWindow win = new MyProfileWindow(SessionManager.CurrentUserId);
            win.ShowDialog();
        }

        private void CardAddCompany_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddCompanyWindow win = new AddCompanyWindow(settings, loc);
            win.ShowDialog();
        }

        private void CardEditTour_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditTourWindow win = new EditTourWindow(settings, loc);
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
