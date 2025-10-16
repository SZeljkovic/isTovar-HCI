using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class DispatcherHomeStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string WelcomeDispatcher => Strings.WelcomeDispatcher;
        public string Drivers => Strings.Drivers;
        public string Trucks => Strings.Trucks;
        public string Trailers => Strings.Trailers;
        public string Issues => Strings.Issues;
        public string Tours => Strings.Tours;
        public string AddTour => Strings.AddTour;

        public string Theme => Strings.Theme;

        public string Language => Strings.Language;

        public string Blue => Strings.Blue;

        public string Light => Strings.Light;

        public string Dark => Strings.Dark;

        public string MyProfile => Strings.MyProfile;

        public string AddCompany => Strings.AddCompanyTitle;

        public string EditTour => Strings.EditTour;


        /// <summary>
        /// Poziva se da osvježi sve binding-e kada se promijeni jezik
        /// </summary>
        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
