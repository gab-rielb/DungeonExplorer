using System;
using System.Runtime.InteropServices;

namespace DungeonExplorer
{
    /// <summary>
    /// Room class will randomly generate a room with a variety of options.
    /// 
    /// Attributes:
    /// (const)(string) Pass -> A way to generate a new room that is safe
    /// (const)(string) SlightDamage -> A way to generate a new room that inflicts slight damage
    /// (const)(string) HeavyDamage -> A way to generate a new room that inflicts heavy damage
    /// (static)(Random) _random -> A way to generate a random number whenever needed
    /// 
    /// Methods:
    /// GetRandomRoom -> Randomly generates and returns one of three room types
    /// ProcessRoom -> Proceeds the user with(out) consequences depending on room type
    /// </summary>
    public class Room
    {
        // Room Attributes
        public const string Pass = "Pass";
        public const string SlightDamage = "SlightDamage";
        public const string HeavyDamage = "HeavyDamage";
        private static Random _random = new Random();

        // Room Accessors
        public string Type { get; }

        // Room Constructor
        public Room(string type)
        {
            Type = type;
        }

        // Room Methods
        public static Room GetRandomRoom()
        {
            int randomnum = _random.Next(1, 4);
            switch (randomnum)
            {
                case 1:
                    return new Room(Pass);
                case 2:
                    return new Room(SlightDamage);
                case 3:
                    return new Room(HeavyDamage);
                default:
                    return new Room(Pass);
            }
        }
        public void ProcessRoom(Player player, ref int roomsPassed)
        {
            switch (Type)
            {
                case Pass:
                    Console.WriteLine("\nYou found an empty room. You moved ahead!");
                    roomsPassed++;
                    break;
                case SlightDamage:
                    int damage = _random.Next(5, 16);
                    player.Health -= damage;
                    Console.WriteLine($"\nYou fell into a small trap!!\nYou took {damage} damage!");
                    roomsPassed++;
                    break;
                case HeavyDamage:
                    damage = _random.Next(15, 26);
                    player.Health -= damage;
                    Console.WriteLine($"\nA monster attacked you!!\nYou took {damage} damage!");
                    roomsPassed++;
                    break;
                default:
                    Console.WriteLine("\n\n\nAn error occured genereating your room.");
                    break;
            }
        }
        
    }
}