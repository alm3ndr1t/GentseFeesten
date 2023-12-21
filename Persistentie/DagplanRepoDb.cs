using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Persistentie
{
    public class DagplanRepoDb : IDagplanRepo
    {
        private readonly string _connectionString;
        private Gebruiker _gebruiker;
        

        public DagplanRepoDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool DagplanExistsOnDate(DateTime date)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if there is any Dagplan for the given date
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Dagplan WHERE Datum = @datum", connection))
                    {
                        cmd.Parameters.Add("@datum", SqlDbType.DateTime).Value = date;
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log or handle the SQL exception
                throw new GentseFeestenException($"Error while checking if Dagplan exists for the given date from the database. SQL Error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new GentseFeestenException($"Error while checking if Dagplan exists for the given date. Error: {ex.Message}", ex);
            }
        }

        public Dagplan GeefDagplanOpDatum(DateTime datum)
        {
            try
            {
                List<Evenement> evenementen = new List<Evenement>();
                Dagplan dagplan = new Dagplan(datum, new Gebruiker("", "", 0));

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Retrieve events for the given date
                    using (SqlCommand cmdEvents = new SqlCommand(
                        "SELECT EvenementenInDagplan.UniqueId, Evenement.Titel, Evenement.StartUur, Evenement.EindUur, Evenement.Prijs, Evenement.Beschrijving " +
                        "FROM Dagplan " +
                        "JOIN EvenementenInDagplan ON Dagplan.DagplanId = EvenementenInDagplan.DagplanId " +
                        "JOIN Evenement ON EvenementenInDagplan.UniqueId = Evenement.UniqueId " +
                        "WHERE Dagplan.Datum = @datum;", connection))
                    {
                        cmdEvents.Parameters.Add("@datum", SqlDbType.DateTime).Value = datum;

                        using (SqlDataReader reader = cmdEvents.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Evenement evenement = new Evenement(
                                    (string)reader["UniqueId"],
                                    (string)reader["Titel"],
                                    (DateTime)reader["StartUur"],
                                    (DateTime)reader["EindUur"],
                                    (decimal)reader["Prijs"],
                                    reader["Beschrijving"] != DBNull.Value ? (string)reader["Beschrijving"] : "Geen beschrijving beschikbaar"
                                );

                                evenementen.Add(evenement);
                            }
                        }
                    }

                    // Retrieve user details for the given date
                    using (SqlCommand cmdUser = new SqlCommand(
                        "SELECT g.Voornaam, g.Achternaam, g.DagBudget, dp.GebruikerId, dp.KostPrijs " +
                        "FROM Dagplan dp " +
                        "JOIN Gebruiker g ON dp.GebruikerId = g.GebruikerId " +
                        "WHERE dp.Datum = @datum;", connection))
                    {
                        cmdUser.Parameters.Add("@datum", SqlDbType.DateTime).Value = datum;

                        using (SqlDataReader reader = cmdUser.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dagplan.Gebruiker = new Gebruiker(
                                        (string)reader["Voornaam"],
                                        (string)reader["Achternaam"],
                                        (decimal)reader["DagBudget"],
                                        (int)reader["GebruikerId"]
                                    );

                                    dagplan.KostPrijs = (decimal)reader["KostPrijs"];
                                }
                            } else
                            {
                                throw new GentseFeestenException("De gebruiker van het dagplan werd niet gevonden");
                            }
                        }
                    }
                }

                foreach (Evenement ev in evenementen)
                {
                    dagplan.VoegEvenementToe(ev);
                }

                dagplan.Gebruiker.VoegDagplanToe(dagplan);
                return dagplan;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle the SQL exception
                throw new GentseFeestenException($"Error while retrieving events for the given date from the database. SQL Error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new GentseFeestenException($"Onverwachte fout opgetreden bij het ophalen van het evenement | Reden: {ex.Message}");
            }
        }

        public void SaveDagplan(Dagplan dagplan)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Start a transaction
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert the dagplan into the Dagplan table
                            using (SqlCommand cmdInsertDagplan = new SqlCommand(
                                "INSERT INTO Dagplan (Datum, GebruikerId, KostPrijs) " +
                                "VALUES (@datum, @gebruikerId, @kostPrijs);" +
                                "SELECT SCOPE_IDENTITY();", connection, transaction))
                            {
                                cmdInsertDagplan.Parameters.Add("@datum", SqlDbType.DateTime).Value = dagplan.Datum;
                                cmdInsertDagplan.Parameters.Add("@gebruikerId", SqlDbType.Int).Value = dagplan.Gebruiker.GebruikerId;
                                cmdInsertDagplan.Parameters.Add("@kostPrijs", SqlDbType.Decimal).Value = dagplan.KostPrijs;

                                dagplan.DagplanId = Convert.ToInt32(cmdInsertDagplan.ExecuteScalar());
                            }

                            // Insert events into the DagplanEvenement table with sequential ListId
                            foreach (Evenement evenement in dagplan.GeefEvenementen())
                            {
                                int listId = 0;
                                using (SqlCommand cmdInsertEvent = new SqlCommand(
                                    "INSERT INTO EvenementenInDagplan (DagplanId, UniqueId) " +
                                    "VALUES (@dagplanId, @uniqueId);", connection, transaction))
                                {
                                    cmdInsertEvent.Parameters.Add("@uniqueId", SqlDbType.VarChar).Value = evenement.UniqueId;
                                    cmdInsertEvent.Parameters.Add("@dagplanId", SqlDbType.Int).Value = dagplan.DagplanId;

                                    cmdInsertEvent.ExecuteNonQuery();
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception exe)
                        {
                            // Rollback the transaction in case of an exception
                            transaction.Rollback();
                            throw new GentseFeestenException($"Opslaan dagplan mislukt. Reden: {exe.Message}");
                        }
                    }
                }
            }
            catch (GentseFeestenException ex)
            {
                throw ex;
            }
        }
    }
}
