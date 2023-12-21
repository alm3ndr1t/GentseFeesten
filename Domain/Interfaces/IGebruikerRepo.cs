using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGebruikerRepo
    {
        public List<Gebruiker> GeefGebruikers();
        public void SaveGebruiker(Gebruiker g);
    }
}
