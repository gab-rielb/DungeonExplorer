using System;
using System.Collections.Generic;
using System.Media;
using System.Security.Authentication;

namespace DungeonExplorer
{
    /// <summary>
    /// Player class contains the main sequence of the game.
    /// 
    /// Attributes:
    /// (Player) player -> A new object of class Player (creates a player for the game)
    /// (int) roomsPassed -> A way to keep track of how many rooms have been passed
    /// 
    /// Methods:
    /// Start ->  Starts the main logic of the game, completes when game is over
    /// PlayTurn -> Logic for player choice for each iteration while game is not over
    /// </summary>
    internal class Game
    {
        private Player player;
        private int roomsPassed;

        public Game()
        {
            player = new Player("", 0, new List<string>());
            roomsPassed = 0;
        }
        public void Start()
        {
            bool playing = true;
            while (playing)
            {
                // Setting up player
                Console.WriteLine("Please enter player's name: ");
                string playerName = Console.ReadLine();
                player.Name = playerName;

                player.Health = 100;

                player.Inventory = new List<string>();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nPlayer '{player.Name}' with {player.Health} health created.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nPress any key to start the game.");
                Console.ReadKey();

                // Declaring Objective
                Console.WriteLine("\nObjective: Exit the maze." +
                    "\nEach room will either be empty, have a trap or a monster." +
                    "\nEach scenario will do varying damage to you." +
                    "\nDepending on the difficulty you will have to navigate more rooms.");

                // Game loop
                playing = false;
                while (player.Health > 0 && roomsPassed < 10)
                {
                    PlayTurn();
                    Console.WriteLine("\nType 'health' to view health" +
                        "\nType 'progress' to view progress" +
                        "\nPress enter to skip.");
                    string checkstats = Console.ReadLine();
                    switch (checkstats)
                    {
                        case "health":
                            Console.WriteLine($"\n{player.Name}'s health: {player.Health}.");
                            break;
                        case "progress":
                            Console.WriteLine($"\n{player.Name} has passed {roomsPassed} rooms.");
                            break;
                        case "":
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nInvalid option. Press any key to continue to game.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            break;
                    }
                }
                if (player.Health <= 0) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n\n{player.Name} perished in the maze.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n\n{player.Name} successfully escaped the maze!!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        public void PlayTurn() 
        {
            Console.WriteLine("\nChoose a direction: (forward or left or right)");
            string direction = Console.ReadLine().ToLower();

            Room nextroom;

            switch (direction)
            {
                case "forward":
                case "left":
                case "right":
                    nextroom = Room.GetRandomRoom();
                    break;

                default:
                    Console.WriteLine("Invalid direction. You stumble around confused.");
                    return;
            }

            nextroom.ProcessRoom(player, ref roomsPassed);
        }

    }
}
