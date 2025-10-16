using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class TourDetailsStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.TourDetails;
        public string SourceCompanies => Strings.SourceCompanies;
        public string DestinationCompanies => Strings.DestinationCompanies;

        public string CompanyLabel => Strings.Company;
        public string AddressLabel => Strings.Address;
        public string TimeLabel => Strings.Time;

        public string BackButton => Strings.Back;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
