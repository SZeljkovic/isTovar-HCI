using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class RegisterStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.Register;
        public string Role => Strings.Role;
        public string Dispatcher => Strings.Dispatcher;
        public string Driver => Strings.Driver;
        public string Username => Strings.Username;
        public string FirstName => Strings.FirstName;
        public string LastName => Strings.LastName;
        public string Email => Strings.Email;
        public string Password => Strings.Password;
        public string Phone => Strings.Phone;
        public string LicenseNumber => Strings.LicenseNumber;
        public string License => Strings.License;
        public string RegisterButton => Strings.RegisterButton;
        public string EmptyFieldsMessage => Strings.EmptyRegisterFields;
        public string DbErrorMessage => Strings.DbError;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
