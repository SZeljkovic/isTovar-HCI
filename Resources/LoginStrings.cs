using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class LoginStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.LoginTitle;
        public string Username => Strings.Username;
        public string Password => Strings.Password;
        public string LoginButton => Strings.LoginTitle;
        public string RegisterLink => Strings.Register;
        public string NoAccountMessage => Strings.NoAccount;
        public string EmptyFieldsMessage => Strings.EmptyLoginFields;
        public string InvalidCredentialsMessage => Strings.InvalidCredentials;
        public string DbErrorMessage => Strings.DbError;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
