using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Resources
{
    public class ToursStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ToursTitle => Strings.Tours;
        public string Id => Strings.Id;
        public string StartDate => Strings.Date;
        public string ArrivalDate => Strings.Date;
        public string Status => Strings.Status;

        public string DriverId => Strings.DriverID;

        public string DispatcherId => Strings.DispatcherId;

        public string DriverName => Strings.DriverName; // New: Driver's full name
        public string DispatcherName => Strings.DispatcherName;

        public string SearchLabel => Strings.Search;

        public string FinalPrice => Strings.FinalPrice;

        public string CloseButton => Strings.Close;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
