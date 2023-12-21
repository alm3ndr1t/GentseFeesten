using Domain.DTOs;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Models
{
    public class Dagplan
    {
        private List<Evenement> _evenement = new();

        public int DagplanId { get; set; }
        public DateTime Datum { get; set; }
        public Gebruiker Gebruiker { get; set; }
        public decimal KostPrijs { get; set; }

        public Dagplan(DateTime datum, Gebruiker gebruiker)
        {
            Datum = datum;
            Gebruiker = gebruiker;
            KostPrijs = GeefTotaleKostprijs();
        }

        public List<Evenement> GeefEvenementen()
        {
            return _evenement;
        }

        public decimal GeefTotaleKostprijs()
        {
            decimal prijs = 0;
            foreach (Evenement ev in _evenement)
            {
                prijs += ev.Prijs;
            }
            return prijs;
        }

        public void VoegEvenementToe(Evenement evenement)
        {
            // Check if the event already exists in the dagplan
            if (_evenement.Any(ev => evenement.EvenementOverlapt(ev) || evenement.Equals(ev)))
            {
                throw new GentseFeestenException("Gekozen evenement zit al in het dagplan of overlapt met een ander evenement.");
            }
            // Check if adding the event exceeds the user's budget
            else if (GeefTotaleKostprijs() + evenement.Prijs > Gebruiker.DagBudget)
            {
                throw new GentseFeestenException("Er is geen budget meer om dit evenement toe te voegen");
            } else
            {
                // Add the event to the dagplan
                _evenement.Add(evenement);
            }
        }


        public bool DagplanForEvenementDatum(DateTime datum)
        {
            return _evenement.Any(e => e.StartUur.Date == datum.Date);
        }

        public Evenement ParseEvenementDto(EvenementDto dto)
        {
            Evenement evenement = new(dto.UniqueId ,dto.Titel, dto.StartUur, dto.EindUur, dto.Prijs, dto.Beschrijving);
            return evenement;
        }

    }
}
