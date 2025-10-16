using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class MyProfileStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Strings.MyProfile;
        public string Username => Strings.Username;
        public string FirstName => Strings.FirstName;
        public string LastName => Strings.LastName;
        public string Email => Strings.Email;
        public string Password => Strings.Password;
        public string Phone => Strings.Phone;
        public string Status => Strings.Status; // Status dispečera
        public string SaveButton => Strings.SaveButton;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
