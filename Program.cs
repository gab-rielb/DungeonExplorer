using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
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
            try
            {
                // Create a new game object and start the game
                Game game = new Game();
                game.Start();
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors that occur during the game
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                // Ensure the console window stays open until the user presses a key
                Console.WriteLine("\n\nEnd of code. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}