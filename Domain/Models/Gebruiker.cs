using Domain.Exceptions;
using System.Collections.Generic;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Models
{
    public class Gebruiker
    {
        private List<Dagplan> _dagplan = new();

        public int GebruikerId { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public decimal DagBudget { get; set; }

        public Gebruiker(string voornaam, string achternaam, decimal dagBudget)
        {
            Voornaam = voornaam;
            Achternaam = achternaam;
            DagBudget = dagBudget;
        }

        public Gebruiker(string voornaam, string achternaam, decimal dagBudget, int gebruikerId) : this(voornaam, achternaam, dagBudget)
        {
            GebruikerId = gebruikerId;
        }

        public List<Dagplan> GeefDagplannen()
        {
            return _dagplan;
        }

        public void VoegDagplanToe(Dagplan dagplan)
        {
            // Check if a dagplan for the same date already exists
            if (_dagplan.Any(dp => dp.Datum.Date == dagplan.Datum.Date))
            {
                throw new GentseFeestenException("Voor deze datum is er al een dagplan gemaakt");
            } else
            {
                // If no dagplan for the same date exists and there are events, add the new dagplan
                _dagplan.Add(dagplan);
            }
        }

        public List<Dagplan> GeefDagplanOpDate(DateTime date)
        {
            List<Dagplan> gevondenDagplannen = new List<Dagplan>();

            foreach (Dagplan dagplan in _dagplan)
            {
                if (dagplan.Datum.Date == date.Date)
                {
                }
            }
            return gevondenDagplannen;
        }

        public override string ToString()
        {
            return $"{Voornaam}, {Achternaam}, {DagBudget}";
        }
    }
}
