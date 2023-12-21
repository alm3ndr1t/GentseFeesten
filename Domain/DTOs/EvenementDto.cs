using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class EvenementDto
    {
        public int EvenementId { get; set; }
        public string UniqueId { get; set; }
        public string Titel { get; set; }
        public DateTime StartUur { get; set; }
        public DateTime EindUur { get; set; }
        public decimal Prijs { get; set; }
        public string Beschrijving { get; set; }

        public EvenementDto(Evenement evenement)
        {
            EvenementId = evenement.EvenementId;
            UniqueId = evenement.UniqueId;
            Titel = evenement.Titel;
            StartUur = evenement.StartUur;
            EindUur = evenement.EindUur;
            Prijs = evenement.Prijs;
            Beschrijving = evenement.Beschrijving;
        }

        public Evenement ParseEvenementDto()
        {
            Evenement evenement = new(UniqueId, Titel, StartUur, EindUur, Prijs, Beschrijving, EvenementId);

            return evenement;
        }
    }
}
