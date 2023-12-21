using Domain.Interfaces;
using Domain.Models;
using Persistentie;
using System;

class Program
{
    static void Main(string[] args)
    {
        IGebruikerRepo gebruikersRepo = new GebruikerRepoFile(@"C:\Users\alm3n\OneDrive\Documenten\School\HoGent\23-24\Programmeren Gevorderd\Eindevaluatie\GentseFeesten\GentseFeesten\users.csv");
        IGebruikerRepo gebruikersRepoDb = new GebruikerRepoDb(@"Data Source=.\SQLEXPRESS;Initial Catalog=gentseDB;Integrated Security=True;");

        IEvenementRepo eventRepo = new EvenementRepoFile(@"C:\Users\alm3n\OneDrive\Documenten\School\HoGent\23-24\Programmeren Gevorderd\Eindevaluatie\GentseFeesten\GentseFeesten\gentse-feesten-evenementen-202324.csv");
        IEvenementRepo eventRepoDb = new EvenementRepoDb(@"Data Source=.\SQLEXPRESS;Initial Catalog=gentseDB;Integrated Security=True;");

        List<Evenement> evenementen = eventRepo.GeefEvenementen();
        foreach (Evenement evenement in evenementen)
        {
            eventRepoDb.SaveEvenement(evenement);
        }

        List<Gebruiker> gebruikers = gebruikersRepo.GeefGebruikers();
        foreach (Gebruiker gebruiker in gebruikers)
        {
            gebruikersRepoDb.SaveGebruiker(gebruiker);
        }
    }
}