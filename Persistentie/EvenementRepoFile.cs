using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Persistentie
{
    public class EvenementRepoFile : IEvenementRepo
    {
        private List<Evenement> _evenement;

        public EvenementRepoFile(string bestand)
        {
            _evenement = new List<Evenement>();

            try
            {
                using (var reader = new StreamReader(bestand))
                {
                    string headerLine = reader.ReadLine();
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] elementen = line.Split(';');

                        if (string.IsNullOrWhiteSpace(elementen[4]))
                        {
                            elementen[4] = "0";
                        }

                        _evenement.Add(new Evenement(
                            elementen[0],
                            elementen[1].Replace("\"", ""),
                            DateTime.Parse(elementen[2].Substring(0, 19)),
                            DateTime.Parse(elementen[3].Substring(0, 19)),
                            decimal.Parse(elementen[4]),
                            elementen[5]
                            ));
                    }
                }

            }
            catch (ArgumentException ax)
            {
                Console.WriteLine("Onverwachte fout opgetreden");
            }
        }


        public List<Evenement> GeefEvenementen()
        {
            return _evenement;
        }

        public void SaveEvenement(Evenement e)
        {
            _evenement.Add(e);
        }
    }
}
