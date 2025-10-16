using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Resources
{
    public class IssuesStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string IssuesTitle => Strings.IssuesTitle;
        public string Id => Strings.Id;
        public string UserId => Strings.UserID;

        public string CloseButton => Strings.Close;
        public string Content => Strings.Content;
        public string Date => Strings.Date;
        public string Status => Strings.Status;

        public string EditButton => Strings.Edit;
        public string SaveButton => Strings.Save;
        public string CancelButton => Strings.Cancel;
        public string EditIssue => Strings.EditIssue;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
