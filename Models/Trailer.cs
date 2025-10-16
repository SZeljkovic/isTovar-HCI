using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Models
{
    public class Trailer
    {
        public int IdPrikolice { get; set; }

        public string Vrsta { get; set; }

        public int Nosivost { get; set; }

        public int GodinaProizvodnje { get; set; }

        public string RegistarskaOznaka { get; set; }
    }
}
