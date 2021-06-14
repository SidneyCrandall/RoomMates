using Roommates.Models;
using Roommates.Repositories;
using System;
using System.Collections.Generic;
using.Linq;

namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true";

        static void Main(string[] args)
        {

            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Search for room"):
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Add a room"):
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Remove a room from renting"):
                        List<Room> removeRoom = roomRepo.GetAll(); 
                        foreach (Room rm in removeRoom)
                        {
                            Console.WriteLine($"{rm.Id} - {rm.Name}");
                        }

                        Console.WriteLine("Pick a room to remove:");
                        int roomId = int.Parse(Console.ReadLine());

                        // We dont want to remove a room that someone is renting at them moment...
                        try
                        {
                            roomRepo.Delete(roomId);
                        }
                        catch
                        {
                            Console.WriteLine("Someone is renting the room. it cannot be removed, at this time");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        Console.WriteLine($"Room was successfully removed!"); 

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    // Chore switch/case 
                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (Chore chre in chores)
                        {
                            Console.WriteLine($"{chre.Name} has an Id of {chre.Id}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Search for a chore"):
                        Console.Write("Chore Id: ");
                        int choreId = int.Parse(Console.ReadLine());

                        Chore chore = choreRepo.GetById(choreId);

                        Console.WriteLine($"{chore.Id} - {chore.Name}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Add a chore"):
                        Console.Write("Chore name: ");
                        string choreName = Console.ReadLine();

                        Chore choreToAdd = new Chore()
                        {
                            Name = choreName, 
                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Show all unassigned chores"):
                        List<Chore> UnassignedChores = choreRepo.UnassignedChores();

                        foreach(Chore chre in UnassignedChores)
                        {
                            Console.WriteLine($"Id: {chre.Id} - Chore: {chre.Name}");
                        };

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Assign a chore"):
                        List<Chore> assignChores = choreRepo.GetAll();
                        List<Roommate> roomatesChore = roommateRepo.GetAll();
                        // A way for the user to see all the chores before assigning them.
                        Console.WriteLine("Chores to be Assigned:");
                        Console.WriteLine("----------------------");
                        foreach (Chore chre in assignChores)
                        {
                            Console.WriteLine($"{chre.Id} - {chre.Name}");
                        }

                        // Prompt for user to assign a chore
                        Console.WriteLine();
                        Console.WriteLine("Which chore would you like to assign?:");
                        int choreAssignment = int.Parse(Console.ReadLine());

                        // A list of the RoomMates for a user to asign chores to 
                        Console.WriteLine("Roommates to be Assigned Chores:");
                        Console.WriteLine("--------------------------------");
                        foreach (Roommate rmmte in roomatesChore)
                        {
                            Console.WriteLine($"{rmmte.Id} - {rmmte.FirstName} {rmmte.LastName}");
                        }

                        // Prompt for user to assign a roomMate to a chore
                        Console.WriteLine();
                        Console.WriteLine("Which roomate should do this chore?:");
                        int assignedMate = int.Parse(Console.ReadLine());

                        choreRepo.AssignChore(choreAssignment, assignedMate);

                        Roommate mate = roommateRepo.GetById(assignedMate);
                        Chore task = choreRepo.GetById(choreAssignment);

                        Console.WriteLine($"The chore of {task.Name} was assigned to {mate.FirstName}");

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    // Get a RoomMate by Id
                    case ("Select a roommate"):
                        Console.Write("RoomMate Id: ");

                        int roommateId = int.Parse(Console.ReadLine());

                        Roommate roommate = roommateRepo.GetById(roommateId);

                        Console.WriteLine($"{roommate.Id} - {roommate.FirstName} pays {roommate.RentPortion}% of rent. They occupy {roommate.Room.Name}.");

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                // ROOMS 
                "Show all rooms",
                "Search for room",
                "Add a room",
                // CHORES
                "Show all chores",
                "Search for a chore",
                "Show all unassigned chores",
                "Assign a chore",
                "Add a chore",
                // ROOMMATES
                "Select a roommate",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}