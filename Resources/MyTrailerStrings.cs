using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class MyTrailerStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.MyTrailer;
        public string TypeLabel => Strings.Type;
        public string CapacityLabel => Strings.Capacity;
        public string YearLabel => Strings.ProductionYear;
        public string LicenseLabel => Strings.LicensePlate;
        public string CloseButton => Strings.Close;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
