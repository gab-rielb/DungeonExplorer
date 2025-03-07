using System;
using System.Collections.Generic;
using System.Media;
using System.Security.Authentication;

namespace DungeonExplorer
{
    /// <summary>
    /// Game class contains the main sequence of the game.
    /// </summary>
    internal class Game
    {
        private Player player; // The player object
        private int roomsPassed; // Number of rooms the player successfully passed
        private int forcedDirectionCounter; // Counter for forced movement
        private string forcedDirection; //Direction for forced movement
        private int roomsToEscape = 10; // Number of rooms needed to escape

        /// <summary>
        /// Constructor for the Game class. Initialises a new game object.
        /// </summary>
        public Game()
        {
            // Initialise a new player with default values
            player = new Player("", 0, new List<string>());
            roomsPassed = 0;
            forcedDirectionCounter = 0;
            forcedDirection = "";
            roomsToEscape = 10; // Default to normal difficulty
        }

        /// <summary>
        /// Starts the main game sequence.
        /// </summary>
        public void Start()
        {
            InitialisePlayer(); // Set up the player
            SelectDifficulty(); // Let the player choose the difficulty
            DisplayGameInstructions(); // Show the game instructions
            GameLoop(); // Start the main game loop
            DisplayGameResult(); // Display the outcome of the game
        }

        /// <summary>
        /// Initialises the player by asking for their name and default values.
        /// </summary>
        private void InitialisePlayer()
        {
            Console.WriteLine("Please enter player's name: ");
            string playerName = Console.ReadLine();
            // Set the player's name, health and inventory
            player.Name = playerName;
            player.Health = 100;
            player.Inventory = new List<string>();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nPlayer '{player.Name}' with {player.Health} health created.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPress any key to begin a game.");
            Console.ReadKey();
        }

        /// <summary>
        /// Allows the player to select the game difficulty, which determines the number of rooms to escape.
        /// </summary>
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

            // Sets roomsToEscape based on player's choice
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

        /// <summary>
        /// Display's the game's objectives and instrucions.
        /// </summary>
        private void DisplayGameInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nObjective: Exit the maze." +
                              "\nEach room will have different effects/events." +
                              $"\nYou must pass {roomsToEscape} rooms to escape.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Displays options to the player to check their status or use items.
        /// </summary>
        private void DisplayTurnOptions()
        {
            Console.WriteLine("\n\n---------------------------------\n");
            Console.WriteLine("\nType 'health' to view health" +
                               "\nType 'progress' to view progress" +
                               "\nType 'item' to use an item" +
                               "\nType 'inventory' to view inventory" +
                               "\nPress enter to skip.");
        }

        /// <summary>
        /// Processes the player's input for turn choices.
        /// </summary>
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
                    HandleItemUse(); // Call method to handle item usage
                    break;
                case "inventory":
                    Console.WriteLine($"\n{player.Name}'s inventory: {player.InventoryContents()}.");
                    break;
                case "":
                    // Player chose to skip
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid option. Press any key to continue to game.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    break;
            }
        }

        /// <summary>
        /// The main game loop. Continues until the player's health reaches 0 or they escape the maze.
        /// </summary>
        private void GameLoop()
        {
            while (player.Health > 0 && roomsPassed < roomsToEscape)
            {
                PlayTurn(); // Process a single turn
                DisplayTurnOptions(); // Options to view stats or use items
                ProcessTurnOptions(); // Process the player's choice
            }
        }

        /// <summary>
        /// Displaus the outcome of the game (win or lose).
        /// </summary>
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

        /// <summary>
        /// Handles the use of items in the player's inventory.
        /// </summary>
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
                return; // Cancel item usage
            }

            bool isItemFound = false;
            // Iterate through the player's inventory to find the item
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
                // Handle specific item effects. Only health potion exists.
                if (itemChoice == "health potion")
                {
                    int healAmount = new Random().Next(10, 21); // Random heal amount between 10-20
                    player.Health += healAmount;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You used a Health Potion and regained {healAmount} health!");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Inventory.Remove("Health Potion"); // Remove the used potion
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

        /// <summary>
        /// Handles a single turn of the game, including player movement and room processing.
        /// </summary>
        public void PlayTurn() 
        {
            string direction = "";

            // Checked for forced movement
            if (forcedDirectionCounter > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nYou are forced to go {forcedDirection}!");
                Console.ForegroundColor = ConsoleColor.White;
                direction = forcedDirection;
                forcedDirectionCounter--;
                if (forcedDirectionCounter == 0)
                {
                    forcedDirection = ""; // Reset forced direction after it's used
                }
            }
            else
            {
                // Get player's direction choice
                Console.WriteLine("\nChoose a direction: forward, left, right");
                direction = Console.ReadLine().ToLower();
            }

            Room nextroom;

            // Determine the next room based on the player's direction choice
            switch (direction)
            {
                case "forward":
                case "left":
                case "right":
                    nextroom = Room.GetRandomRoom(); // Get a random room
                    break;

                default:
                    Console.WriteLine("Invalid direction. You stumble around confused.");
                    return; // Invalid direction, end the turn
            }

            // Process the effects of the room. Passes 'ref' paramaters to modify the game state
            nextroom.ProcessRoom(player, ref roomsPassed, ref forcedDirectionCounter, ref forcedDirection);
        }

    }
}
