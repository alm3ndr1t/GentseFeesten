using Domain.DTOs;
using Domain.Interfaces;
using Domain.Models;

namespace Domain
{
    public class DomainManager
    {
        IEvenementRepo _evenementRepo;
        IGebruikerRepo _gebruikerRepo;
        IDagplanRepo _dagplanRepo;

        public DomainManager(IEvenementRepo evenementRepo, IGebruikerRepo gebruikerRepo, IDagplanRepo dagplanRepo)
        {
            _evenementRepo = evenementRepo;
            _gebruikerRepo = gebruikerRepo;
            _dagplanRepo = dagplanRepo;
        }

        // Gebruikers
        public List<GebruikerDto> GetGebruikers()
        {
            List<Gebruiker> gebruikers = _gebruikerRepo.GeefGebruikers();
            return gebruikers.Select(gebruiker => new GebruikerDto(gebruiker)).OrderBy(g => g.GebruikerId).ToList();
        }

        // Get all users from database
        // if gebruiker with users input exist = return Gebruiker
        // sort list on Id -> fName -> lName
        public List<GebruikerDto> ZoekGebruikerViaNaam(string naam)
        {
            List<Gebruiker> gevondenGebruikers = new();

            foreach (Gebruiker gebruiker in _gebruikerRepo.GeefGebruikers())
            {
                string volledigeNaam = $"{gebruiker.Voornaam} {gebruiker.Achternaam}";
                volledigeNaam = volledigeNaam.ToLower();

                if (volledigeNaam.Contains(naam.ToLower()))
                {
                    gevondenGebruikers.Add(gebruiker);
                }
            }
            return gevondenGebruikers.Select(gebruiker => new GebruikerDto(gebruiker)).OrderBy(g => g.GebruikerId).ToList();
        }

        // Evenement
        // Get all events -> sort on StartUur
        public List<EvenementDto> GeefEvenementen()
        {
            List<Evenement> evenementen = _evenementRepo.GeefEvenementen();
            return evenementen.Select(evenement => new EvenementDto(evenement)).OrderBy(e => e.StartUur).ToList();
        }

        // Get all events where start = selectedDate then sort on StartUUr
        public List<EvenementDto> GeefEvenementOpDatum(DateTime datum)
        {
            List<Evenement> gevondenEvenementen = new();

            foreach (Evenement evenement in _evenementRepo.GeefEvenementen())
            {
                // Compare the StartUur without adjusting to the local time zone
                if (evenement.StartUur.Date == datum.Date)
                {
                    gevondenEvenementen.Add(evenement);
                }
            }

            // Sort the list by the StartUur
            return gevondenEvenementen.Select(evenemenent => new EvenementDto(evenemenent)).OrderBy(e => e.StartUur).ToList();
        }



        // Search events based on users input
        public List<EvenementDto> ZoekEvenementenOpTitel(List<EvenementDto> evenementen, string naam)
        {
            List<EvenementDto> gevondenEvenementen = new();
            foreach (EvenementDto evenement in evenementen)
            {
                if (evenement.Titel.Contains(naam))
                {
                    gevondenEvenementen.Add(evenement);
                }
            }
            return gevondenEvenementen;
        }

        // Dagplan
        // Get dagplan on date
        public DagplanDto GetDagplanOnDate(DateTime datum)
        {
            Dagplan gevondenDagplan = _dagplanRepo.GeefDagplanOpDatum(datum);

            return new DagplanDto (gevondenDagplan);
        } 

        // Save dagplan in database
        public void SaveDagplan(Dagplan dagplan)
        {
            _dagplanRepo.SaveDagplan(dagplan);
        }
        
        // dto omzetten in evenement en save in to database
        public void SaveEvenementInDagplan(EvenementDto evDto, Dagplan dp)
        {
            Evenement eve = evDto.ParseEvenementDto();
            dp.VoegEvenementToe(eve);
        }

        public bool DagplanExistsOnDate(DateTime date)
        {
            // Check if a Dagplan already exists for the given date
            return _dagplanRepo.DagplanExistsOnDate(date);
        }
    }
}