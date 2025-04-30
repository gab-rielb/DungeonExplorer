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
        /// Initializes a new instance of the <see cref=""/> class.
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
        public override int GetHashCode()
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
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(Point left, Point right) => left.Equals(right);

        public static bool operator !=(Point left, Point right) => !(left == right);
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
        /// Initializes a new instance of the <see cref="GameMap"/> class.
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

            _rooms.TryGetValue(coordinates, out Room room);

            return room;
        }

        /// <summary>
        /// The GetRoom
        /// </summary>
        /// <param name="x">The x<see cref="int"/></param>
        /// <param name="y">The y<see cref="int"/></param>
        /// <returns>The <see cref="Room"/></returns>
        public Room GetRoom(int x, int y) => GetRoom(new Point(x, y));

        /// <summary>
        /// The GenerateMap
        /// </summary>
        /// <param name="difficultyChoice">The difficultyChoice<see cref="int"/></param>
        public void GenerateMap(int difficultyChoice)
        {

            _rooms.Clear();

            StartRoom = null;

            _difficultyModifier = Math.Max(0, difficultyChoice - 1);

            int mainPathLength;

            switch (difficultyChoice)

            {

                case 1: mainPathLength = 10; break;

                case 3: mainPathLength = 20; break;

                case 2: default: mainPathLength = 15; break;

            }

            Point currentCoords = new Point(0, 0);

            StartRoom = new Room(null, currentCoords, RoomType.Start, 0);

            _rooms.Add(currentCoords, StartRoom);

            PopulateRoomContent(StartRoom);

            Room previousRoom = StartRoom;

            for (int y = 1; y < mainPathLength; y++)

            {

                currentCoords = new Point(0, y);

                Room mainPathRoom = new Room(null, currentCoords, RoomType.Safe, y);

                _rooms.Add(currentCoords, mainPathRoom);

                LinkRooms(previousRoom, mainPathRoom, Direction.Forward);

                PopulateRoomContent(mainPathRoom);

                previousRoom = mainPathRoom;

            }

            for (int y = 1; y < mainPathLength - 1; y++)

            {

                if (_random.Next(100) < 50)

                {

                    Point mainPathNode = new Point(0, y);

                    if (!_rooms.TryGetValue(mainPathNode, out Room mainPathRoom)) continue;

                    int xDirection = (_random.Next(2) == 0) ? -1 : 1;

                    string branchDirection = (xDirection == 1) ? Direction.Right : Direction.Left;

                    Point firstBranchCoord = new Point(mainPathNode.X + xDirection, y);

                    if (!_rooms.ContainsKey(firstBranchCoord))

                    {

                        GenerateSpecificBranch(mainPathRoom, firstBranchCoord, branchDirection, y);

                    }

                }

            }

            Point finalRoomCoords = new Point(0, mainPathLength - 1);

            if (_rooms.TryGetValue(finalRoomCoords, out Room finalRoom))

            {

                finalRoom.GeneratedType = RoomType.Exit;

                finalRoom.IsExitRoom = true;

                finalRoom.Description = "A menacing aura fills this vast chamber. Bones litter the floor. The final challenge awaits! A huge DRAGON glares at you!";

                finalRoom.ItemsInRoom.Clear();

                finalRoom.MonstersInRoom.Clear();

                finalRoom.IsPuzzleActive = false;

                finalRoom.IsLocked = false;

                finalRoom.AddMonster(new Dragon());

                Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.WriteLine($"DEBUG: Placed Exit/Dragon at {finalRoom.Coordinates}");

                Console.ForegroundColor = ConsoleColor.White;

            }

            else

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
        private void GenerateSpecificBranch(Room mainPathRoom, Point startCoord, string branchDir, int depth)
        {

            Point currentBranchCoord = startCoord;

            Room previousBranchRoom = mainPathRoom;

            int xDirection = (branchDir == Direction.Right) ? 1 : -1;

            Room branchRoom1 = new Room($"A small side chamber branching {branchDir.ToLower()}.", currentBranchCoord, RoomType.Safe, depth);

            _rooms.Add(currentBranchCoord, branchRoom1);

            LinkRooms(previousBranchRoom, branchRoom1, branchDir);

            PopulateRoomContent(branchRoom1);

            previousBranchRoom = branchRoom1;

            currentBranchCoord = new Point(currentBranchCoord.X + xDirection, depth);

            if (!_rooms.ContainsKey(currentBranchCoord))

            {

                Room branchRoom2 = new Room(null, currentBranchCoord, RoomType.Puzzle, depth);

                _rooms.Add(currentBranchCoord, branchRoom2);

                LinkRooms(previousBranchRoom, branchRoom2, branchDir);

                PopulateRoomContent(branchRoom2);

                previousBranchRoom = branchRoom2;

                currentBranchCoord = new Point(currentBranchCoord.X + xDirection, depth);

                if (!_rooms.ContainsKey(currentBranchCoord))

                {

                    Room branchRoom3 = new Room($"The air grows cold, this dead-end chamber feels dangerous.", currentBranchCoord, RoomType.Monster, depth);

                    branchRoom3.IsLocked = true;

                    branchRoom3.RequiredKeyId = branchRoom2.PuzzleKeyId;

                    _rooms.Add(currentBranchCoord, branchRoom3);

                    LinkRooms(previousBranchRoom, branchRoom3, branchDir);

                    PopulateRoomContent(branchRoom3);

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

            if (roomA == null || roomB == null || string.IsNullOrEmpty(directionAToB)) return;

            string directionBToA = Direction.GetOpposite(directionAToB);

            if (directionBToA == null) return;

            roomA.AddExit(directionAToB, roomB);

            roomB.AddExit(directionBToA, roomA);
        }

        /// <summary>
        /// The PopulateRoomContent
        /// </summary>
        /// <param name="room">The room<see cref="Room"/></param>
        private void PopulateRoomContent(Room room)
        {
            if (room == null || room.IsExitRoom) return;

            int generation = room.GenerationLevel;

            switch (room.GeneratedType)
            {
                case RoomType.Start:
                    if (_random.Next(100) < 85)
                        room.AddItem(new Potion("Small Healing Potion", "Restores 20 health.", 20));
                    break;

                case RoomType.Safe:
                    bool isMainPath = room.Coordinates.X == 0;
                    if (isMainPath && generation > 0)
                    {
                        if (_random.Next(100) < 30 + generation * 2)
                        {
                            room.AddItem(GenerateRandomHealingItem());
                        }
                        if (_random.Next(100) < 20 + generation)
                        {
                            room.AddItem(new MiscItem("Energy Crystal", "Humming with latent experience. (Grants XP on entry)"));
                        }
                    }
                    else
                    {
                        if (_random.Next(100) < 40)
                        {
                            room.AddItem(GenerateRandomHealingItem());
                        }
                    }
                    break;

                case RoomType.Puzzle:
                    if (_random.Next(100) < 15)
                        room.AddItem(new Food("Trail Rations", "Simple sustenance.", 10));
                    break;

                case RoomType.Monster:
                    Monster monster = GenerateRawMonster(generation);
                    double healthMultiplier = 1.0 + (_difficultyModifier * 0.10);
                    int baseHealth = monster.MaxHealth;
                    int boostedHealth = (int)Math.Ceiling(baseHealth * healthMultiplier);
                    monster.ApplyHealthBoost(boostedHealth);
                    room.AddMonster(monster);
                    break;

            }
        }

        /// <summary>
        /// The GenerateRandomHealingItem
        /// </summary>
        /// <returns>The <see cref="Item"/></returns>
        public static Item GenerateRandomHealingItem()
        {
            int roll = _random.Next(100);

            if (roll < 50)
                return new Food("Stale Biscuit", "Dry and crumbly, but edible. Restores 10 health.", 10);
            else if (roll < 85)
                return new Potion("Healing Potion", "Restores 30 health.", 30);
            else
            {
                if (_random.Next(2) == 0)
                    return new Food("Hearty Stew", "Restores 60 health.", 60);
                else
                    return new Potion("Greater Healing Potion", "Restores 75 health.", 75);
            }
        }

        /// <summary>
        /// The GenerateRawMonster
        /// </summary>
        /// <param name="generation">The generation<see cref="int"/></param>
        /// <returns>The <see cref="Monster"/></returns>
        private Monster GenerateRawMonster(int generation)
        {
            int tier = Math.Max(0, generation / 5);

            int roll = _random.Next(100);

            if (tier <= 1)
            {
                if (roll < 70) return new Goblin();
                else return new Ogre();
            }
            else
            {
                if (roll < 40) return new Goblin();
                else return new Ogre();
            }
        }
    }

}
