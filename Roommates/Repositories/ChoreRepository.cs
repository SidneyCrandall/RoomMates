using System;
using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Chore data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class ChoreRepository : BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Chores in the database
        /// </summary>
        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.

            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();
                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the chores we retrieve from the database.
                    List<Chore> chores = new List<Chore>();

                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        // Now let's create a new room object using the data from the database.
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };

                        // ...and add that room object to our list.
                        chores.Add(chore);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of chores.
                    return chores;
                }
            }
        }

        // Get chores by id
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }

                    reader.Close();

                    return chore;
                }
            }
        }

        // Display the chores that arent attached to a Room Mate
        public List<Chore> UnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT chore.Id, chore.Name 
                                        FROM Chore
                                        LEFT JOIN roommateChore on chore.Id = roommateChore.ChoreId
                                        WHERE roommateChore.Id IS NULL;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Chore> UnassignedChores = new List<Chore>();

                    while (reader.Read())
                    {
                        Chore chore = new Chore
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };

                        UnassignedChores.Add(chore);
                    }

                    reader.Close();
                    return UnassignedChores;
                }
            }
        }

        // Now lets Assign those chores
        public void AssignChore(int roommate, int chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // OUTPUT INSERTED.id needed to help from throwing an exception. 
                    // the Roommates and the chores need to be connected in order for them to be assigned
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId)
                                            OUTPUT INSERTED.id
                                            VALUES (@roommateId, @choreId)";
                    cmd.Parameters.AddWithValue("@roommateId", roommate);
                    cmd.Parameters.AddWithValue("@choreId", chore);

                    cmd.ExecuteScalar();

                    int chorAssigned = (int)cmd.ExecuteScalar();

                }
            }
        }

        ///<summary>
        ///Add a new chore to the database
        ///Note: This method SENDS data to the db and retrieves nothing,
        ///so there is nothing to return
        ///</summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }

        /// <summary>
        ///  Updates the chore
        /// </summary>
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Room
                                    SET Name = @name
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@id", chore.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Delete the chore with the given id
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // What do you think this code will do if there is a roommate in the room we're deleting???
                    cmd.CommandText = "DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method used to get the chore count
       /* public List<ChoreCount> GetChoreCount()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(Chore.Name) as CountOfChores, Roommate.FirstName 
                                        FROM Roommate 
                                        LEFT JOIN RoommateChore on RoommateChore.RoommateId = Roommate.Id
                                        LEFT JOIN Chore on CHore.Id = RoommateChore.ChoreId
                                        GROUP BY Roommate.Id, Roommate.FirstName";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<ChoreCount> counts = new List<ChoreCount>();

                    while (reader.Read())
                    {
                        ChoreCount choreCount = new ChoreCount
                        {
                            Name = reader.GetString(reader.GetOrdinal("FirstName"))
                            CountOfChores = reader.GetInt32(reader.GetOrdinal("CountOfChores"))
                        };
                        counts.Add(choreCount);
                    }
                    reader.Close();
                    return choreCount;*/
                }
            }
        }
    }
}
                      /*  int firstNameColumn = reader.GetOrdinal("FirstName");
                        string firstName = reader.GetString(firstNameColumn);

                        int choreCountColumn = reader.GetOrdinal("Count");
                        int choresCounted = reader.GetInt32(choreCountColumn);*/