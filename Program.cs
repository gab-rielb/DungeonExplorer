using System;
using System.IO;
using DungeonExplorer;

/// <summary>
/// Defines the <see cref="Program" />
/// </summary>
internal class Program
{
    /// <summary>
    /// The Main
    /// </summary>
    /// <param name="args">The args<see cref="string[]"/></param>
    internal static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Welcome to Dungeon Explorer!");
        Console.WriteLine("--------------------------");
        Console.WriteLine("Please choose an option:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  1. Start Game");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("  2. Run Tests");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("--------------------------");
        Console.ForegroundColor = ConsoleColor.White;

        string choice = "";
        bool validInput = false;

        while (!validInput) // Loop until valid input is received
    /// The main entry point of the Dungeon Explorer game.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method that initialises and runs the game.
        /// </summary>
        /// <param name="args">Command Line arguments (not used)</param>
        static void Main(string[] args)
        {
            Console.Write("Enter your choice (1 or 2): ");
            Console.ForegroundColor = ConsoleColor.White;
            choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    RunGame();
                    validInput = true;
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Running in Test Mode...");
                    DungeonTester tester = new DungeonTester();
                    tester.RunAllTests();
                    validInput = true;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        Console.WriteLine("\n\nProgramme finished. Press any key to exit...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// The RunGame
    /// </summary>
    internal static void RunGame() // This method starts the game
    {
        try // Check for any exceptions during game startup
        {
            Game game = new Game();
            game.Start();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nA critical error occurred during the game: {ex.Message}");
            Console.WriteLine("--- Stack Trace ---");
            Console.WriteLine(ex.StackTrace);
            Console.ForegroundColor = ConsoleColor.White;

            try
            {
                File.AppendAllText("critical_error_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | GAME CRITICAL ERROR: {ex.ToString()}\n\n");
                Console.WriteLine("Error details logged to critical_error_log.txt");
            }
            catch (Exception logEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Failed to write to error log: {logEx.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                // Ensure the console window stays open until the user presses a key
                Console.WriteLine("\n\nEnd of code. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}