using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class MyToursStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.MyTours;
        public string DepartureLabel => Strings.DepartureTime;
        public string ArrivalLabel => Strings.ArrivalTime;
        public string StatusLabel => Strings.Status;
        public string PriceLabel => Strings.Price;
        public string DetailsButton => Strings.Details;
        public string CloseButton => Strings.Close;

        public string UpcomingToursLabel => Strings.UpcomingTours; // New
        public string PastToursLabel => Strings.PastTours; // New

        public string NoDataMessage => Strings.NoTours;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
