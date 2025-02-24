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
            Game game = new Game();
            game.Start();
            Console.WriteLine("\n\nEnd of code. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
