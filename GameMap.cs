namespace DungeonExplorer
{
    using System;
    using System.Collections.Generic;
    using DungeonExplorer.Items;
    using DungeonExplorer.Monsters;

    /// <summary>
    /// Defines the <see cref="Direction" />
    /// </summary>
    public static class Direction
    {
        /// <summary>
        /// Defines the Forward
        /// </summary>
        public const string Forward = "Forward";

        /// <summary>
        /// Defines the Right
        /// </summary>
        public const string Right = "Right";

        /// <summary>
        /// Defines the Back
        /// </summary>
        public const string Back = "Back";

        /// <summary>
        /// Defines the Left
        /// </summary>
        public const string Left = "Left";

        /// <summary>
        /// Defines the Compass
        /// </summary>
        public static readonly string[] Compass = { Forward, Right, Back, Left };

        /// <summary>
        /// The GetOpposite
        /// </summary>
        /// <param name="dir">The dir<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetOpposite(string dir)
        {
            switch (dir?.ToLowerInvariant())
            {
                case "forward": return Back;
                case "right": return Left;
                case "back": return Forward;
                case "left": return Right;
                default: return null;

            }
        }
    }

    /// <summary>
    /// Defines the <see cref="RoomType" />
    /// </summary>
    public static class RoomType
    {
        /// <summary>
        /// Defines the Start
        /// </summary>
        public const string Start = "Start";

        /// <summary>
        /// Defines the Monster
        /// </summary>
        public const string Monster = "Monster";

        /// <summary>
        /// Defines the Puzzle
        /// </summary>
        public const string Puzzle = "Puzzle";

        /// <summary>
        /// Defines the Safe
        /// </summary>
        public const string Safe = "Safe";

        /// <summary>
        /// Defines the Exit
        /// </summary>
        public const string Exit = "Exit";
    }

    /// <summary>
    /// Defines the <see cref="Point" />
    /// </summary>
    public readonly struct Point : IEquatable<Point>
    {
        /// <summary>
        /// Gets the X
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref=""/> class.
        /// </summary>
        /// <param name="x">The x<see cref="int"/></param>
        /// <param name="y">The y<see cref="int"/></param>
        public Point(int x, int y)
        {
            X = x; Y = y;
        }

        /// <summary>
        /// The Equals
        /// </summary>
        /// <param name="other">The other<see cref="Point"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        /// <summary>
        /// The Equals
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool Equals(object obj) => obj is Point other && Equals(other);

        /// <summary>
        /// The GetHashCode
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int GetHashCode() // Use unchecked to avoid overflow exceptions
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;

            }
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString() => $"({X}, {Y})"; // Format as needed

        public static bool operator ==(Point left, Point right) => left.Equals(right); // Check for equality

        public static bool operator !=(Point left, Point right) => !(left == right); // Check for inequality
    }

    /// <summary>
    /// Defines the <see cref="GameMap" />
    /// </summary>
    public class GameMap
    {
        /// <summary>
        /// Defines the _rooms
        /// </summary>
        private Dictionary<Point, Room> _rooms;

        /// <summary>
        /// Defines the _random
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Gets the StartRoom
        /// </summary>
        public Room StartRoom { get; private set; }

        /// <summary>
        /// Defines the _difficultyModifier
        /// </summary>
        private int _difficultyModifier = 0;

        /// <summary>
        /// Initialises a new instance of the <see cref="GameMap"/> class.
        /// </summary>
        public GameMap()
        {
            _rooms = new Dictionary<Point, Room>();
        }

        /// <summary>
        /// The GetRoom
        /// </summary>
        /// <param name="coordinates">The coordinates<see cref="Point"/></param>
        /// <returns>The <see cref="Room"/></returns>
        public Room GetRoom(Point coordinates)
        {
            _rooms.TryGetValue(coordinates, out Room room); // Try to get the room at the specified coordinates
            return room;
        }

        /// <summary>
        /// The GetRoom
        /// </summary>
        /// <param name="x">The x<see cref="int"/></param>
        /// <param name="y">The y<see cref="int"/></param>
        /// <returns>The <see cref="Room"/></returns>
        public Room GetRoom(int x, int y) => GetRoom(new Point(x, y)); // Get the room at the specified x and y coordinates

        /// <summary>
        /// The GenerateMap
        /// </summary>
        /// <param name="difficultyChoice">The difficultyChoice<see cref="int"/></param>
        public void GenerateMap(int difficultyChoice) // Generate a map based on the difficulty choice
        {
            _rooms.Clear(); // Clear the existing rooms
            StartRoom = null; // Reset the start room
            _difficultyModifier = Math.Max(0, difficultyChoice - 1); // Set the difficulty modifier
            int mainPathLength; // Set the main path length based on the difficulty choice

            switch (difficultyChoice)
            {
                case 1: mainPathLength = 10; break;
                case 3: mainPathLength = 20; break;
                case 2: default: mainPathLength = 15; break;
            }

            Point currentCoords = new Point(0, 0); // Create the starting coordinates

            StartRoom = new Room(null, currentCoords, RoomType.Start, 0); // Create the start room
            _rooms.Add(currentCoords, StartRoom); // Add the start room to the dictionary
            PopulateRoomContent(StartRoom); // Populate the start room content
            Room previousRoom = StartRoom; // Set the previous room to the start room

            for (int y = 1; y < mainPathLength; y++) // Generate the main path
            {
                currentCoords = new Point(0, y); // Create the current coordinates
                Room mainPathRoom = new Room(null, currentCoords, RoomType.Safe, y); // Create the main path room
                _rooms.Add(currentCoords, mainPathRoom); // Add the main path room to the dictionary
                LinkRooms(previousRoom, mainPathRoom, Direction.Forward); // Link the previous room to the main path room
                PopulateRoomContent(mainPathRoom); // Populate the main path room content
                previousRoom = mainPathRoom; // Set the previous room to the main path room
            }

            for (int y = 1; y < mainPathLength - 1; y++) // Generate branches
            {
                if (_random.Next(100) < 50) // 50% chance for a room to generate a branch
                {
                    Point mainPathNode = new Point(0, y); // Create the main path node
                    if (!_rooms.TryGetValue(mainPathNode, out Room mainPathRoom)) continue; // Get the main path room
                    int xDirection = (_random.Next(2) == 0) ? -1 : 1; // Randomly choose a direction
                    string branchDirection = (xDirection == 1) ? Direction.Right : Direction.Left; // Choose the branch direction
                    Point firstBranchCoord = new Point(mainPathNode.X + xDirection, y); // Create the first branch coordinates

                    if (!_rooms.ContainsKey(firstBranchCoord)) // Check if the first branch coordinates already exist
                    {
                        GenerateSpecificBranch(mainPathRoom, firstBranchCoord, branchDirection, y); // Generate the specific branch
                    }
                }
            }

            Point finalRoomCoords = new Point(0, mainPathLength - 1); // Create the final room coordinates

            if (_rooms.TryGetValue(finalRoomCoords, out Room finalRoom)) // Check if the final room coordinates exist
            {
                finalRoom.GeneratedType = RoomType.Exit; // Set the final room type to Exit
                finalRoom.IsExitRoom = true; // Set the final room as the exit room
                finalRoom.Description = "A menacing aura fills this vast chamber. Bones litter the floor. The final challenge awaits! A huge DRAGON glares at you!";
                finalRoom.ItemsInRoom.Clear(); // Clear the items in the final room
                finalRoom.MonstersInRoom.Clear(); // Clear the monsters in the final room
                finalRoom.IsPuzzleActive = false; // Set the puzzle active state to false
                finalRoom.IsLocked = false; // Set the locked state to false
                finalRoom.AddMonster(new Dragon()); // Add a dragon to the final room
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"DEBUG: Placed Exit/Dragon at {finalRoom.Coordinates}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else // Check if the final room coordinates do not exist
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CRITICAL ERROR: Could not find final room at {finalRoomCoords} to place Dragon!");
                Console.ForegroundColor = ConsoleColor.White;
                if (previousRoom != StartRoom) previousRoom.IsExitRoom = true;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Map generated with {_rooms.Count} rooms. Main Path Length: {mainPathLength}.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The GenerateSpecificBranch
        /// </summary>
        /// <param name="mainPathRoom">The mainPathRoom<see cref="Room"/></param>
        /// <param name="startCoord">The startCoord<see cref="Point"/></param>
        /// <param name="branchDir">The branchDir<see cref="string"/></param>
        /// <param name="depth">The depth<see cref="int"/></param>
        private void GenerateSpecificBranch(Room mainPathRoom, Point startCoord, string branchDir, int depth) // Generate a specific branch
        {
            Point currentBranchCoord = startCoord; // Create the current branch coordinates
            Room previousBranchRoom = mainPathRoom; // Set the previous branch room to the main path room
            int xDirection = (branchDir == Direction.Right) ? 1 : -1; // Randomly choose a direction
            Room branchRoom1 = new Room($"A small side chamber branching {branchDir.ToLower()}.", currentBranchCoord, RoomType.Safe, depth); // Create the first branch room
            _rooms.Add(currentBranchCoord, branchRoom1); // Add the first branch room to the dictionary

            LinkRooms(previousBranchRoom, branchRoom1, branchDir); // Link the previous room to the first branch room
            PopulateRoomContent(branchRoom1); // Populate the first branch room content
            previousBranchRoom = branchRoom1; // Set the previous branch room to the first branch room
            currentBranchCoord = new Point(currentBranchCoord.X + xDirection, depth); // Create the current branch coordinates

            if (!_rooms.ContainsKey(currentBranchCoord)) // Check if the current branch coordinates already exist
            {
                Room branchRoom2 = new Room(null, currentBranchCoord, RoomType.Puzzle, depth); // Create the second branch room
                _rooms.Add(currentBranchCoord, branchRoom2); // Add the second branch room to the dictionary
                LinkRooms(previousBranchRoom, branchRoom2, branchDir); // Link the previous room to the second branch room
                PopulateRoomContent(branchRoom2); // Populate the second branch room content
                previousBranchRoom = branchRoom2; // Set the previous branch room to the second branch room
                currentBranchCoord = new Point(currentBranchCoord.X + xDirection, depth); // Create the current branch coordinates

                if (!_rooms.ContainsKey(currentBranchCoord)) // Check if the current branch coordinates already exist
                {
                    Room branchRoom3 = new Room($"The air grows cold, this dead-end chamber feels dangerous.", currentBranchCoord, RoomType.Monster, depth); // Create the third branch room
                    branchRoom3.IsLocked = true; // Set the locked state to true
                    branchRoom3.RequiredKeyId = branchRoom2.PuzzleKeyId; // Set the required key ID for the third branch room
                    _rooms.Add(currentBranchCoord, branchRoom3); // Add the third branch room to the dictionary
                    LinkRooms(previousBranchRoom, branchRoom3, branchDir); // Link the previous room to the third branch room
                    PopulateRoomContent(branchRoom3); // Populate the third branch room content
                }
            }
        }

        /// <summary>
        /// The LinkRooms
        /// </summary>
        /// <param name="roomA">The roomA<see cref="Room"/></param>
        /// <param name="roomB">The roomB<see cref="Room"/></param>
        /// <param name="directionAToB">The directionAToB<see cref="string"/></param>
        private void LinkRooms(Room roomA, Room roomB, string directionAToB)
        {
            if (roomA == null || roomB == null || string.IsNullOrEmpty(directionAToB)) return; // Check if the rooms and direction are valid

            string directionBToA = Direction.GetOpposite(directionAToB); // Get the opposite direction

            if (directionBToA == null) return; // Check if the opposite direction is valid

            roomA.AddExit(directionAToB, roomB); // Add the exit from room A to room B
            roomB.AddExit(directionBToA, roomA); // Add the exit from room B to room A
        }

        /// <summary>
        /// The PopulateRoomContent
        /// </summary>
        /// <param name="room">The room<see cref="Room"/></param>
        private void PopulateRoomContent(Room room) // Populate the room content
        {
            if (room == null || room.IsExitRoom) return; // Check if the room is valid and not an exit room

            int generation = room.GenerationLevel; // Get the generation level of the room

            switch (room.GeneratedType) // Check the room type
            {
                case RoomType.Start: // Start room
                    if (_random.Next(100) < 85) // 85% chance to add a healing item
                        room.AddItem(new Potion("Small Healing Potion", "Restores 20 health.", 20)); // Add a small healing potion
                    break;

                case RoomType.Safe: // Safe room
                    bool isMainPath = room.Coordinates.X == 0; // Check if the room is on the main path
                    if (isMainPath && generation > 0) // Check if the room is on the main path and has a generation level
                    {
                        if (_random.Next(100) < 30 + generation * 2) // 30% chance to add a healing item
                        {
                            room.AddItem(GenerateRandomHealingItem()); // Add a random healing item
                        }
                        if (_random.Next(100) < 20 + generation) // 20% chance to add a special crystal
                        {
                            room.AddItem(new MiscItem("Energy Crystal", "Humming with latent experience. (Grants XP on entry)"));
                        }
                    }
                    else // Check if the room is not on the main path
                    {
                        if (_random.Next(100) < 40) //40% chance to add a healing item
                        {
                            room.AddItem(GenerateRandomHealingItem()); // Add a random healing item
                        }
                    }
                    break;

                case RoomType.Puzzle: // Puzzle room
                    if (_random.Next(100) < 15) // 15% chance to add a healing item
                        room.AddItem(new Food("Trail Rations", "Simple sustenance.", 10)); // Add trail rations
                    break;

                case RoomType.Monster: // Monster room
                    Monster monster = GenerateRawMonster(generation); // Generate a raw monster
                    double healthMultiplier = 1.0 + (_difficultyModifier * 0.10); // Calculate the health multiplier
                    int baseHealth = monster.MaxHealth; // Get the base health of the monster
                    int boostedHealth = (int)Math.Ceiling(baseHealth * healthMultiplier); // Calculate the boosted health
                    monster.ApplyHealthBoost(boostedHealth); // Apply the health boost to the monster
                    room.AddMonster(monster); // Add the monster to the room
                    break;

            }
        }

        /// <summary>
        /// The GenerateRandomHealingItem
        /// </summary>
        /// <returns>The <see cref="Item"/></returns>
        public static Item GenerateRandomHealingItem() // Generate a random healing item
        {
            int roll = _random.Next(100); // Generate a random roll

            if (roll < 50) // 50% chance to add a food item
                return new Food("Stale Biscuit", "Dry and crumbly, but edible. Restores 10 health.", 10); // Add a stale biscuit
            else if (roll < 85) // 35% chance to add a potion
                return new Potion("Healing Potion", "Restores 30 health.", 30); // Add a healing potion
            else // 15% chance to add a special item
            {
                if (_random.Next(2) == 0) // 50% chance to add a food item
                    return new Food("Hearty Stew", "Restores 60 health.", 60); // Add a hearty stew
                else // 50% chance to add a potion
                    return new Potion("Greater Healing Potion", "Restores 75 health.", 75); // Add a greater healing potion
            }
        }

        /// <summary>
        /// The GenerateRawMonster
        /// </summary>
        /// <param name="generation">The generation<see cref="int"/></param>
        /// <returns>The <see cref="Monster"/></returns>
        private Monster GenerateRawMonster(int generation) // Generate a raw monster
        {
            int tier = Math.Max(0, generation / 5); // Calculate the tier based on the generation

            int roll = _random.Next(100); // Generate a random roll

            if (tier <= 1) // Check if the tier is less than or equal to 1
            {
                if (roll < 70) return new Goblin(); // 70% chance to add a goblin
                else return new Ogre(); // 30% chance to add an ogre
            }
            else // Check if the tier is greater than 1
            {
                if (roll < 40) return new Goblin(); // 40% chance to add a goblin
                else return new Ogre(); // 60% chance to add an ogre
            }
        }
    }

}
