using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar.Models
{
    public class Driver
    {
        public int IdKorisnika { get; set; }
        public string KorisnickoIme { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public string BrojDozvole { get; set; }
        public string Licenca { get; set; }
        public int? IdKamiona { get; set; }
        public string Dostupnost { get; set; }
    }
}

