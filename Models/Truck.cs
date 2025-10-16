using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Models
{
    public class Truck
    {
        public int IdKamiona { get; set; }

        public string Tip { get; set; }

        public string Brend {  get; set; }

        public int KonjskeSnage { get; set; }

        public int? IdPrikolice { get; set; }

        public string Gorivo { get; set; }

        public int GodinaProizvodnje { get; set; }

        public string RegistarskaOznaka { get; set; }

        public int Kilometraza { get; set; }

    }
}
