using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class MaintenanceStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.MaintenanceTitle;
        public string Truck => Strings.Truck;
        public string Trailer => Strings.Trailer;
        public string TruckInfo => Strings.TruckInfo;
        public string TrailerInfo => Strings.TrailerInfo;
        public string Date => Strings.Date;
        public string Description => Strings.Description;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
