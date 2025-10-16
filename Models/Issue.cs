using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Models
{
    public class Issue
    {
        public int IdProblema { get; set; }

        public int IdKorisnika { get; set; }

        public string TekstProblema { get; set; }

        public DateOnly Datum { get; set; }

        public string Status { get; set; }
    }
}
