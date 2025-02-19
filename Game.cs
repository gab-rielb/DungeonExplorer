using System;
using System.Collections.Generic;
using System.Media;

namespace DungeonExplorer
{
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
                // Setting up player...
                Console.WriteLine("Please enter player's name: ");
                string playerName = Console.ReadLine();
                player.Name = playerName;

                player.Health = 100;

                player.Inventory = new List<string>();

                Console.WriteLine($"\nPlayer: {player.Name} with {player.Health} health created." +
                    "\nPress any key to begin.");
                Console.ReadKey();

                playing = false;

                Console.WriteLine("\nObjective: Exit the maze.");
                while (player.Health > 0 && roomsPassed < 10)
                {
                    PlayTurn();
                }
                if (player.Health <= 0) 
                {
                    Console.WriteLine($"\n\n{player.Name} perished in the maze.");
                }
                else
                {
                    Console.WriteLine($"\n\n{player.Name} successfully escaped the maze!!");
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
