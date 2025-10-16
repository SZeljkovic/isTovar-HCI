using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Resources
{
    public class TrailersStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string TrailersTitle => Strings.TrailersList;
        public string Id => Strings.Id;
        public string CloseButton => Strings.Close;
        public string ColumnKind => Strings.Kind;
        public string ColumnCapacity => Strings.Capacity;
        public string ColumnProductionYear => Strings.ProductionYear;
        public string ColumnLicensePlate => Strings.LicensePlate;

        public string TabList => Strings.TrailersList;
        public string TabAdd => Strings.TabAddTrailer;
        public string AddNewTrailer => Strings.TabAddTrailer;
        public string SaveButton => Strings.Save;

        public string EditButton => Strings.Edit;
        public string DeleteButton => Strings.Delete;
        public string EditTrailer => Strings.EditTrailer;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
