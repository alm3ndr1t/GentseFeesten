using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie
{
    public class GebruikerRepoFile : IGebruikerRepo
    {
        private List<Gebruiker> _gebruiker;

        public GebruikerRepoFile(string bestand)
        {
            _gebruiker = new List<Gebruiker>();

            try
            {
                using (var reader = new StreamReader(bestand))
                {
                    string headLine = reader.ReadLine();
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] elementen = line.Split(';');

                        // Check if the array has the expected number of elements
                        if (elementen.Length >= 3)
                        {
                            _gebruiker.Add(new Gebruiker(
                                elementen[0],
                                elementen[1],
                                decimal.Parse(elementen[2])
                            ));
                        } else
                        {
                            // Log or handle the case where the line doesn't have enough elements
                            Console.WriteLine($"Skipped invalid line: {line}");
                        }
                    }
                }
            }
            catch (ArgumentException ax)
            {
                Console.WriteLine("Onverwachte fout opgetreden.");
            }
        }

        public List<Gebruiker> GeefGebruikers()
        {
            return _gebruiker;
        }

        public void SaveGebruiker(Gebruiker g)
        {
            _gebruiker.Add(g);
        }
    }
}