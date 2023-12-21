using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class DagplanDto
    {
        public List<Evenement> _evenements = new();
        public int DagplanId { get; set; }
        public DateTime Datum { get; set; }
        public Gebruiker gebruiker { get; set; }
        public decimal KostPrijs { get; set; }

        public DagplanDto(Dagplan dagplan)
        {
            _evenements = dagplan.GeefEvenementen();
            Datum = dagplan.Datum;
            this.gebruiker = dagplan.Gebruiker;
            KostPrijs = dagplan.KostPrijs;
        }

        public List<EvenementDto> GeefEvenementen()
        {
            return _evenements.Select(evenement => new EvenementDto(evenement)).ToList();
        }
    }
}
