using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistentie
{
    public class GebruikerRepoDb : IGebruikerRepo
    {
        private readonly string _connectionString;

        public GebruikerRepoDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Gebruiker> GeefGebruikers()
        {
            List<Gebruiker> gebruikers = new List<Gebruiker>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string selectSql = "SELECT GebruikerId, Voornaam, Achternaam, DagBudget FROM Gebruiker;";

                    using (SqlCommand cmd = new SqlCommand(selectSql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int gebruikerId = reader.GetInt32(0);
                                string voornaam = reader.GetString(1);
                                string achternaam = reader.GetString(2);
                                decimal dagBudget = reader.GetDecimal(3);

                                Gebruiker gebruiker = new Gebruiker(voornaam, achternaam, dagBudget, gebruikerId);
                                gebruikers.Add(gebruiker);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new GentseFeestenException("Fout opgetreden bij het ophalen van de gebruiker");
            }

            return gebruikers;
        }

        public void SaveGebruiker(Gebruiker gebruiker)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insert command opstellen en parameters meegeven
                    string insertSql = $"INSERT INTO Gebruiker (Voornaam, Achternaam, DagBudget)" +
                        $"VALUES (@Voornaam, @Achternaam, @DagBudget);";

                    SqlCommand cmd = new SqlCommand(insertSql, connection);

                    cmd.Parameters.Add("@Voornaam", SqlDbType.VarChar);
                    cmd.Parameters["@Voornaam"].Value = gebruiker.Voornaam;

                    cmd.Parameters.Add("@Achternaam", SqlDbType.VarChar);
                    cmd.Parameters["@Achternaam"].Value = gebruiker.Achternaam;

                    cmd.Parameters.Add("@DagBudget", SqlDbType.Decimal);
                    cmd.Parameters["@DagBudget"].Value = gebruiker.DagBudget;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                throw new GentseFeestenException("Fout opgetreden bij het opslaan van de gebruiker");
            }
        }
    }
}
