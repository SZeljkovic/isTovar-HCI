using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Projekat_B_isTovar.Resources;
using Projekat_B_isTovar.UserThemes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_B_isTovar.Views
{
    public partial class EditTourWindow : Window
    {
        private UserSettings settings;
        private EditTourStrings loc;

        public EditTourWindow(UserSettings settings, DispatcherHomeStrings parentLoc)
        {
            InitializeComponent();

            this.settings = settings;
            loc = new EditTourStrings();
            DataContext = loc;

            ApplyManager.ApplyTheme(settings.Theme);
            ApplyManager.ApplyLanguage(settings.Language, loc);

            LoadTure();
            LoadFirme();
        }

        private void LoadTure()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT idTure, vrijemePolaska, vrijemeDolaska FROM tura", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var liste = new List<object>();
                        while (reader.Read())
                        {
                            liste.Add(new
                            {
                                Id = reader.GetInt32("idTure"),
                                Display = $"#{reader.GetInt32("idTure")} | {reader.GetDateTime("vrijemePolaska")} - {reader.GetDateTime("vrijemeDolaska")}"
                            });
                        }
                        cbTure.ItemsSource = liste;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = Strings.TourLoadError + ex.Message;
            }
        }

        private void LoadFirme()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT idFirme, naziv FROM firma", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var firme = new List<object>();
                        while (reader.Read())
                        {
                            firme.Add(new
                            {
                                Id = reader.GetInt32("idFirme"),
                                Naziv = reader.GetString("naziv")
                            });
                        }
                        cbFirmeIzvoriste.ItemsSource = firme;
                        cbFirmeOdrediste.ItemsSource = firme;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = Strings.CompanyLoadError + ex.Message;
            }
        }

        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            if (cbTure.SelectedValue == null || cbFirmeIzvoriste.SelectedValue == null || dpUtovar.SelectedDate == null || tpUtovar.SelectedTime == null)
            {
                lblMessage.Text = Strings.SourceCompanies;
                return;
            }

            DateTime vrijemeUtovara = dpUtovar.SelectedDate.Value.Date + tpUtovar.SelectedTime.Value.TimeOfDay;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO tura_firma_izvorista (idTure, idFirme, vrijemeUtovara) VALUES (@idTure, @idFirme, @vrijeme)", conn);
                    cmd.Parameters.AddWithValue("@idTure", cbTure.SelectedValue);
                    cmd.Parameters.AddWithValue("@idFirme", cbFirmeIzvoriste.SelectedValue);
                    cmd.Parameters.AddWithValue("@vrijeme", vrijemeUtovara);
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = Strings.SourceAdded;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"{Strings.Error}: " + ex.Message;
            }
        }

        private void AddDestination_Click(object sender, RoutedEventArgs e)
        {
            if (cbTure.SelectedValue == null || cbFirmeOdrediste.SelectedValue == null || dpIstovar.SelectedDate == null || tpIstovar.SelectedTime == null)
            {
                lblMessage.Text = Strings.DestinationFields;
                return;
            }

            DateTime vrijemeIstovara = dpIstovar.SelectedDate.Value.Date + tpIstovar.SelectedTime.Value.TimeOfDay;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO tura_firma_odredista (idTure, idFirme, vrijemeIstovara) VALUES (@idTure, @idFirme, @vrijeme)", conn);
                    cmd.Parameters.AddWithValue("@idTure", cbTure.SelectedValue);
                    cmd.Parameters.AddWithValue("@idFirme", cbFirmeOdrediste.SelectedValue);
                    cmd.Parameters.AddWithValue("@vrijeme", vrijemeIstovara);
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = Strings.DestinationAdded;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = Strings.Error + ": " + ex.Message;
            }
        }

    }
}
