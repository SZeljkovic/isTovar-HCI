using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class DriversStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string DriversTitle => Strings.DriversList;
        public string Id => Strings.Id;
        public string CloseButton => Strings.Close;
        public string ColumnUsername => Strings.Username;
        public string ColumnName => Strings.FirstName;
        public string ColumnSurname => Strings.LastName;
        public string ColumnEmail => Strings.Email;
        public string ColumnPhone => Strings.Phone;
        public string ColumnLicenseNo => Strings.LicenseNumber;
        public string ColumnLicense => Strings.License;
        public string ColumnTruckId => Strings.TruckId;
        public string ColumnAvailable => Strings.Availability;
        public string SelectDriversTitle => Strings.SelectDriver; // Added
        public string SelectButton => Strings.SelectButton;

        public string DeleteButton => Strings.Delete;

        public string EditButton => Strings.Edit;



        public string SaveButton => Strings.Save;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}

