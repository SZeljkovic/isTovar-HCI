using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class AddCompanyStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.AddCompanyTitle;
        public string Name => Strings.CompanyName;
        public string Email => Strings.Email;
        public string Account => Strings.Account;
        public string Street => Strings.Street;
        public string Number => Strings.Number;
        public string City => Strings.City;
        public string PostalCode => Strings.PostalCode;
        public string Country => Strings.Country;
        public string Phone => Strings.Phone;
        public string Fax => Strings.Fax;

        public string AddButton => Strings.Add;
        public string ClearButton => Strings.Clear;
        public string CloseButton => Strings.Close;

        public string EmptyFieldsMessage => Strings.EmptyCompanyFields;
        public string SuccessMessage => Strings.CompanyAdded;
        public string DbErrorMessage => Strings.DbError;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
