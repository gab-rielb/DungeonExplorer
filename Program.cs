using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    internal class Program
    {
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
