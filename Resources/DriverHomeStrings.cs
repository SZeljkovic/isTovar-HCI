using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class DriverHomeStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string WelcomeDriver => Strings.WelcomeDriver;
        public string Schedule => Strings.Schedule;
        public string MyTruck => Strings.MyTruck;
        public string MyTrailer => Strings.MyTrailer;
        public string ReportIssue => Strings.ReportIssue;

        public string MyProfile => Strings.MyProfile;


        public string Theme => Strings.Theme;

        public string Language => Strings.Language;

        public string Dark => Strings.Dark;

        public string Light => Strings.Light;

        public string Blue => Strings.Blue;

        public string Services => Strings.Services;


        /// <summary>
        /// Poziva se da osvježi sve binding-e kada se promijeni jezik
        /// </summary>
        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
