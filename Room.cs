using System;
using System.Collections.Generic;
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
        public const string Lucky = "Lucky";
        public const string Unlucky = "Unlucky";
        public const string Mystery = "Mystery";

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
            int randomnum = _random.Next(1, 7);
            switch (randomnum)
            {
                case 1:
                    return new Room(Pass);
                case 2:
                    return new Room(SlightDamage);
                case 3:
                    return new Room(HeavyDamage);
                case 4:
                    return new Room(Lucky);
                case 5:
                    return new Room(Unlucky);
                case 6:
                    return new Room(Mystery);
                default:
                    return new Room(Pass);
            }
        }
        public void ProcessRoom(Player player, ref int roomsPassed, ref int forcedDirectionCounter, ref string forcedDirection)
        {

            GenerateRoomDescription();

            switch (Type)
            {
                case Pass:
                    roomsPassed++; // Increment roomsPassed
                    break;

                case SlightDamage:
                    int damage = _random.Next(5, 16);
                    player.Health -= damage;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"You took {damage} damage!");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed++; // Increment roomsPassed
                    break;

                case HeavyDamage:
                    damage = _random.Next(15, 26);
                    player.Health -= damage;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"You took {damage} damage!");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed++; // Increment roomsPassed
                    break;

                case Lucky:
                    ProcessLuckyRoom(player, ref roomsPassed);
                    roomsPassed++;
                    break;

                case Unlucky:
                    ProcessUnluckyRoom(player, ref roomsPassed, ref forcedDirectionCounter, ref forcedDirection);
                    // roomsPassed incremented within ProcessUnluckyRoom
                    break;

                case Mystery:
                    ProcessMysteryRoom(player, ref roomsPassed, ref forcedDirectionCounter, ref forcedDirection); //pass all the way through
                    //roomsPassed incremented within ProcessMysteryRoom
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\n\nAn error occured generating your room.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        private void ProcessLuckyRoom(Player player, ref int roomsPassed)
        {
            int luckyevent = _random.Next(1, 4);
            switch (luckyevent)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nYou found a health potion!" +
                        "\nWould you like to use it now? (y/n)");
                    Console.ForegroundColor = ConsoleColor.White;
                    string choice = Console.ReadLine().ToLower();
                    if (choice == "y")
                    {
                        int healamount = _random.Next(10, 21);
                        player.Health += healamount;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You used a health potion and gained {healamount} health!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Health potion added to inventory.");
                        Console.ForegroundColor = ConsoleColor.White;
                        player.PickUpItem("Health Potion");
                    }
                    break;

                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nYou found a kebab! You restore ALL your health.");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Health = 100;
                    break;

                case 3:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You discover a hidden shortcut! You advance two rooms.");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed ++;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\n\nAn error occured generating the lucky room.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        private void ProcessUnluckyRoom(Player player, ref int roomsPassed, ref int forcedDirectionCounter, ref string forcedDirection)
        {
            int unluckyEvent = _random.Next(1, 4);
            switch (unluckyEvent)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cursed boots bind on to your feet!");

                    forcedDirectionCounter = 3;

                    int direction = _random.Next(1, 4);  //1 forward, 2 left, 3 right,
                    if (direction == 1)
                    {
                        forcedDirection = "forward";
                    }
                    else if (direction == 2)
                    {
                        forcedDirection = "left";
                    }
                    else
                    {
                        forcedDirection = "right";
                    }

                    Console.WriteLine($"You will be forced to go {forcedDirection} for the next 3 turns.");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed++;
                    break;

                case 2:
                    int setback = _random.Next(1, 4);
                    int falldamage = _random.Next(1, 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"You fall through a trapdoor! You are set back {setback} room(s) and take {falldamage} fall damage.");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed -= setback;
                    if (roomsPassed - setback < -1)
                    {
                        roomsPassed = 0; //prevent negative rooms
                    }
                    player.Health -= falldamage;

                    break;

                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You are poisoned! You lose 10 health.");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Health -= 10;
                    roomsPassed++;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\n\nAn error occured generating the unlucky room.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        private void ProcessMysteryRoom(Player player, ref int roomsPassed, ref int forcedDirectionCounter, ref string forcedDirection)
        {
            int mysteryEvent = _random.Next(1, 4);
            switch (mysteryEvent)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("You found a mysterious loom! It advances you forward.");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed += _random.Next(1, 4);
                    break;

                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("You stumble across a mysterious horn! It summons a mystical monster!");
                    Console.ForegroundColor = ConsoleColor.White;
                    int monsterDamage = _random.Next(10, 31); //  damage
                    player.Health -= monsterDamage;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The monster deals {monsterDamage} damage!");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed++;
                    break;

                case 3:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("You find a mysterious potion. You feel compelled to drink it!");
                    Console.ForegroundColor = ConsoleColor.White;
                    int potionEffect = _random.Next(1, 4);  // 1 = good, 2 = bad, 3 = forced direction
                    if (potionEffect == 1)
                    {
                        int heal = _random.Next(5, 16);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The potion heals you for {heal} health");
                        Console.ForegroundColor= ConsoleColor.White;
                        if (player.Health + heal > 100)
                        {
                            player.Health = 100;
                        }
                        else
                        {
                            player.Health += heal;
                        }
                        roomsPassed++;
                    }
                    else if (potionEffect == 2)
                    {
                        int damage = _random.Next(5, 16);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The potion damages you for {damage} health");
                        Console.ForegroundColor = ConsoleColor.White;
                        player.Health -= damage;
                        roomsPassed++;
                    }
                    else
                    {
                        forcedDirectionCounter = _random.Next(1, 4);
                        int direction = _random.Next(1, 4); //1 forward, 2 left, 3 right
                        if (direction == 1)
                        {
                            forcedDirection = "forward";
                        }
                        else if (direction == 2)
                        {
                            forcedDirection = "left";
                        }
                        else
                        {
                            forcedDirection = "right";
                        }
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"The potion hynotises you" +
                            $" in a trance you to go {forcedDirection} for the next {forcedDirectionCounter} turns.");
                        Console.ForegroundColor = ConsoleColor.White;
                        roomsPassed++;
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\n\nAn error occured generating the mystery room.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

            }
        }

        public void GenerateRoomDescription()
        {
            List<string> descriptions = new List<string>();

            switch (Type)
            {
                case Pass:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    descriptions.Add("The room is dimly lit and appears to be empty. A sense of quiet fills the air.");
                    descriptions.Add("You enter a large, empty chamber. Dust glimmers in the faint light.");
                    descriptions.Add("This room is unremarkable, with plain stone walls and a musty smell.");
                    break;
                case SlightDamage:
                    Console.ForegroundColor = ConsoleColor.Red;
                    descriptions.Add("As you step into the room, you trigger a hidden pressure plate. A dart flies from the wall!");
                    descriptions.Add("You brush against a tripwire, and a net falls from the ceiling, briefly entangling you.");
                    descriptions.Add("The floor creaks ominously under your feet, and a small section collapses, causing you to stumble.");
                    break;
                case HeavyDamage:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    descriptions.Add("A fearsome-looking monster with glowing red eyes lunges at you from the shadows!");
                    descriptions.Add("You hear a growl, and a large beast with sharp claws emerges from a dark corner.");
                    descriptions.Add("As you enter, the door slams shut behind you, and a monstrous figure appears before you!");
                    break;
                case Lucky:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    descriptions.Add("Sunlight streams into this room from a crack in the ceiling, illuminating a small corner.");
                    descriptions.Add("This room is surprisingly clean and well-kept. A gentle breeze flows through.");
                    descriptions.Add("You feel a sense of calm and good fortune as you enter this room.");
                    break;
                case Unlucky:
                    Console.ForegroundColor = ConsoleColor.Red;
                    descriptions.Add("A chilling wind blows through this room, and you feel a sense of dread.");
                    descriptions.Add("The air in this room is heavy and thick with an unnatural scent, making it difficult to breathe.");
                    descriptions.Add("You hear unsettling whispers coming from the walls of this room.");
                    break;
                case Mystery:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    descriptions.Add("This room is filled with strange symbols and mystical artifacts. You feel a magical presence.");
                    descriptions.Add("The walls of this room are covered in shimmering, otherworldly patterns.");
                    descriptions.Add("You enter a room that seems to defy the laws of physics, with objects floating in mid-air.");
                    break;
                default:
                    Console.WriteLine("An error occured genereating your room.");
                    break;
            }

            int i = _random.Next(descriptions.Count);
            Console.WriteLine("\n");
            Console.WriteLine(descriptions[i]);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
    }
}