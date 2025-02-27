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
            // Development begins
            try
            {
                Game game = new Game();
                game.Start();
            }
            catch (Exception ex)
            {   
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.WriteLine("\n\nEnd of code. Press any key to exit...");
            }
            ;
        }
    }
}
