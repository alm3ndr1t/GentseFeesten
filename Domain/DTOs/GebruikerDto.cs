using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class GebruikerDto
    {
        private List<Dagplan> _dagplan = new();

        public int GebruikerId { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public decimal DagBudget { get; set; }

        public GebruikerDto(Gebruiker gebruiker)
        {
            GebruikerId = gebruiker.GebruikerId;
            Voornaam = gebruiker.Voornaam;
            Achternaam = gebruiker.Achternaam;
            DagBudget = gebruiker.DagBudget;
            _dagplan = gebruiker.GeefDagplannen();
        }

        public override string? ToString()
        {
            return $"{GebruikerId}, {Voornaam}, {Achternaam}, {DagBudget}";
        }

    }
}
