using System;
using System.Collections.Generic;
using System.Media;
using System.Security.Authentication;

namespace DungeonExplorer
{
    /// <summary>
    /// Game class contains the main sequence of the game.
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
        private int forcedDirectionCounter;
        private string forcedDirection;

        public Game()
        {
            player = new Player("", 0, new List<string>());
            roomsPassed = 0;
            forcedDirectionCounter = 0;
            forcedDirection = "";
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

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nPlayer '{player.Name}' with {player.Health} health created.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nPress any key to start the game.");
                Console.ReadKey();

                // Declaring Objective
                Console.WriteLine("\nObjective: Exit the maze." +
                    "\nEach room will have different effects/events." +
                    "\nDepending on the difficulty you will have to navigate more rooms.");

                // Game loop
                playing = false;
                while (player.Health > 0 && roomsPassed < 10)
                {
                    PlayTurn();
                    Console.WriteLine("\nType 'health' to view health" +
                        "\nType 'progress' to view progress" +
                        "\nType 'item' to use an item" +
                        "\nType 'inventory' to view inventory" +
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
                        case "item":
                            HandleItemUse();
                            break;
                        case "inventory":
                            Console.WriteLine($"\n{player.Name}'s inventory: {player.InventoryContents()}.");
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

        private void HandleItemUse()
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("\nYour inventory is empty.");
                return;
            }

            Console.WriteLine($"\nInventory: {player.InventoryContents()}");
            Console.WriteLine("Enter the name of the item to use, or type 'skip' to cancel:");
            string itemChoice = Console.ReadLine().ToLower();

            if (itemChoice == "skip")
            {
                return;
            }

            bool isItemFound = false;
            foreach (string item in player.Inventory)
            {
                if (item.ToLower() == itemChoice)
                {
                    isItemFound = true;
                    break;
                }
            }

            if (isItemFound)
            {
                if (itemChoice == "health potion")
                {
                    int healAmount = new Random().Next(10, 21);
                    player.Health += healAmount;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You used a Health Potion and regained {healAmount} health!");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Inventory.Remove("Health Potion");
                }
                else
                {
                    Console.WriteLine("You cannot use that item right now.");
                }
            }
            else
            {
                Console.WriteLine("You do not have that item in your inventory.");
            }
        }
        public void PlayTurn() 
        {
            string direction = "";

            if (forcedDirectionCounter > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nYou are forced to go {forcedDirection}!");
                Console.ForegroundColor = ConsoleColor.White;
                direction = forcedDirection;
                forcedDirectionCounter--;
                if (forcedDirectionCounter == 0)
                {
                    forcedDirection = ""; // Reset forced direction
                }
            }
            else
            {
                Console.WriteLine("\nChoose a direction: forward, left, right");
                direction = Console.ReadLine().ToLower();
            }

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

            nextroom.ProcessRoom(player, ref roomsPassed, ref forcedDirectionCounter, ref forcedDirection);
        }

    }
}
