using System;
using System.Collections.Generic;
using System.Media;

namespace DungeonExplorer
{
    internal class Game
    {
        private Player player;
        private Room currentRoom;

        public Game()
        {
            player = new Player("", 0, new List<string>());
            currentRoom = new Room("");
        }
        public void Start()
        {
            bool playing = true;
            while (playing)
            {
                try
                {

                    // Setting up player...
                    Console.WriteLine("Please enter player's name: ");
                    string playerName = Console.ReadLine();
                    player.Name = playerName;

                    player.Health = 100;

                    player.Inventory = new List<string>();

                    Console.WriteLine($"Player: {player.Name} with {player.Health} health created." +
                        "\nPress any key to begin.");
                    Console.ReadKey();

                    //playing = false;

                    while (player.Health > 0) 
                    {
                        // Actual game logic here.
                    }

                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"Error: {e.ParamName} cannot be null.\nPress any key to continue.");
                    Console.ReadKey();
                }
            }
        }
    }
}