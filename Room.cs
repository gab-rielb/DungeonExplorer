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
        /// Initializes a new instance of the <see cref="Room"/> class.
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

            if (GeneratedType == RoomType.Puzzle)
            {
                IsPuzzleActive = true;
                _puzzleFailed = false;
                _puzzleSolved = false;
                PuzzleKeyId = Math.Abs(coordinates.X * 19 + coordinates.Y * 29 + generationLevel * 13);
                Description += $" A strange mechanism (ID: {PuzzleKeyId}) blocks the path ahead.";
            }
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
        public void CompletePuzzle(Player player)
        {
            if (!IsPuzzleActive || _puzzleFailed || _puzzleSolved) return;

            IsPuzzleActive = false;
            _puzzleSolved = true;
            Description = GetDefaultDescription(GeneratedType) + " The mechanism hums softly and retracts, its purpose fulfilled.";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nThe mechanism yields! The path forward in the branch is clear.");

            Item keyReward = new Key($"Branch Key [{PuzzleKeyId}]", $"Unlocks a room linked to puzzle {PuzzleKeyId}.", PuzzleKeyId);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"A hidden compartment opens, revealing: {keyReward.Name}! It clatters onto the floor.");
            AddItem(keyReward);

            Console.ForegroundColor = ConsoleColor.White;

            if (_random.Next(100) < 25)
            {
                Item bonusItem = GameMap.GenerateRandomHealingItem();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"You also find a {bonusItem.Name} nearby.");
                Console.ForegroundColor = ConsoleColor.White;
                AddItem(bonusItem);
            }
        }

        /// <summary>
        /// The FailPuzzle
        /// </summary>
        public void FailPuzzle()
        {
            if (!IsPuzzleActive || _puzzleFailed || _puzzleSolved) return;

            _puzzleFailed = true;
            Description = GetDefaultDescription(GeneratedType) + $" The mechanism (ID: {PuzzleKeyId}) buzzes angrily and remains sealed.";
        }

        /// <summary>
        /// The ResetPuzzleFailFlag
        /// </summary>
        public void ResetPuzzleFailFlag()
        {
            if (_puzzleFailed)
            {
                _puzzleFailed = false;
                if (IsPuzzleActive && !_puzzleSolved)
                {
                    Description = GetDefaultDescription(GeneratedType) + $" A strange mechanism (ID: {PuzzleKeyId}) blocks the path ahead.";
                }
            }
        }

        /// <summary>
        /// The TurnSafe
        /// </summary>
        public void TurnSafe()
        {
            if (GeneratedType == RoomType.Monster && !IsExitRoom)
            {
                MonstersInRoom.RemoveAll(m => !m.IsAlive);
                if (!MonstersInRoom.Any(m => m.IsAlive))
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
        public void AddMonster(Monster monster)
        {
            if (monster != null) MonstersInRoom.Add(monster);
        }

        /// <summary>
        /// The RemoveMonster
        /// </summary>
        /// <param name="monster">The monster<see cref="Monster"/></param>
        public void RemoveMonster(Monster monster)
        {
            if (monster != null) MonstersInRoom.Remove(monster);
        }

        /// <summary>
        /// The AddItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        public void AddItem(Item item)
        {
            if (item != null) ItemsInRoom.Add(item);
        }

        /// <summary>
        /// The RemoveItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        public void RemoveItem(Item item)
        {
            if (item != null) ItemsInRoom.Remove(item);
        }

        /// <summary>
        /// The AddExit
        /// </summary>
        /// <param name="direction">The direction<see cref="string"/></param>
        /// <param name="connectedRoom">The connectedRoom<see cref="Room"/></param>
        public void AddExit(string direction, Room connectedRoom)
        {
            if (connectedRoom != null && !string.IsNullOrWhiteSpace(direction))
            {
                Exits[direction] = connectedRoom;
            }
        }

        /// <summary>
        /// The DescribeRoom
        /// </summary>
        public void DescribeRoom()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- Location: {Coordinates} [Depth: {GenerationLevel}, Type: {GeneratedType}] ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Description);

            if (IsLocked) { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"The way into this room requires Key ID: {RequiredKeyId}."); }

            Console.ForegroundColor = ConsoleColor.White;

            if (ItemsInRoom.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("You see items:");
                var groupedItems = ItemsInRoom.GroupBy(i => i.ToString()).Select(g => new { ItemInfo = g.Key, Count = g.Count() });
                foreach (var group in groupedItems) { Console.WriteLine($"- {group.ItemInfo}{(group.Count > 1 ? $" (x{group.Count})" : "")}"); }
                Console.ForegroundColor = ConsoleColor.White;
            }

            var aliveMonsters = MonstersInRoom.Where(m => m.IsAlive).ToList();
            if (aliveMonsters.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("DANGER! Creature present:");
                aliveMonsters.ForEach(m => Console.WriteLine($"- {m.Name} ({m.Health}/{m.MaxHealth} HP)"));
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (MonstersInRoom.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("The remains of a defeated creature lie here.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            var orderedExits = Exits.Keys
                                    .OrderBy(k => k.ToLowerInvariant() == "forward" ? 0 : k.ToLowerInvariant() == "right" ? 1 : k.ToLowerInvariant() == "back" ? 2 : k.ToLowerInvariant() == "left" ? 3 : 4)
                                    .Select(k => k.ToLowerInvariant())
                                    .ToList();
            Console.WriteLine("Exits: " + (orderedExits.Any() ? string.Join(", ", orderedExits) : "None"));
            Console.ForegroundColor = ConsoleColor.White;

            if (IsPuzzleActive)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (_puzzleFailed)
                {
                    Console.WriteLine($"The puzzle mechanism (ID: {PuzzleKeyId}) remains locked, but you can try again! Type 'solve'.");
                }
                else
                {
                    Console.WriteLine($"The way forward is blocked by a puzzle (ID: {PuzzleKeyId})! Type 'solve' to attempt it.");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (_puzzleSolved)
            {
            }
        }

        /// <summary>
        /// The OnPlayerEnter
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        public void OnPlayerEnter(Player player)
        {
            if (IsPuzzleActive && _puzzleFailed) { ResetPuzzleFailFlag(); }

            DescribeRoom();

            var xpCrystals = ItemsInRoom.OfType<MiscItem>().Where(i => i.Name == "Energy Crystal").ToList();
            if (xpCrystals.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var crystal in xpCrystals)
                {
                    Console.WriteLine($"The {crystal.Name} disintegrates as you absorb its energy!");
                    int xpAmount = (GenerationLevel * 5) + 15 + _random.Next(0, 11);
                    player.AddExperience(xpAmount);
                    ItemsInRoom.Remove(crystal);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
