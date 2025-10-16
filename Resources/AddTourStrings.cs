using System;
using System.ComponentModel;

namespace Projekat_B_isTovar.Resources
{
    public class AddTourStrings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string AddTourTitle => Strings.AddTour;
        public string DepartureDate => Strings.DepartureDate; 
        public string DepartureTime => Strings.DepartureTime; // dodaj u resx ako hoćeš prevod
        public string ArrivalDate => Strings.ArrivalDate; // isto "Date"
        public string ArrivalTime => Strings.ArrivalTime; // dodaj u resx ako hoćeš prevod
        public string Status => Strings.Status;
        public string DriverId => Strings.DriverID;
        public string DispatcherId => Strings.DispatcherId;
        public string FinalPrice => Strings.FinalPrice;
        public string SelectDriver => Strings.SelectDriver; // može i poseban string "SelectDriver"
        public string CloseButton => Strings.Close;
        public string SubmitButton => Strings.Submit; // dodaj u resx
        public string CancelButton => Strings.Cancel;  // dodaj u resx

        public string EnterStatus => Strings.EnterStatus;

        public string EnterFinalPrice => Strings.EnterFinalPrice;

        public string SelectDate => Strings.SelectDate;

        public string Hours => Strings.Hours;

        public string Minutes => Strings.Minutes;

        public string ChoosenDriver => Strings.ChoosenDriver;


        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
