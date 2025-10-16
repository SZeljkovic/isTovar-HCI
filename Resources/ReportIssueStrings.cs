using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class ReportIssueStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ReportIssueTitle => Strings.ReportIssue;
        public string IssueDescriptionLabel => Strings.IssueDescription;
        public string SubmitButton => Strings.Submit;
        public string CloseButton => Strings.Close;

        public string EmptyErrorMessage => Strings.EmptyErrorMessage;
        public string SuccessMessage => Strings.SuccessMessage;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
