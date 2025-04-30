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
        /// Defines the _playerAttackedLastTurn
        /// </summary>
        private bool _playerAttackedLastTurn = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class.
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
                DisplayIntro(); // Welcome message
                InitialisePlayer(); // Player setup
                if (_player == null) return; // Check if player was created successfully

                SelectDifficultyAndGenerateMap(); // Map generation

                if (_gameMap.StartRoom == null) // Check if map generation was successful
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CRITICAL ERROR: Map generation failed. Cannot start game.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                _currentRoom = _gameMap.StartRoom; // Set the starting room
                _playerPosition = _currentRoom.Coordinates; // Set the player's starting position
                _previousRoom = null; // Set the previous room to null

                DisplayGameInstructions(); // Game instructions
                GameLoop(); // Main game loop
            }
            catch (Exception ex) // Handle any unexpected errors
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
                if (_player != null) // If player creation was successful
                {
                    DisplayGameResult(); // Display game result 
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
* Welcome to DUNGEON EXPLORER!      *
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
            string playerName = ""; // Player name input
            Console.WriteLine("Enter your adventurer's name (max 25 characters):");
            while (true) // Loop until valid input
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.White;
                playerName = Console.ReadLine()?.Trim(); // Get player name input

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length <= 25) break; // Check for valid name
                else if (string.IsNullOrWhiteSpace(playerName)) // Check for empty name
                { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Name cannot be empty."); }
                else
                { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Name is too long (max 25 characters)."); }
                Console.ForegroundColor = ConsoleColor.White;
            }

            _player = new Player(playerName); // Create player instance

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
        private void SelectDifficultyAndGenerateMap() // Select difficulty and generate map
        {
            _difficultyChoice = 2; // Default to Normal difficulty

            Console.WriteLine("\nSelect Difficulty:");
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("1. Easy   (Fewer Rooms, Weaker Foes)");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("2. Normal (Standard Rooms & Foes)");
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("3. Hard   (More Rooms, Tougher Foes)");
            Console.ForegroundColor = ConsoleColor.White;

            while (true) // Loop until valid input
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter your choice (1-3): ");
                string choiceStr = Console.ReadLine()?.Trim(); // Get user input
                switch (choiceStr) // Check and process input
                {
                    case "1": _difficultyChoice = 1; Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Difficulty set to Easy. Generating map..."); goto Generate;
                    case "3": _difficultyChoice = 3; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Difficulty set to Hard. Generating map..."); goto Generate;
                    case "2": _difficultyChoice = 2; Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Difficulty set to Normal. Generating map..."); goto Generate;
                    default: Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Invalid choice. Please enter 1, 2, or 3."); break;
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

        Generate: // Generate map based on difficulty
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
            Console.WriteLine("  Actions:  'look'                       - Describe the current room.");
            Console.WriteLine("            'pickup [item]'/'take [item]' - Pick up an item from the room.");
            Console.WriteLine("            'use [consumable]'           - Use a Potion or Food from inventory.");
            Console.WriteLine("            'equip [weapon]'/'eq [w]'    - Equip a weapon by name from inventory.");
            Console.WriteLine("            'equip strongest'/'eq s'     - Equip highest damage weapon from inventory.");
            Console.WriteLine("            'equip fists'/'eq none'      - Unequip current weapon.");
            Console.WriteLine("            'attack'/'a'                 - Attack the monster in the room (Monster will retaliate).");
            Console.WriteLine("            'solve'                      - Attempt to solve a puzzle in the room.");
            Console.WriteLine("  Status:   'inventory'/'inv'/'i'      - View your inventory.");
            Console.WriteLine("            'status'/'stats'/'st'        - View player stats & equipment.");
            Console.WriteLine("  Game:     'help'                       - Show this list of commands.");
            Console.WriteLine("            'quit'                       - Exit the game.");
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
            _gameOver = false; // Game loop flag
            _currentRoom?.OnPlayerEnter(_player); // Call OnPlayerEnter method of the current room

            while (!_gameOver && _player.IsAlive) // Loop until game over or player is dead
            {
                _currentRoom = _gameMap.GetRoom(_playerPosition); // Check if current room is valid
                if (_currentRoom == null) // Check if current room is null
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("CRITICAL ERROR: Player is in an invalid location!"); Console.ForegroundColor = ConsoleColor.White;
                    _gameOver = true; break;
                }

                var aliveMonstersInRoom = _currentRoom.MonstersInRoom.Where(m => m.IsAlive).ToList(); // Get alive monsters in the room
                if (aliveMonstersInRoom.Any()) // Check if there are any alive monsters
                {
                    Monster monster = aliveMonstersInRoom[0]; // Get the first alive monster

                    if (_playerAttackedLastTurn) // Check if player attacked last turn
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"\n--- {monster.Name}'s Turn (Retaliating) ---");
                        Console.ForegroundColor = ConsoleColor.White;
                        monster.PerformAction(_player, _currentRoom, _gameMap);
                    }
                    else // Check if player did not attack last turn
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"\n--- {monster.Name}'s Turn (Waiting) ---");
                        Console.WriteLine($"{monster.Name} watches you warily, waiting for you to act...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    _playerAttackedLastTurn = false; // Reset player attack flag

                    if (!_player.IsAlive) { _gameOver = true; break; } // Check if player is dead

                    aliveMonstersInRoom = _currentRoom.MonstersInRoom.Where(m => m.IsAlive).ToList(); // Get alive monsters in the room again
                }
                else // Check if there are no alive monsters
                {
                    _playerAttackedLastTurn = false; // Reset player attack flag
                }
                if (_currentRoom.IsExitRoom && !aliveMonstersInRoom.Any()) // Check if current room is the exit room and no alive monsters
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
        private void ProcessPlayerInput(string input) // Process player input
        {
            if (string.IsNullOrWhiteSpace(input)) return; // Check for empty input

            input = input.ToLowerInvariant(); // Normalise input to lowercase

            string[] parts = input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries); // Split input into command and argument
            string commandWord = parts[0]; // Get command word
            string argument = parts.Length > 1 ? parts[1] : null; // Get argument (if any)
            string commandAction = commandWord; // Normalise command action

            bool currentPlayerActionIsAttack = false; // Check if player action is an attack

            if (_currentRoom.IsPuzzleActive) // Check if current room is a puzzle room
            {
                bool isAllowedPuzzleCommand = commandWord == "solve" || commandWord == "look" ||
                                              commandWord == "inventory" || commandWord == "inv" || commandWord == "i" ||
                                              commandWord == "status" || commandWord == "stats" || commandWord == "st" ||
                                              commandWord == "help" || commandWord == "quit"; // Check if command is allowed in puzzle room

                if (!isAllowedPuzzleCommand) // Check if command is not allowed in puzzle room
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("You must focus on the puzzle! Type 'solve', 'look', or other allowed status/game commands.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                if (commandWord == "l") commandAction = "look";
            }
            else // Check if current room is not a puzzle room
            {
                string resolvedAction = commandWord; // Resolve action based on command word
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
                    case "eq": resolvedAction = "equip"; break;
                    case "inv":
                    case "i": resolvedAction = "inventory"; break;
                    case "stats":
                    case "st": resolvedAction = "status"; break;
                }
                commandAction = resolvedAction; // Normalise command action
            }

            bool isMovementCommand = commandAction == Direction.Forward || commandAction == Direction.Back ||
                                     commandAction == Direction.Left || commandAction == Direction.Right; // Check if command is a movement command

            try // Check if current room is valid
            {
                _currentRoom = _gameMap.GetRoom(_playerPosition); // Check if current room is valid
                if (_currentRoom == null) throw new InvalidOperationException("Player location has become invalid!"); // Check if current room is null

                if (isMovementCommand) // Check if command is a movement command
                {
                    MovePlayer(commandAction); // Move player in the specified direction
                }
                else // Check if command is not a movement command
                {
                    switch (commandAction) 
                    {
                        case "look": _currentRoom.DescribeRoom(); break;
                        case "take": HandleGetItem(argument); break;
                        case "inventory": _player.Inventory.DisplayInventory(); break;
                        case "status": _player.DisplayStatus(); break;
                        case "use": HandleUseItem(argument); break;
                        case "equip": HandleEquipWeapon(argument); break;
                        case "attack":
                            HandleAttackMonster();
                            currentPlayerActionIsAttack = true;
                            break;
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
            catch (Exception ex) // Handle any exceptions
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error processing command '{commandWord}': {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                _playerAttackedLastTurn = currentPlayerActionIsAttack; // Set player attack flag
            }
        }

        /// <summary>
        /// The MovePlayer
        /// </summary>
        /// <param name="direction">The direction<see cref="string"/></param>
        private void MovePlayer(string direction) // Move player in the spcecified directions
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition); // Check if current room is valid
            if (_currentRoom == null) return; // Check if current room is null

            if (_currentRoom.MonstersInRoom.Any(m => m.IsAlive)) // Check if there are any alive monsters in the room
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot move while a hostile creature is present! Deal with the {0} first.", _currentRoom.MonstersInRoom.First(m => m.IsAlive).Name);
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (!_currentRoom.Exits.TryGetValue(direction, out Room nextRoom) || nextRoom == null) // Check if there is an exit in the specified direction
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You cannot go {direction.ToLower()}. There is no exit that way.");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (nextRoom.IsLocked) // Check if the next room is locked
            {
                Key matchingKey = _player.Inventory.GetAllItems() // Get all items in the player's inventory
                                       .OfType<Key>()
                                       .FirstOrDefault(k => k.KeyId == nextRoom.RequiredKeyId);

                if (matchingKey != null) // Check if the player has the correct key
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You use '{matchingKey.Name}' to unlock the way {direction.ToLower()}.");
                    Console.ForegroundColor = ConsoleColor.White;
                    nextRoom.IsLocked = false;
                }
                else // Check if the player does not have the correct key
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The way {direction.ToLower()} is locked! You need the correct key (ID: {nextRoom.RequiredKeyId}).");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            _previousRoom = _currentRoom; // Store the previous room
            _playerPosition = nextRoom.Coordinates; // Update the player's position
            _currentRoom = nextRoom; // Get the next room

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
        private void HandleGetItem(string itemName) // Pick up an item from the room
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition); // Check if current room is valid
            if (_currentRoom == null) return; // Check if current room is null
            if (string.IsNullOrWhiteSpace(itemName)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Take what item?"); Console.ForegroundColor = ConsoleColor.White; return; } // Check for empty item name

            Item itemToGet = _currentRoom.ItemsInRoom.FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)); // Get item by name

            if (itemToGet == null) // Check if item is not found by name
            {
                var partialMatches = _currentRoom.ItemsInRoom
                    .Where(item => item.Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                    .ToList(); // Get partial matches

                if (partialMatches.Count == 1) // Check if there is only one partial match
                {
                    itemToGet = partialMatches[0];
                    Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Taking {itemToGet.Name})"); Console.ForegroundColor = ConsoleColor.White;
                }
                else if (partialMatches.Count > 1) // Check if there are multiple partial matches
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which item do you want to take? Be more specific: {string.Join(", ", partialMatches.Select(i => i.Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                }
                else // Check if there are no matches
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"There is no '{itemName}' here to take."); Console.ForegroundColor = ConsoleColor.White; return;
                }
            }

            _player.PickUpItem(itemToGet, _currentRoom); // Add item to player's inventory
        }

        /// <summary>
        /// The HandleUseItem
        /// </summary>
        /// <param name="itemName">The itemName<see cref="string"/></param>
        private void HandleUseItem(string itemName) // Use an item from the inventory
        {
            if (string.IsNullOrWhiteSpace(itemName)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Use what item?"); Console.ForegroundColor = ConsoleColor.White; return; } // Check for empty item name

            Item itemInInventory = _player.Inventory.GetItemByName(itemName); // Get item by name

            if (itemInInventory == null) // Check if item is not found by name
            {
                var useableMatches = _player.Inventory.GetUseableItems()
                   .Where(item => ((Item)item).Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                   .ToList(); // Get useable items

                if (useableMatches.Count == 1) // Check if there is only one useable match
                {
                    itemInInventory = (Item)useableMatches[0]; // Get the item
                    Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Using {itemInInventory.Name})"); Console.ForegroundColor = ConsoleColor.White;
                }
                else if (useableMatches.Count > 1) // Check if there are multiple useable matches
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which consumable item? Be more specific: {string.Join(", ", useableMatches.Select(i => ((Item)i).Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                }
                else // Check if there are no matched
                {
                    var nonUseableMatches = _player.Inventory.GetAllItems()
                        .Where(item => !(item is IUseable) && item.Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                        .ToList(); // Get non-useable items

                    if (nonUseableMatches.Any()) // Check if there are any non-useable matches
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You have '{nonUseableMatches.First().Name}', but it is not consumable."); Console.ForegroundColor = ConsoleColor.White;
                    }
                    else // Check if there are no matches
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You don't have a consumable item called '{itemName}'."); Console.ForegroundColor = ConsoleColor.White;
                    }
                    return; // Exit the method
                }
            }

            if (itemInInventory is IUseable useableItem) // Check if item is useable
            {
                useableItem.Use(_player); // Use the item
            }
            else // Check if item is not useable
            {
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You cannot use '{itemInInventory.Name}'. It's not consumable."); Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The HandleEquipWeapon
        /// </summary>
        /// <param name="argument">The argument<see cref="string"/></param>
        private void HandleEquipWeapon(string argument) // Equip a weapon from the inventory
        {
            if (string.IsNullOrWhiteSpace(argument)) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Equip what? (Weapon name, 'strongest', or 'none'/'fists')."); Console.ForegroundColor = ConsoleColor.White; return; } // Check for empty argument

            argument = argument.Trim().ToLowerInvariant(); // Normalise argument to lowercase

            if (argument == "none" || argument == "fists") // Check if argument is 'none' or 'fists'
            {
                _player.EquipWeapon(null); // Unequip current weapon
                return;
            }

            Weapon weaponToEquip = null; // Get the weapon to equip

            if (argument == "strongest" || argument == "s") // Check if argument is 'strongest' or 's'
            {
                weaponToEquip = _player.Inventory.GetStrongestWeapon(); // Get the strongest weapon in the inventory
                if (weaponToEquip == null) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("You have no weapons in your inventory to equip."); Console.ForegroundColor = ConsoleColor.White; return; } // Check if there are no weapons in the inventory
                Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Equipping strongest: {weaponToEquip.Name})"); Console.ForegroundColor = ConsoleColor.White; // Display the equipped weapon
            }
            else // Check if argument is a specific weapon name
            {
                weaponToEquip = _player.Inventory.GetWeapons().FirstOrDefault(w => w.Name.Equals(argument, StringComparison.OrdinalIgnoreCase)); // Get weapon by name

                if (weaponToEquip == null) // Check if weapon is not found by name
                {
                    var partialMatches = _player.Inventory.GetWeapons()
                        .Where(w => w.Name.ToLowerInvariant().Contains(argument))
                        .ToList(); // Get partial matches

                    if (partialMatches.Count == 1) // Check if there is only one partial match
                    {
                        weaponToEquip = partialMatches[0]; // Get the weapon
                        Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine($"(Equipping {weaponToEquip.Name})"); Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (partialMatches.Count > 1) // Check if there are multiple partial matches
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Which weapon? Be more specific: {string.Join(", ", partialMatches.Select(w => w.Name))}"); Console.ForegroundColor = ConsoleColor.White; return;
                    }
                    else // Check if there are no matches
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"You don't have a weapon called '{argument}' in your inventory."); Console.ForegroundColor = ConsoleColor.White; return;
                    }
                }
            }

            _player.EquipWeapon(weaponToEquip); // Equip the weapon
        }

        /// <summary>
        /// The HandleAttackMonster
        /// </summary>
        private void HandleAttackMonster() // Attack a monster in the room
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition); if (_currentRoom == null) return; // Check if current room is valid

            Monster target = _currentRoom.MonstersInRoom.FirstOrDefault(m => m.IsAlive); // Get the first alive monster in the room

            if (target == null) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("There is no monster here to attack."); Console.ForegroundColor = ConsoleColor.White; return; } // Check if there are any alive monsters

            _player.Attack(target); // Attack the monster

            if (!target.IsAlive) // Check if the monster is dead
            {
                _currentRoom.TurnSafe(); // Mark the room as safe
                _player.AddExperience(target.ExperienceValue); // Add experience to the player

                Item loot = target.GetLootDrop(); // Get loot drop from the monster
                if (loot != null) // Check if loot is available
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"{target.Name} dropped: {loot.Name}"); Console.ForegroundColor = ConsoleColor.White;
                    _currentRoom.AddItem(loot);
                }
            }
        }

        /// <summary>
        /// The HandleSolvePuzzle
        /// </summary>
        private void HandleSolvePuzzle() // Solve a puzzle in the room
        {
            _currentRoom = _gameMap.GetRoom(_playerPosition); // Check if current room is valid
            if (_currentRoom == null || !_currentRoom.IsPuzzleActive) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("There's no active puzzle here to solve."); Console.ForegroundColor = ConsoleColor.White; return; } // Check if current room is null or not a puzzle room

            Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine("\nYou examine the strange mechanism..."); Console.ForegroundColor = ConsoleColor.White; // Display puzzle message

            if (_currentRoom.MonstersInRoom.Any(m => m.IsAlive)) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Cannot focus on the puzzle with monsters nearby!"); Console.ForegroundColor = ConsoleColor.White; return; } // Check if there are any alive monsters

            int correctAnswers = 0; int questionsNeeded = 3; bool failed = false; // Check if the puzzle is active
            for (int i = 0; i < questionsNeeded; i++) // Generate and ask questions
            {
                int numRange = 5 + _currentRoom.GenerationLevel * 2; // Set the number range based on room level
                int n1 = _random.Next(1, numRange + 1); // Generate first number
                int n2 = _random.Next(1, numRange + 1); // Generate second number
                int opType = _random.Next(3); // Generate operation type (0: addition, 1: subtraction, 2: multiplication)
                int answer = 0; string qText = ""; // Generate question text

                switch (opType) // Check operation type
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
                string inputAnswer = Console.ReadLine(); // Get user input

                if (int.TryParse(inputAnswer, out int userAnswer) && userAnswer == answer) // Check if the answer is correct
                {
                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Correct!"); Console.ForegroundColor = ConsoleColor.White; correctAnswers++;
                }
                else // Check if the answer is incorrect
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"Incorrect! The answer was {answer}."); Console.ForegroundColor = ConsoleColor.White;
                    failed = true; break;
                }
            }

            if (!failed && correctAnswers == questionsNeeded) // Check if all answers are correct
            {
                _currentRoom.CompletePuzzle(_player); // Mark the puzzle as complete
            }
            else
            {
                _currentRoom.FailPuzzle(); // Mark the puzzle as failed

                if (_previousRoom != null) // Check if there is a previous room
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nThe failed puzzle mechanism shunts you forcefully back the way you came!");
                    Console.ForegroundColor = ConsoleColor.White;

                    _playerPosition = _previousRoom.Coordinates; // Update the player's position
                    Room roomReturnedTo = _previousRoom; // Get the previous room
                    _currentRoom = roomReturnedTo; // Get the current room
                    _previousRoom = null; // Set the previous room to null

                    Console.WriteLine("\n---------------------------------\n");
                    _currentRoom.OnPlayerEnter(_player); // Call OnPlayerEnter method of the current room
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nThe mechanism buzzes angrily but you remain in the room (nowhere to go back to!). The puzzle resets.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        /// <summary>
        /// The HandleQuit
        /// </summary>
        private void HandleQuit() // Quit the game
        {
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("Are you sure you want to quit? (y/n): "); Console.ForegroundColor = ConsoleColor.White;
            string confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (confirmation == "y")
            {
                _gameOver = true; // Set game over flag
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
        private void DisplayGameResult() // Display the game result
        {
            Console.WriteLine("\n=========================================");
            if (_player == null) return; // Check if player is null

            bool won = _player.IsAlive && _gameOver && _currentRoom != null && _currentRoom.IsExitRoom && !_currentRoom.MonstersInRoom.Any(m => m.IsAlive); // Check if the player won

            if (won) // Check if the player won
            {
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n*** VICTORY! {_player.Name} (Level {_player.Level}) has slain the Dragon! ***");
                Console.WriteLine($"You conquered the dungeon, finishing at {_playerPosition}.");
                Console.WriteLine($"Final Health: {_player.Health}/{_player.MaxHealth}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (!_player.IsAlive) // Check if the player is dead
            {
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($"\n*** Alas, {_player.Name} (Level {_player.Level}) has fallen! ***");
                Console.WriteLine("The dungeon claims another soul...");
                if (_currentRoom != null) Console.WriteLine($"Defeated in room {_playerPosition} (Depth: {_currentRoom.GenerationLevel}, Type: {_currentRoom.GeneratedType})");
                else Console.WriteLine($"Defeated near {_playerPosition}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else // Check if the game was quit
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
