using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistentie
{
    public class EvenementRepoDb : IEvenementRepo
    {
        private readonly string _connectionString;

        public EvenementRepoDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Evenement> GeefEvenementen()
        {
            List<Evenement> evenementen = new List<Evenement>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string selectSql = "SELECT UniqueId, Titel, StartUur, EindUur, Prijs, Beschrijving FROM Evenement;";

                    using (SqlCommand cmd = new SqlCommand(selectSql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                evenementen.Add(new Evenement(
                                    (string)reader["UniqueId"],
                                    (string)reader["Titel"],
                                    (DateTime)reader["StartUur"],
                                    (DateTime)reader["EindUur"],
                                    reader["Prijs"] != DBNull.Value ? (decimal)reader["Prijs"] : 0,
                                    reader["Beschrijving"] != DBNull.Value ? (string)reader["Beschrijving"] : "Geen beschrijving beschikbaar"
                                    ));
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log or handle the SQL exception
                throw new GentseFeestenException($"Error while retrieving events from the database. SQL Error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new GentseFeestenException($"Fout opgetreden bij het ophalen van het evenement | Reden: {ex.Message}");
            }
            return evenementen;
        }

        public void SaveEvenement(Evenement evenement)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insert command and parameters
                    string insertSql = $"INSERT INTO Evenement (UniqueId, Titel, StartUur, EindUur, Prijs, Beschrijving)" +
                        $"VALUES (@UniqueId, @Titel, @StartUur, @EindUur, @Prijs, @Beschrijving);";

                    SqlCommand cmd = new(insertSql, connection);

                    cmd.Parameters.Add("@UniqueId", SqlDbType.VarChar);
                    cmd.Parameters["@UniqueId"].Value = evenement.UniqueId;

                    cmd.Parameters.Add("@Titel", SqlDbType.VarChar);
                    cmd.Parameters["@Titel"].Value = evenement.Titel;

                    cmd.Parameters.Add("@StartUur", SqlDbType.DateTime);
                    cmd.Parameters["@StartUur"].Value = evenement.StartUur;

                    cmd.Parameters.Add("@EindUur", SqlDbType.DateTime);
                    cmd.Parameters["@EindUur"].Value = evenement.EindUur;

                    cmd.Parameters.Add("@Prijs", SqlDbType.Decimal);
                    cmd.Parameters["@Prijs"].Value = evenement.Prijs;

                    if (String.IsNullOrWhiteSpace(evenement.Beschrijving))
                    {
                        cmd.Parameters.AddWithValue("@Beschrijving", DBNull.Value);
                    } else
                    {
                        cmd.Parameters.AddWithValue("@Beschrijving", evenement.Beschrijving);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                // Log or display the SQL exception details
                string errorMessage = $"Error while saving the event. SQL Error: {sqlEx.Message}, LineNumber: {sqlEx.LineNumber}, ErrorCode: {sqlEx.ErrorCode}";
                throw new GentseFeestenException(errorMessage);
            }
            catch (Exception ex)
            {
                throw new GentseFeestenException($"Het evenement kon niet worden opgeslagen | Reden: {ex.Message}");
            }
        }
    }
}
