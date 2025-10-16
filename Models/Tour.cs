using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Models
{
    public class Tour
    {
        public int IdTure { get; set; }

        public DateTime VrijemePolaska { get; set; }

        public DateTime VrijemeDolaska { get; set; }

        public string Status { get; set; }

        public int IdVozaca { get; set; }

        public int IdDispecera {  get; set; }

        public long UkupnaCijenaTure { get; set; }

        public string DriverName { get; set; } // New property for driver's full name
        public string DispatcherName { get; set; }


    }
}
