using System;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with RoomMate data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    class RoommateRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRepository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT FirstName, LastName, RentPortion, Room.Name
                                        FROM Roommate
                                        JOIN Room on RoomId = room.Id";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    if (reader.Read())
                    {
                        Room room = new Room
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };

                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            Room = room,
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }
    }
}