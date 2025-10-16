using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class MyTruckStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.MyTruck;
        public string TypeLabel => Strings.Type;
        public string BrandLabel => Strings.Brand;

        public string HorsepowerLabel => Strings.HorsePower;

        public string FuelLabel => Strings.Fuel;

        public string YearLabel => Strings.ProductionYear;
        public string LicenseLabel => Strings.LicensePlate;

        public string MileageLabel => Strings.Mileage;

        public string CloseButton => Strings.Close;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
