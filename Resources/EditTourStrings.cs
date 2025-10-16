using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class EditTourStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.EditTour;
        public string SelectTour => Strings.SelectTour;
        public string Source => Strings.Source;
        public string Destination => Strings.Destination;
        public string SelectCompany => Strings.SelectCompany;
        public string AddSource => Strings.AddSource;
        public string AddDestination => Strings.AddDestination;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
