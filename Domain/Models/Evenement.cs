using Domain.DTOs;
using Domain.Exceptions;
using System;

namespace Domain.Models
{
    public class Evenement
    {
        public int EvenementId { get; set; }
        public string UniqueId { get; set; }
        public string Titel { get; set; }
        public string Beschrijving { get; set; }
        public DateTime StartUur { get; set; }
        public DateTime EindUur { get; set; }
        public decimal Prijs { get; set; }

        public Evenement(string uniqueId, string titel, DateTime startUur, DateTime eindUur, decimal prijs, string beschrijving)
        {
            UniqueId = uniqueId;
            Titel = titel;
            StartUur = startUur;
            EindUur = eindUur;
            Prijs = prijs;
            Beschrijving = beschrijving;
        }

        public Evenement(string uniqueId, string titel, DateTime eindUur, DateTime startUur, decimal prijs, string beschrijving, int evenementId) : this(uniqueId, titel, eindUur, startUur, prijs, beschrijving)
        {
            EvenementId = evenementId;
        }

        public override string? ToString()
        {
            return $"{this.UniqueId}, {this.Titel}, {this.StartUur}, {this.EindUur}, {this.Prijs}, {this.Beschrijving}";
        }

        
        public bool EvenementOverlapt(Evenement e)
        {
            return StartUur.TimeOfDay < e.EindUur.TimeOfDay && e.StartUur.TimeOfDay < EindUur.TimeOfDay;
        }
    }
}
