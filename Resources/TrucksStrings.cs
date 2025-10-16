using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class TrucksStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string TrucksTitle => Strings.TrucksList;
        public string Id => Strings.Id;
        public string CloseButton => Strings.Close;
        public string ColumnType => Strings.Type;
        public string ColumnBrand => Strings.Brand;
        public string ColumnHP => Strings.HorsePower;
        public string IdTrailer => Strings.IdTrailer;
        public string ColumnFuel => Strings.Fuel;
        public string ColumnProductionYear => Strings.ProductionYear;
        public string ColumnLicensePlate => Strings.LicensePlate;
        public string ColumnMileage => Strings.Mileage;

        public string SaveButton => Strings.Save;

        public string TabList => Strings.TrucksList;
        public string TabAdd => Strings.AddTruck;
        public string AddNewTruck => Strings.AddTruck;

        // Novi stringovi
        public string TabAssignTrailer => Strings.AssignTrailer;
        public string AssignTrailerTitle => Strings.AssignTrailer;
        public string AssignButton => Strings.AssignButton;

        public string EditButton => Strings.Edit;
        public string DeleteButton => Strings.Delete;

        public string LoadError => Strings.TruckLoadError;



        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
