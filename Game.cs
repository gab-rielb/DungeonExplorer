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
        private int roomsToEscape = 10;

        public Game()
        {
            player = new Player("", 0, new List<string>());
            roomsPassed = 0;
            forcedDirectionCounter = 0;
            forcedDirection = "";
            roomsToEscape = 10;
        }

        public void Start()
        {
            InitialisePlayer();
            SelectDifficulty();
            DisplayGameInstructions();
            GameLoop();
            DisplayGameResult();
        }

        private void InitialisePlayer()
        {
            Console.WriteLine("Please enter player's name: ");
            string playerName = Console.ReadLine();
            player.Name = playerName;
            player.Health = 100;
            player.Inventory = new List<string>();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nPlayer '{player.Name}' with {player.Health} health created.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPress any key to begin a game.");
            Console.ReadKey();
        }

        private void SelectDifficulty()
        {
            Console.WriteLine("\nSelect Difficulty:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Easy (5 rooms)");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("2. Normal (10 rooms)");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("3. Hardcore (20 rooms)");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter your choice (1-3): ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    roomsToEscape = 5;
                    break;
                case "2":
                    roomsToEscape = 10;
                    break;
                case "3":
                    roomsToEscape = 20;
                    break;
                default:
                    Console.WriteLine("Invalid choice.  Defaulting to Normal difficulty (10 rooms).");
                    roomsToEscape = 10;
                    break;
            }
        }

        private void DisplayGameInstructions()
        {
            Console.WriteLine("\nObjective: Exit the maze." +
                              "\nEach room will have different effects/events." +
                              $"\nYou must pass {roomsToEscape} rooms to escape.");
        }

        private void DisplayTurnOptions()
        {
            Console.WriteLine("\n\n---------------------------------\n");
            Console.WriteLine("\nType 'health' to view health" +
                               "\nType 'progress' to view progress" +
                               "\nType 'item' to use an item" +
                               "\nType 'inventory' to view inventory" +
                               "\nPress enter to skip.");
        }

        private void ProcessTurnOptions()
        {
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
        private void GameLoop()
        {
            while (player.Health > 0 && roomsPassed < roomsToEscape)
            {
                PlayTurn();
                DisplayTurnOptions();
                ProcessTurnOptions();
            }
        }

        private void DisplayGameResult()
        {
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
