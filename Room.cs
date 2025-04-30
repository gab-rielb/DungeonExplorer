namespace DungeonExplorer
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DungeonExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Room" />
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the MonstersInRoom
        /// </summary>
        public List<Monster> MonstersInRoom { get; private set; }

        /// <summary>
        /// Gets the ItemsInRoom
        /// </summary>
        public List<Item> ItemsInRoom { get; private set; }

        /// <summary>
        /// Gets the Exits
        /// </summary>
        public Dictionary<string, Room> Exits { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsExitRoom
        /// </summary>
        public bool IsExitRoom { get; set; } = false;

        /// <summary>
        /// Gets the Coordinates
        /// </summary>
        public Point Coordinates { get; private set; }

        /// <summary>
        /// Gets or sets the GeneratedType
        /// </summary>
        public string GeneratedType { get; set; }

        /// <summary>
        /// Gets the GenerationLevel
        /// </summary>
        public int GenerationLevel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPuzzleActive
        /// </summary>
        public bool IsPuzzleActive { get; set; } = false;

        /// <summary>
        /// Gets the PuzzleKeyId
        /// </summary>
        public int PuzzleKeyId { get; private set; } = -1;

        /// <summary>
        /// Defines the _puzzleFailed
        /// </summary>
        private bool _puzzleFailed = false;

        /// <summary>
        /// Defines the _puzzleSolved
        /// </summary>
        private bool _puzzleSolved = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsLocked
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Gets or sets the RequiredKeyId
        /// </summary>
        public int RequiredKeyId { get; set; } = -1;

        /// <summary>
        /// Defines the _random
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Initialises a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="description">The description<see cref="string"/></param>
        /// <param name="coordinates">The coordinates<see cref="Point"/></param>
        /// <param name="generatedType">The generatedType<see cref="string"/></param>
        /// <param name="generationLevel">The generationLevel<see cref="int"/></param>
        public Room(string description, Point coordinates, string generatedType, int generationLevel)
        {
            Coordinates = coordinates;
            GeneratedType = generatedType;
            GenerationLevel = generationLevel;
            Description = description ?? GetDefaultDescription(generatedType);
            MonstersInRoom = new List<Monster>();
            ItemsInRoom = new List<Item>();
            Exits = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);

            if (GeneratedType == RoomType.Puzzle) // Check if the generated room is a puzzle
            {
                IsPuzzleActive = true;
                _puzzleFailed = false;
                _puzzleSolved = false;
                PuzzleKeyId = Math.Abs(coordinates.X * 19 + coordinates.Y * 29 + generationLevel * 13);
                Description += $" A strange mechanism (ID: {PuzzleKeyId}) blocks the path ahead.";
            }
            if (string.IsNullOrEmpty(type))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                throw new ArgumentException("\nRoom type cannot be null or empty");
            }
            Type = type;
            Debug.Assert(!string.IsNullOrWhiteSpace(Type), "Room type was not initialized.");
        }

        /// <summary>
        /// The GetDefaultDescription
        /// </summary>
        /// <param name="type">The type<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetDefaultDescription(string type)
        {
            switch (type)
            {
                case RoomType.Start: return "You stand at the entrance. The air is heavy with anticipation. The only way is forward.";
                case RoomType.Safe: return "This area seems quiet. A brief respite on the main path, or perhaps the start of a side venture?";
                case RoomType.Puzzle: return "Intricate symbols cover the walls.";
                case RoomType.Monster: return "The metallic tang of blood hangs in the air. You are not alone.";
                case RoomType.Exit: return "The air crackles with immense power. The final challenge awaits!";
                default: return "An unremarkable stone chamber.";
            }
        }

        /// <summary>
        /// The CompletePuzzle
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        public void CompletePuzzle(Player player) // Completes the puzzle and rewards the player
        {
            if (!IsPuzzleActive || _puzzleFailed || _puzzleSolved) return; // Check if the puzzle is active and not already solved or failed

            IsPuzzleActive = false; // Mark the puzzle as inactive
            _puzzleSolved = true; // Mark the puzzle as solved
            Description = GetDefaultDescription(GeneratedType) + " The mechanism hums softly and retracts, its purpose fulfilled.";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nThe mechanism yields! The path forward in the branch is clear.");

            Item keyReward = new Key($"Branch Key [{PuzzleKeyId}]", $"Unlocks a room linked to puzzle {PuzzleKeyId}.", PuzzleKeyId); // Create a new key item
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"A hidden compartment opens, revealing: {keyReward.Name}! It clatters onto the floor.");
            AddItem(keyReward); // Add the key to the room

            Console.ForegroundColor = ConsoleColor.White;

            if (_random.Next(100) < 25) // 25% chance to find a bonus item
            {
                Item bonusItem = GameMap.GenerateRandomHealingItem(); // Create a random healing item
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"You also find a {bonusItem.Name} nearby.");
                Console.ForegroundColor = ConsoleColor.White;
                AddItem(bonusItem); // Add the bonus item to the room
            }
        }

        /// <summary>
        /// The FailPuzzle
        /// </summary>
        public void FailPuzzle() // Fails the puzzle and locks the room
        {
            if (!IsPuzzleActive || _puzzleFailed || _puzzleSolved) return; // Check if the puzzle is active and not already solved or failed

            _puzzleFailed = true; // Mark the puzzle as failed
            Description = GetDefaultDescription(GeneratedType) + $" The mechanism (ID: {PuzzleKeyId}) buzzes angrily and remains sealed.";
        }
                case 3:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You discover a hidden shortcut! You advance two rooms.");
                    Console.ForegroundColor = ConsoleColor.White;
                    roomsPassed++; // Advance an extra room
                    break;

        /// <summary>
        /// The ResetPuzzleFailFlag
        /// </summary>
        public void ResetPuzzleFailFlag() // Resets the puzzle fail flag
        {
            if (_puzzleFailed) // Check if the puzzle has failed
            {
                _puzzleFailed = false; // Reset the fail flag
                if (IsPuzzleActive && !_puzzleSolved) // Checks if puzzle is active and isn't solved
                {
                    Description = GetDefaultDescription(GeneratedType) + $" A strange mechanism (ID: {PuzzleKeyId}) blocks the path ahead.";
                }
            }
        }

        /// <summary>
        /// The TurnSafe
        /// </summary>
        public void TurnSafe() // Turns the room safe
        {
            if (GeneratedType == RoomType.Monster && !IsExitRoom) // Check if the room is a monster room and not an exit
            {
                MonstersInRoom.RemoveAll(m => !m.IsAlive); // Remove all dead monsters
                if (!MonstersInRoom.Any(m => m.IsAlive)) // Check if there are any alive monsters
                {
                    Description = "The defeated creature lies still. The immediate danger has passed.";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The room is now clear.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        /// <summary>
        /// The AddMonster
        /// </summary>
        /// <param name="monster">The monster<see cref="Monster"/></param>
        public void AddMonster(Monster monster) // Adds a monster to the room
        {
            if (monster != null) MonstersInRoom.Add(monster); // Check if the monster is not null
        }

        /// <summary>
        /// The RemoveMonster
        /// </summary>
        /// <param name="monster">The monster<see cref="Monster"/></param>
        public void RemoveMonster(Monster monster) // Removes a monster from the room
        {
            if (monster != null) MonstersInRoom.Remove(monster); // Check if the monster is not null
        }
        /// <summary>
        /// The AddItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        public void AddItem(Item item) // Adds an item to the room
        {
            if (item != null) ItemsInRoom.Add(item); // Check if the item is not null
        }

        /// <summary>
        /// The RemoveItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        public void RemoveItem(Item item) // Removes an item from the room
        {
            if (item != null) ItemsInRoom.Remove(item); // Check if the item is not null
        }
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
                        Console.ForegroundColor = ConsoleColor.White;
                        if (player.Health + heal > 100)
                        {
                            player.Health = 100; // Cap health at 100
                        }
                        else
                        {
                            player.Health += heal;
                        }
                        roomsPassed++;
                    }
                    else if (potionEffect == 2)
                    {
                        int damage = _random.Next(5, 16); // Random damage between 5 and 15
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The potion damages you for {damage} health");
                        Console.ForegroundColor = ConsoleColor.White;
                        player.Health -= damage;
                        roomsPassed++;
                    }
                    else
                    {
                        forcedDirectionCounter = _random.Next(1, 4); // Random forced direction between 1 and 3 turns
                        int direction = _random.Next(1, 4); // 1 forward, 2 left, 3 right
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

        /// <summary>
        /// The AddExit
        /// </summary>
        /// <param name="direction">The direction<see cref="string"/></param>
        /// <param name="connectedRoom">The connectedRoom<see cref="Room"/></param>
        public void AddExit(string direction, Room connectedRoom) // Adds an exit to the room
        {
            if (connectedRoom != null && !string.IsNullOrWhiteSpace(direction)) // Check if the connected room is not null and the direction is not null or empty
            {
                Exits[direction] = connectedRoom; // Add the exit to the dictionary
            }
        }

        /// <summary>
        /// The DescribeRoom
        /// </summary>
        public void DescribeRoom() // Describes the room
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- Location: {Coordinates} [Depth: {GenerationLevel}, Type: {GeneratedType}] ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Description);

            if (IsLocked) { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"The way into this room requires Key ID: {RequiredKeyId}."); } // Checks if the room is locked

            Console.ForegroundColor = ConsoleColor.White;

            if (ItemsInRoom.Any()) // Check if there are any items in the room
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("You see items:");
                var groupedItems = ItemsInRoom.GroupBy(i => i.ToString()).Select(g => new { ItemInfo = g.Key, Count = g.Count() }); // Group items by name and count them
                foreach (var group in groupedItems) { Console.WriteLine($"- {group.ItemInfo}{(group.Count > 1 ? $" (x{group.Count})" : "")}"); } // Display grouped items
                Console.ForegroundColor = ConsoleColor.White;
            }

            var aliveMonsters = MonstersInRoom.Where(m => m.IsAlive).ToList(); // LINQ query to get all alive monsters
            if (aliveMonsters.Any()) // Check if there are any alive monsterss
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("DANGER! Creature present:");
                aliveMonsters.ForEach(m => Console.WriteLine($"- {m.Name} ({m.Health}/{m.MaxHealth} HP)")); // Display alive monsters
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (MonstersInRoom.Count > 0) // Check if there are any monsters in the room
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("The remains of a defeated creature lie here.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            var orderedExits = Exits.Keys
                                    .OrderBy(k => k.ToLowerInvariant() == "forward" ? 0 : k.ToLowerInvariant() == "right" ? 1 : k.ToLowerInvariant() == "back" ? 2 : k.ToLowerInvariant() == "left" ? 3 : 4)
                                    .Select(k => k.ToLowerInvariant())
                                    .ToList(); // LINQ query to order exits
            Console.WriteLine("Exits: " + (orderedExits.Any() ? string.Join(", ", orderedExits) : "None"));
            Console.ForegroundColor = ConsoleColor.White;

            if (IsPuzzleActive) // Check if the puzzle is active
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (_puzzleFailed) // Check if the puzzle has failed
                {
                    Console.WriteLine($"The puzzle mechanism (ID: {PuzzleKeyId}) remains locked, but you can try again! Type 'solve'.");
                }
                else // If the puzzle is not failed
                {
                    Console.WriteLine($"The way forward is blocked by a puzzle (ID: {PuzzleKeyId})! Type 'solve' to attempt it.");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The OnPlayerEnter
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        public void OnPlayerEnter(Player player) // Called when the player enters the room
        {
            if (IsPuzzleActive && _puzzleFailed) { ResetPuzzleFailFlag(); } // Reset the puzzle fail flag if the puzzle is active and failed

            DescribeRoom(); // Describe the room to the player

            var xpCrystals = ItemsInRoom.OfType<MiscItem>().Where(i => i.Name == "Energy Crystal").ToList(); // Get all XP crystals in the room
            if (xpCrystals.Any()) // Check if there are any XP crystals
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var crystal in xpCrystals) // Iterate through each crystal
                {
                    Console.WriteLine($"The {crystal.Name} disintegrates as you absorb its energy!");
                    int xpAmount = (GenerationLevel * 5) + 15 + _random.Next(0, 11); // Calculate XP amount
                    player.AddExperience(xpAmount); // Add XP to the player
                    ItemsInRoom.Remove(crystal); // Remove the crystal from the room
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
