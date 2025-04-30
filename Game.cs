namespace DungeonExplorer
{
    using System;
    using System.Linq;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Game" />
    /// </summary>
    internal class Game
    {
        /// <summary>
        /// Defines the _player
        /// </summary>
        private Player _player;

        /// <summary>
        /// Defines the _gameMap
        /// </summary>
        private GameMap _gameMap;

        /// <summary>
        /// Defines the _currentRoom
        /// </summary>
        private Room _currentRoom;

        /// <summary>
        /// Defines the _previousRoom
        /// </summary>
        private Room _previousRoom = null;

        /// <summary>
        /// Defines the _gameOver
        /// </summary>
        private bool _gameOver;

        /// <summary>
        /// Defines the _random
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Defines the _playerPosition
        /// </summary>
        private Point _playerPosition;

        /// <summary>
        /// Defines the _difficultyChoice
        /// </summary>
        private int _difficultyChoice = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            _gameMap = new GameMap();
            _gameOver = false;
        }

        /// <summary>
        /// The Start
        /// </summary>
        public void Start()
        {
            try
            {
                DisplayIntro();
                InitialisePlayer();
                if (_player == null) return;

                SelectDifficultyAndGenerateMap();

                if (_gameMap.StartRoom == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CRITICAL ERROR: Map generation failed. Cannot start game.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                _currentRoom = _gameMap.StartRoom;
                _playerPosition = _currentRoom.Coordinates;
                _previousRoom = null;

                DisplayGameInstructions();
                GameLoop();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn unexpected critical error occurred during the game: {ex.Message}");
                Console.WriteLine("--- Stack Trace ---");
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
                _gameOver = true;
            }
            finally
            {
                if (_player != null)
                {
                    DisplayGameResult();
                }
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// The DisplayIntro
        /// </summary>
        private void DisplayIntro()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
***************************************
* *
* Welcome to DUNGEON EXPLORER!        *
* *
***************************************
");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Prepare to delve into the depths...\n");
        }

        /// <summary>
        /// The InitialisePlayer
        /// </summary>
        private void InitialisePlayer()
        {
            string playerName = "";
            Console.WriteLine("Enter your adventurer's name (max 25 characters):");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.White;
                playerName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length <= 25) break;
                else if (string.IsNullOrWhiteSpace(playerName))
                { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Name cannot be empty."); }
                else
                { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Name is too long (max 25 characters)."); }
                Console.ForegroundColor = ConsoleColor.White;
            }

            _player = new Player(playerName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAdventurer '{_player.Name}' ready for action! Inventory capacity: {_player.Inventory.Capacity}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to select difficulty...");
            Console.ReadKey(true);
            Console.Clear();
        }

        /// <summary>
        /// The SelectDifficultyAndGenerateMap
        /// </summary>
        private void SelectDifficultyAndGenerateMap()
        {
            _difficultyChoice = 2;

            Console.WriteLine("\nSelect Difficulty:");
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("1. Easy (Fewer Rooms, Weaker Foes)");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("2. Normal (Standard Rooms & Foes)");
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("3. Hard (More Rooms, Tougher Foes)");
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter your choice (1-3): ");
                string choiceStr = Console.ReadLine()?.Trim();
                switch (choiceStr)
                {
                    case "1": _difficultyChoice = 1; Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Difficulty set to Easy. Generating map..."); goto Generate;
                    case "3": _difficultyChoice = 3; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Difficulty set to Hard. Generating map..."); goto Generate;
                    case "2": _difficultyChoice = 2; Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Difficulty set to Normal. Generating map..."); goto Generate;
                    default: Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Invalid choice. Please enter 1, 2, or 3."); break;
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

        Generate:
            Console.ForegroundColor = ConsoleColor.White;
            _gameMap.GenerateMap(_difficultyChoice);
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Map generation complete.");
            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Press any key to enter the dungeon...");
            Console.ReadKey(true);
            Console.Clear();
        }

        /// <summary>
        /// The DisplayGameInstructions
        /// </summary>
        private void DisplayGameInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nObjective: Navigate the dungeon, overcome challenges, and defeat the final Dragon!");
            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine("  Movement: 'forward'/'f', 'back'/'b', 'left'/'l', 'right'/'r'");
            Console.WriteLine("  Actions:  'look'                             - Describe the current room.");
            Console.WriteLine("            'pickup [item]'/'take [item]' - Pick up an item from the room.");
            Console.WriteLine("            'use [consumable]'             - Use a Potion or Food from inventory.");
            Console.WriteLine("            'equip [weapon]'/'eq [w]'      - Equip a weapon by name from inventory.");
            Console.WriteLine("            'equip strongest'/'eq s'       - Equip highest damage weapon from inventory.");
            Console.WriteLine("            'equip fists'/'eq none'        - Unequip current weapon.");
            Console.WriteLine("            'attack'/'a'                   - Attack the monster in the room.");
            Console.WriteLine("            'solve'                        - Attempt to solve a puzzle in the room.");
            Console.WriteLine("  Status:   'inventory'/'inv'/'i'          - View your inventory.");
            Console.WriteLine("            'status'/'stats'/'st'          - View player stats & equipment.");
            Console.WriteLine("  Game:     'help'                           - Show this list of commands.");
            Console.WriteLine("            'quit'                           - Exit the game.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPress any key to begin...");
            Console.ReadKey(true);
            Console.Clear();
        }

        /// <summary>
        /// The GameLoop
        /// </summary>
        private void GameLoop()
        {
            _gameOver = false;
            _currentRoom?.OnPlayerEnter(_player);

            while (!_gameOver && _player.IsAlive)
            {
                _currentRoom = _gameMap.GetRoom(_playerPosition);
                if (_currentRoom == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("CRITICAL ERROR: Player is in an invalid location!"); Console.ForegroundColor = ConsoleColor.White;
                    _gameOver = true; break;
                }

                var aliveMonstersInRoom = _currentRoom.MonstersInRoom.Where(m => m.IsAlive).ToList();
                if (aliveMonstersInRoom.Any())
                {
                    Monster monster = aliveMonstersInRoom[0];
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n--- {monster.Name}'s Turn ---");
                    Console.ForegroundColor = ConsoleColor.White;

                    monster.PerformAction(_player, _currentRoom, _gameMap);

                    if (!_player.IsAlive) { _gameOver = true; break; }

                    aliveMonstersInRoom = _currentRoom.MonstersInRoom.Where(m => m.IsAlive).ToList();
                }

                if (_currentRoom.IsExitRoom && !aliveMonstersInRoom.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n*** The Dragon is defeated! You have conquered the dungeon! ***");
                    Console.ForegroundColor = ConsoleColor.White;
                    _gameOver = true; break;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n--- Your Turn ---");
                Console.Write($"{_player.Name} (HP: {_player.Health}/{_player.MaxHealth}) at {_playerPosition}> ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine()?.Trim() ?? "";
                ProcessPlayerInput(input);

                Console.WriteLine("\n---------------------------------\n");
            }
        }

        /// <summary>
        /// The ProcessPlayerInput
        /// </summary>
        /// <param name="input">The input<see cref="string"/></param>
        private void ProcessPlayerInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            input = input.ToLowerInvariant();

            string[] parts = input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string commandWord = parts[0];
            string argument = parts.Length > 1 ? parts[1] : null;
            string commandAction = commandWord;

            if (_currentRoom.IsPuzzleActive)
            {
                bool isAllowedPuzzleCommand = commandWord == "solve" || commandWord == "look" ||
                                              commandWord == "inventory" || commandWord == "inv" || commandWord == "i" ||
                                              commandWord == "status" || commandWord == "stats" || commandWord == "st" ||
                                              commandWord == "help" || commandWord == "quit";

                if (!isAllowedPuzzleCommand)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("You must focus on the puzzle! Type 'solve', 'look', or other allowed status/game commands.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                if (commandWord == "l") commandAction = "look";
            }
            else
            {
                string resolvedAction = commandWord;
                switch (commandWord)
                {
                    case "f": resolvedAction = Direction.Forward; break;
                    case "b": resolvedAction = Direction.Back; break;
                    case "r": resolvedAction = Direction.Right; break;
                    case "l": resolvedAction = Direction.Left; break;
                    case "forward": resolvedAction = Direction.Forward; break;
                    case "back": resolvedAction = Direction.Back; break;
                    case "right": resolvedAction = Direction.Right; break;
                    case "left": resolvedAction = Direction.Left; break;
                    case "pickup":
                    case "get": resolvedAction = "take"; break;
                    case "a": resolvedAction = "attack"; break;
                    case "inv":
                    case "i": resolvedAction = "inventory"; break;
                    case "stats":
                    case "st": resolvedAction = "status"; break;
                    case "eq": resolvedAction = "equip"; break;
                }
                commandAction = resolvedAction;
            }

            bool isMovementCommand = commandAction == Direction.Forward || commandAction == Direction.Back ||
                                     commandAction == Direction.Left || commandAction == Direction.Right;

            try
            {
                _currentRoom = _gameMap.GetRoom(_playerPosition);
                if (_currentRoom == null) throw new InvalidOperationException("Player location has become invalid!");

                if (isMovementCommand)
                {
                    MovePlayer(commandAction);
                }
                else
                {
                    switch (commandAction)
                    {
                        case "look": _currentRoom.DescribeRoom(); break;
                        case "take": HandleGetItem(argument); break;
                        case "inventory": _player.Inventory.DisplayInventory(); break;
                        case "status": _player.DisplayStatus(); break;
                        case "use": HandleUseItem(argument); break;
                        case "equip": HandleEquipWeapon(argument); break;
                        case "attack": HandleAttackMonster(); break;
                        case "solve": HandleSolvePuzzle(); break;
                        case "help": DisplayGameInstructions(); break;
                        case "quit": HandleQuit(); break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unknown command: '{commandWord}'. Type 'help' for commands.");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error processing command '{commandWord}': {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The MovePlayer
        /// </summary>
        /// <param name="direction">The direction<see cref="string"/></param>
        private void MovePlayer(string direction)
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition);
            if (_currentRoom == null) return;

            if (_currentRoom.MonstersInRoom.Any(m => m.IsAlive))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot move while a hostile creature is present! Deal with the {0} first.", _currentRoom.MonstersInRoom.First(m => m.IsAlive).Name);
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (!_currentRoom.Exits.TryGetValue(direction, out Room nextRoom) || nextRoom == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You cannot go {direction.ToLower()}. There is no exit that way.");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (nextRoom.IsLocked)
            {
                bool hasKey = _player.Inventory.GetAllItems()
                                     .OfType<Key>()
                                     .Any(k => k.KeyId == nextRoom.RequiredKeyId);
                if (hasKey)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Key usedKey = _player.Inventory.GetAllItems().OfType<Key>().First(k => k.KeyId == nextRoom.RequiredKeyId);
                    Console.WriteLine($"You use '{usedKey.Name}' to unlock the way {direction.ToLower()}.");
                    Console.ForegroundColor = ConsoleColor.White;
                    nextRoom.IsLocked = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The way {direction.ToLower()} is locked! You need the correct key (ID: {nextRoom.RequiredKeyId}).");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            _previousRoom = _currentRoom;
            _playerPosition = nextRoom.Coordinates;
            _currentRoom = nextRoom;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"You move {direction.ToLower()}...");
            Console.ForegroundColor = ConsoleColor.White;
            _currentRoom.OnPlayerEnter(_player);
        }

        /// <summary>
        /// The HandleGetItem
        /// </summary>
        /// <param name="itemName">The itemName<see cref="string"/></param>
        private void HandleGetItem(string itemName)
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition);
            if (_currentRoom == null) return;
            if (string.IsNullOrWhiteSpace(itemName)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Take what item?"); Console.ForegroundColor = ConsoleColor.White; return; }

            Item itemToGet = _currentRoom.ItemsInRoom.FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (itemToGet == null)
            {
                var partialMatches = _currentRoom.ItemsInRoom
                    .Where(item => item.Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                    .ToList();

                if (partialMatches.Count == 1)
                {
                    itemToGet = partialMatches[0];
                    Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Taking {itemToGet.Name})"); Console.ForegroundColor = ConsoleColor.White;
                }
                else if (partialMatches.Count > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which item do you want to take? Be more specific: {string.Join(", ", partialMatches.Select(i => i.Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"There is no '{itemName}' here to take."); Console.ForegroundColor = ConsoleColor.White; return;
                }
            }

            _player.PickUpItem(itemToGet, _currentRoom);
        }

        /// <summary>
        /// The HandleUseItem
        /// </summary>
        /// <param name="itemName">The itemName<see cref="string"/></param>
        private void HandleUseItem(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Use what item?"); Console.ForegroundColor = ConsoleColor.White; return; }

            Item itemInInventory = _player.Inventory.GetItemByName(itemName);

            if (itemInInventory == null)
            {
                var useableMatches = _player.Inventory.GetUseableItems()
                    .Where(item => ((Item)item).Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                    .ToList();

                if (useableMatches.Count == 1)
                {
                    itemInInventory = (Item)useableMatches[0];
                    Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Using {itemInInventory.Name})"); Console.ForegroundColor = ConsoleColor.White;
                }
                else if (useableMatches.Count > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which consumable item? Be more specific: {string.Join(", ", useableMatches.Select(i => ((Item)i).Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                }
                else
                {
                    var nonUseableMatches = _player.Inventory.GetAllItems()
                        .Where(item => !(item is IUseable) && item.Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                        .ToList();

                    if (nonUseableMatches.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You have '{nonUseableMatches.First().Name}', but it is not consumable."); Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You don't have a consumable item called '{itemName}'."); Console.ForegroundColor = ConsoleColor.White;
                    }
                    return;
                }
            }

            if (itemInInventory is IUseable useableItem)
            {
                useableItem.Use(_player);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You cannot use '{itemInInventory.Name}'. It's not consumable."); Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The HandleEquipWeapon
        /// </summary>
        /// <param name="argument">The argument<see cref="string"/></param>
        private void HandleEquipWeapon(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Equip what? (Weapon name, 'strongest', or 'none'/'fists')."); Console.ForegroundColor = ConsoleColor.White; return; }

            argument = argument.Trim().ToLowerInvariant();

            if (argument == "none" || argument == "fists")
            {
                _player.EquipWeapon(null);
                return;
            }

            Weapon weaponToEquip = null;

            if (argument == "strongest" || argument == "s")
            {
                weaponToEquip = _player.Inventory.GetStrongestWeapon();
                if (weaponToEquip == null) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("You have no weapons in your inventory to equip."); Console.ForegroundColor = ConsoleColor.White; return; }
                Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Equipping strongest: {weaponToEquip.Name})"); Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                weaponToEquip = _player.Inventory.GetWeapons().FirstOrDefault(w => w.Name.Equals(argument, StringComparison.OrdinalIgnoreCase));

                if (weaponToEquip == null)
                {
                    var partialMatches = _player.Inventory.GetWeapons()
                        .Where(w => w.Name.ToLowerInvariant().Contains(argument))
                        .ToList();

                    if (partialMatches.Count == 1)
                    {
                        weaponToEquip = partialMatches[0];
                        Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Equipping {weaponToEquip.Name})"); Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (partialMatches.Count > 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which weapon? Be more specific: {string.Join(", ", partialMatches.Select(w => w.Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You don't have a weapon called '{argument}' in your inventory."); Console.ForegroundColor = ConsoleColor.White; return;
                    }
                }
            }

            _player.EquipWeapon(weaponToEquip);
        }

        /// <summary>
        /// The HandleAttackMonster
        /// </summary>
        private void HandleAttackMonster()
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition); if (_currentRoom == null) return;

            Monster target = _currentRoom.MonstersInRoom.FirstOrDefault(m => m.IsAlive);

            if (target == null) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("There is no monster here to attack."); Console.ForegroundColor = ConsoleColor.White; return; }

            _player.Attack(target);

            if (!target.IsAlive)
            {
                _currentRoom.TurnSafe();
                _player.AddExperience(target.ExperienceValue);

                Item loot = target.GetLootDrop();
                if (loot != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"{target.Name} dropped: {loot.Name}"); Console.ForegroundColor = ConsoleColor.White;
                    _currentRoom.AddItem(loot);
                }
            }
        }

        /// <summary>
        /// The HandleSolvePuzzle
        /// </summary>
        private void HandleSolvePuzzle()
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition);
            if (_currentRoom == null || !_currentRoom.IsPuzzleActive) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("There's no active puzzle here to solve."); Console.ForegroundColor = ConsoleColor.White; return; }

            Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine("\nYou examine the strange mechanism..."); Console.ForegroundColor = ConsoleColor.White;

            if (_currentRoom.MonstersInRoom.Any(m => m.IsAlive)) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Cannot focus on the puzzle with monsters nearby!"); Console.ForegroundColor = ConsoleColor.White; return; }

            int correctAnswers = 0; int questionsNeeded = 3; bool failed = false;
            for (int i = 0; i < questionsNeeded; i++)
            {
                int numRange = 5 + _currentRoom.GenerationLevel * 2;
                int n1 = _random.Next(1, numRange + 1);
                int n2 = _random.Next(1, numRange + 1);
                int opType = _random.Next(3);
                int answer = 0; string qText = "";

                switch (opType)
                {
                    case 0: answer = n1 + n2; qText = $"{n1} + {n2}"; break;
                    case 1:
                        if (n2 > n1) { (n1, n2) = (n2, n1); }
                        answer = n1 - n2; qText = $"{n1} - {n2}"; break;
                    case 2:
                        n1 = _random.Next(1, Math.Max(2, numRange / 2 + 1));
                        n2 = _random.Next(1, Math.Max(2, numRange / 2 + 1));
                        answer = n1 * n2; qText = $"{n1} * {n2}"; break;
                }

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write($"Question {i + 1} of {questionsNeeded}: What is {qText}? "); Console.ForegroundColor = ConsoleColor.White;
                string inputAnswer = Console.ReadLine();

                if (int.TryParse(inputAnswer, out int userAnswer) && userAnswer == answer)
                {
                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Correct!"); Console.ForegroundColor = ConsoleColor.White; correctAnswers++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"Incorrect! The answer was {answer}."); Console.ForegroundColor = ConsoleColor.White;
                    failed = true; break;
                }
            }

            if (!failed && correctAnswers == questionsNeeded)
            {
                _currentRoom.CompletePuzzle(_player);
            }
            else
            {
                _currentRoom.FailPuzzle();

                if (_previousRoom != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nThe failed puzzle mechanism shunts you forcefully back the way you came!");
                    Console.ForegroundColor = ConsoleColor.White;

                    _playerPosition = _previousRoom.Coordinates;
                    Room roomReturnedTo = _previousRoom;
                    _currentRoom = roomReturnedTo;
                    _previousRoom = null;

                    Console.WriteLine("\n---------------------------------\n");
                    _currentRoom.OnPlayerEnter(_player);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nThe mechanism buzzes angrily but you remain in the room (nowhere to go back to!).");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        /// <summary>
        /// The HandleQuit
        /// </summary>
        private void HandleQuit()
        {
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("Are you sure you want to quit? (y/n): "); Console.ForegroundColor = ConsoleColor.White;
            string confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (confirmation == "y")
            {
                _gameOver = true;
                Console.ForegroundColor = ConsoleColor.Gray; Console.WriteLine("Quitting game..."); Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Quit cancelled."); Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The DisplayGameResult
        /// </summary>
        private void DisplayGameResult()
        {
            Console.WriteLine("\n=========================================");
            if (_player == null) return;

            bool won = _player.IsAlive && _gameOver && _currentRoom != null && _currentRoom.IsExitRoom && !_currentRoom.MonstersInRoom.Any(m => m.IsAlive);

            if (won)
            {
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n*** VICTORY! {_player.Name} (Level {_player.Level}) has slain the Dragon! ***");
                Console.WriteLine($"You conquered the dungeon, finishing at {_playerPosition}.");
                Console.WriteLine($"Final Health: {_player.Health}/{_player.MaxHealth}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (!_player.IsAlive)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($"\n*** Alas, {_player.Name} (Level {_player.Level}) has fallen! ***");
                Console.WriteLine("The dungeon claims another soul...");
                if (_currentRoom != null) Console.WriteLine($"Defeated in room {_playerPosition} (Depth: {_currentRoom.GenerationLevel}, Type: {_currentRoom.GeneratedType})");
                else Console.WriteLine($"Defeated near {_playerPosition}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("\n--- Game Ended ---");
                if (_currentRoom != null) Console.WriteLine($"You stopped your journey at {_playerPosition} (Depth: {_currentRoom.GenerationLevel}, Type: {_currentRoom.GeneratedType}).");
                else Console.WriteLine($"You stopped your journey near {_playerPosition}.");
                Console.WriteLine($"Level: {_player.Level}, Health: {_player.Health}/{_player.MaxHealth}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("=========================================\n");
        }
    }
}
