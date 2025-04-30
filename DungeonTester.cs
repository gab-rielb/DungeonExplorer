namespace DungeonExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using DungeonExplorer.Items;
    using DungeonExplorer.Monsters;

    /// <summary>
    /// Defines the <see cref="DungeonTester" />
    /// </summary>
    public class DungeonTester
    {
        /// <summary>
        /// Defines the _logFilePath
        /// </summary>
        private string _logFilePath = "test_log.txt";

        /// <summary>
        /// Defines the _logWriter
        /// </summary>
        private StreamWriter _logWriter;

        /// <summary>
        /// Defines the _testsPassed
        /// </summary>
        private int _testsPassed = 0;

        /// <summary>
        /// Defines the _testsFailed
        /// </summary>
        private int _testsFailed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DungeonTester"/> class.
        /// </summary>
        /// <param name="logFileName">The logFileName<see cref="string"/></param>
        public DungeonTester(string logFileName = "test_log.txt")
        {
            _logFilePath = logFileName;
        }

        /// <summary>
        /// The RunAllTests
        /// </summary>
        public void RunAllTests()
        {
            try
            {
                using (_logWriter = new StreamWriter(_logFilePath, append: true))
                {
                    Log($"\n--- Starting Test Run: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---");

                    TestPlayerCreationAndStats();
                    TestItemCreation();
                    TestInventoryManagement();
                    TestCombatSystem();
                    TestLevelingSystem();
                    TestRoomAndMapBasics();
                    TestInteractionLogic();
                    TestPuzzleAndLockLogic();
                    TestMonsterAI();
                    Log($"\n--- Test Run Finished: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---");
                    Log($"Result: {_testsPassed} Passed, {_testsFailed} Failed.");

                    Console.WriteLine($"\nTest run complete. Results logged to '{_logFilePath}'.");
                    Console.WriteLine($"Passed: {_testsPassed}, Failed: {_testsFailed}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CRITICAL ERROR during testing setup/logging: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                _logWriter?.Close();
                Console.WriteLine($"Fallback Log: Test Run Aborted at {DateTime.Now:yyyy-MM-dd HH:mm:ss} due to Error: {ex}");
            }
        }

        /// <summary>
        /// The TestPlayerCreationAndStats
        /// </summary>
        private void TestPlayerCreationAndStats()
        {
            StartTest("Player Creation and Stats");
            Player player = null;
            try
            {
                player = new Player("Tester", 100, 5, 15);
                Assert(player.Name == "Tester", "Player name set correctly.");
                Assert(player.Health == 100, "Player initial health correct.");
                Assert(player.MaxHealth == 100, "Player initial max health correct.");
                Assert(player.Damage == 5, "Player base damage correct.");
                Assert(player.Level == 1, "Player initial level correct.");
                Assert(player.ExperiencePoints == 0, "Player initial XP correct.");
                Assert(player.Inventory.Capacity == 15, "Player inventory capacity correct.");
                Assert(player.EquippedWeapon != null && player.EquippedWeapon.Name == "Fists", "Player starts with Fists equipped.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestItemCreation
        /// </summary>
        private void TestItemCreation()
        {
            StartTest("Item Creation");
            try
            {
                Weapon sword = new Weapon("Iron Sword", "A basic sword.", 8);
                Assert(sword.Name == "Iron Sword" && sword.Damage == 8 && !sword.IsUsable, "Weapon created correctly.");

                Potion potion = new Potion("Healing Potion", "Heals.", 30);
                Assert(potion.Name == "Healing Potion" && potion.HealAmount == 30 && potion.IsUsable, "Potion created correctly.");

                Food bread = new Food("Bread", "Sustenance.", 15);
                Assert(bread.Name == "Bread" && bread.HealAmount == 15 && bread.IsUsable, "Food created correctly.");

                Key key = new Key("Rusty Key", "Opens something.", 123);
                Assert(key.Name == "Rusty Key" && key.KeyId == 123 && !key.IsUsable, "Key created correctly.");

                MiscItem junk = new MiscItem("Odd Stone", "A smooth stone.");
                Assert(junk.Name == "Odd Stone" && !junk.IsUsable, "Misc Item created correctly.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Item Creation - Invalid Arguments");
            try
            {
                try
                {
                    new Weapon("Bad", "Neg Dmg", -5);
                    FailTest("Negative weapon damage did not throw expected ArgumentOutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative weapon damage correctly threw ArgumentOutOfRangeException."); }
                catch (Exception ex) { FailTest($"Incorrect exception type for negative damage: {ex.GetType()}. Expected ArgumentOutOfRangeException."); }

                try
                {
                    new Potion("Bad", "Neg Heal", -10);
                    FailTest("Negative potion heal did not throw expected ArgumentOutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative potion heal correctly threw ArgumentOutOfRangeException."); }
                catch (Exception ex) { FailTest($"Incorrect exception type for negative heal: {ex.GetType()}. Expected ArgumentOutOfRangeException."); }

                try
                {
                    new Food("Bad", "Neg Food Heal", -1);
                    FailTest("Negative food heal did not throw expected ArgumentOutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative food heal correctly threw ArgumentOutOfRangeException."); }
                catch (Exception ex) { FailTest($"Incorrect exception type for negative food heal: {ex.GetType()}. Expected ArgumentOutOfRangeException."); }

                try
                {
                    new Key("Bad", "Neg Key ID", -1);
                    FailTest("Negative key ID did not throw expected ArgumentOutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative key ID correctly threw ArgumentOutOfRangeException."); }
                catch (Exception ex) { FailTest($"Incorrect exception type for negative key ID: {ex.GetType()}. Expected ArgumentOutOfRangeException."); }

            }
            catch (Exception ex) { FailTest($"General exception during invalid argument tests: {ex.Message}"); }
        }

        /// <summary>
        /// The TestInventoryManagement
        /// </summary>
        private void TestInventoryManagement()
        {
            StartTest("Inventory Add/Remove/Capacity (Keys Ignore Capacity)");
            Player player = new Player("InvTester", 100, 5, 3);
            Weapon sword = new Weapon("Sword", "S", 5);
            Potion potion = new Potion("Potion", "P", 10);
            Food bread = new Food("Bread", "B", 5);
            Key key = new Key("Key", "K", 1);

            try
            {
                Assert(player.Inventory.AddItem(sword), "Added first item (Sword).");
                Assert(player.Inventory.Count == 1, "Non-key count is 1.");
                Assert(player.Inventory.AddItem(potion), "Added second item (Potion).");
                Assert(player.Inventory.Count == 2, "Non-key count is 2.");
                Assert(player.Inventory.AddItem(bread), "Added third item (Bread).");
                Assert(player.Inventory.Count == 3, "Non-key count is 3.");
                Assert(player.Inventory.IsFull, "Inventory is full (for non-keys).");

                Assert(player.Inventory.AddItem(key), "Successfully added item (Key) to full non-key inventory.");

                Assert(player.Inventory.Count == 3, "Non-key count remains 3 after adding key.");
                Assert(player.Inventory.GetAllItems().Count == 4, "Total item count (incl. key) is now 4.");
                Assert(player.Inventory.GetItemByName("Key") == key, "Key exists in inventory after adding.");
                Assert(player.Inventory.RemoveItem(potion), "Removed potion.");
                Assert(player.Inventory.Count == 2, "Non-key count is 2 after removal.");
                Assert(player.Inventory.GetAllItems().Count == 3, "Total item count is 3 after removal.");
                Assert(!player.Inventory.IsFull, "Inventory is not full (for non-keys) after removal.");

                Assert(player.Inventory.GetItemByName("Sword") == sword, "Found sword by name.");
                Assert(player.Inventory.GetItemByName("Potion") == null, "Did not find removed potion.");
                Assert(player.Inventory.GetItemByName("Key") == key, "Key still exists after removing potion.");

                Assert(!player.Inventory.RemoveItem(potion), "Failed to remove already removed potion.");

                Assert(player.Inventory.RemoveItem(key), "Removed key.");
                Assert(player.Inventory.Count == 2, "Non-key count remains 2 after removing key.");
                Assert(player.Inventory.GetAllItems().Count == 2, "Total item count is 2 after removing key.");
                Assert(player.Inventory.GetItemByName("Key") == null, "Key no longer exists after removal.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Inventory LINQ Methods");
            Player playerLinq = new Player("LinqTester", 100, 5, 10);
            Weapon swordLinq = new Weapon("Sword", "S", 5);
            Weapon daggerLinq = new Weapon("Dagger", "D", 4);
            Weapon axeLinq = new Weapon("Axe", "A", 7);
            Potion potionLinq = new Potion("Potion", "P", 10);
            Food breadLinq = new Food("Bread", "B", 5);
            Key keyLinq = new Key("Keycard", "KC", 101);

            playerLinq.Inventory.AddItem(swordLinq);
            playerLinq.Inventory.AddItem(daggerLinq);
            playerLinq.Inventory.AddItem(axeLinq);
            playerLinq.Inventory.AddItem(potionLinq);
            playerLinq.Inventory.AddItem(breadLinq);
            playerLinq.Inventory.AddItem(keyLinq);

            try
            {
                Assert(playerLinq.Inventory.GetWeapons().Count == 3, "Found 3 weapons.");
                Assert(playerLinq.Inventory.GetPotions().Count == 1, "Found 1 potion.");
                Assert(playerLinq.Inventory.GetFood().Count == 1, "Found 1 food.");
                Assert(playerLinq.Inventory.GetUseableItems().Count == 2, "Found 2 useable items (potion + food).");
                Assert(playerLinq.Inventory.GetStrongestWeapon() == axeLinq, "Found Axe as strongest weapon.");

                Assert(playerLinq.Inventory.GetAllItems().Count == 6, "GetAllItems includes key (total 6).");

                var sorted = playerLinq.Inventory.GetItemsSortedByName();
                Assert(sorted.Count == 6, "Sorted list has 6 items.");
                Assert(sorted[0].Name == "Axe", "Sorted list starts with Axe.");
                Assert(sorted[1].Name == "Bread", "Sorted list second is Bread.");
                Assert(sorted[2].Name == "Dagger", "Sorted list third is Dagger.");
                Assert(sorted[3].Name == "Keycard", "Sorted list fourth is Keycard.");
                Assert(sorted[4].Name == "Potion", "Sorted list fifth is Potion.");
                Assert(sorted[5].Name == "Sword", "Sorted list last is Sword.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestCombatSystem
        /// </summary>
        private void TestCombatSystem()
        {
            StartTest("Player Taking Damage/Healing");
            Player player = new Player("Combatant", 50, 5, 10);
            try
            {
                player.TakeDamage(10);
                Assert(player.Health == 40, "Took 10 damage (50 -> 40).");
                player.TakeDamage(100);
                Assert(player.Health == 0, "Health clamped at 0 after lethal damage (40 -> 0).");
                Assert(!player.IsAlive, "Player IsAlive is false after lethal damage.");
                player.TakeDamage(5);
                Assert(player.Health == 0, "Taking damage when dead has no effect (remains 0).");

                Player playerHeal = new Player("Healer", 50, 5, 10);
                playerHeal.TakeDamage(30);
                Assert(playerHeal.Heal(15) == 15, "Heal(15) returned 15.");
                Assert(playerHeal.Health == 35, "Health is 35 after healing (20 + 15).");
                Assert(playerHeal.Heal(20) == 15, "Heal(20) returned 15 (healing capped at max).");
                Assert(playerHeal.Health == 50, "Health is 50 (max) after healing past max (35 + 15).");
                Assert(playerHeal.Heal(10) == 0, "Heal(10) returned 0 (healing when full).");
                Assert(playerHeal.Health == 50, "Health remains 50 when healing at full.");

                player.Heal(10);
                Assert(player.Health == 0, "Healing when dead has no effect.");
                Assert(!player.IsAlive, "Player remains dead after attempting heal.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Monster Taking Damage");
            Goblin goblin = new Goblin();
            try
            {
                int initialGoblinHealth = goblin.Health;
                goblin.TakeDamage(10);
                Assert(goblin.Health == initialGoblinHealth - 10, $"Goblin took 10 damage ({initialGoblinHealth} -> {initialGoblinHealth - 10}).");
                goblin.TakeDamage(50);
                Assert(goblin.Health == 0, "Goblin health clamped at 0 after lethal damage.");
                Assert(!goblin.IsAlive, "Goblin IsAlive is false after lethal damage.");
                goblin.TakeDamage(5);
                Assert(goblin.Health == 0, "Goblin health remains 0 when damaged while dead.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Player Attack");
            Player attacker = new Player("Attacker", 100, 5, 10);
            Goblin target = new Goblin();
            Weapon sword = new Weapon("Sword", "S", 8);
            attacker.Inventory.AddItem(sword);
            attacker.EquipWeapon(sword);

            try
            {
                int initialHealth = target.Health;
                int expectedDamage = attacker.Damage + attacker.EquippedWeapon.Damage;
                attacker.Attack(target);
                Assert(target.Health == initialHealth - expectedDamage, $"Target health reduced by player+weapon damage (expected {initialHealth - expectedDamage}, got {target.Health}).");

                target = new Goblin();
                initialHealth = target.Health;
                Weapon currentWeapon = attacker.EquippedWeapon;
                attacker.EquipWeapon(null);

                Assert(attacker.EquippedWeapon.Name == "Fists", "Unequipping results in Fists.");
                Assert(attacker.Inventory.GetAllItems().Contains(currentWeapon), "Unequipped weapon returned to inventory.");

                expectedDamage = attacker.Damage + attacker.EquippedWeapon.Damage;
                attacker.Attack(target);
                Assert(target.Health == initialHealth - expectedDamage, $"Target health reduced by base+fists damage (expected {initialHealth - expectedDamage}, got {target.Health}).");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Monster Attack (Basic Check - Damage Varies)");
            Player playerTarget = new Player("Target", 100, 1, 10);
            Goblin monsterAttacker = new Goblin();
            Ogre ogreAttacker = new Ogre();
            try
            {
                int initialPlayerHealthGoblin = playerTarget.Health;
                monsterAttacker.Attack(playerTarget);
                Assert(playerTarget.Health < initialPlayerHealthGoblin, "Player health decreased after Goblin attack.");
                PassTest("Goblin attacked, player health decreased (random damage applied).");

                int initialPlayerHealthOgre = playerTarget.Health;
                ogreAttacker.Attack(playerTarget);
                Assert(playerTarget.Health < initialPlayerHealthOgre, "Player health decreased further after Ogre attack.");
                PassTest("Ogre attacked, player health decreased (random damage applied).");
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestLevelingSystem
        /// </summary>
        private void TestLevelingSystem()
        {
            StartTest("Player Levelling Up");
            Player player = new Player("LeveleR", 100, 5, 10);
            try
            {
                int initialMaxHp = player.MaxHealth;
                int initialDmg = player.Damage;

                player.AddExperience(50);
                Assert(player.Level == 1, "Gained 50 XP, still Level 1.");
                Assert(player.ExperiencePoints == 50, "XP is 50.");

                player.AddExperience(75);
                Assert(player.Level == 2, "Leveled up to 2 (125 >= 100).");
                Assert(player.ExperiencePoints == 25, $"Remaining XP correct after level up (expected 25, got {player.ExperiencePoints}).");
                Assert(player.Health == player.MaxHealth, "Health fully restored on level up.");
                Assert(player.MaxHealth > initialMaxHp, $"Max health increased (was {initialMaxHp}, now {player.MaxHealth}).");
                Assert(player.Damage > initialDmg, $"Base damage increased (was {initialDmg}, now {player.Damage}).");

                int xpForLevel3 = 150;
                player.AddExperience(xpForLevel3 - player.ExperiencePoints);
                Assert(player.Level == 3, "Leveled up to 3.");
                Assert(player.ExperiencePoints == 0, "XP is exactly 0 after reaching level 3 threshold.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestRoomAndMapBasics
        /// </summary>
        private void TestRoomAndMapBasics()
        {
            StartTest("Room Creation and Content");
            Room room = new Room("Test Room", new Point(1, 1), RoomType.Safe, 1);
            Monster goblin = new Goblin();
            Item potion = new Potion("Potion", "P", 10);
            try
            {
                room.AddMonster(goblin);
                Assert(room.MonstersInRoom.Count == 1 && room.MonstersInRoom[0] == goblin, "Monster added to room.");
                room.AddItem(potion);
                Assert(room.ItemsInRoom.Count == 1 && room.ItemsInRoom[0] == potion, "Item added to room.");
                room.RemoveItem(potion);
                Assert(room.ItemsInRoom.Count == 0, "Item removed from room.");
                room.RemoveMonster(goblin);
                Assert(room.MonstersInRoom.Count == 0, "Monster removed from room.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("GameMap Generation (Basic Checks)");
            GameMap map = new GameMap();
            try
            {
                map.GenerateMap(2);
                Assert(map.StartRoom != null, "Start room was created.");
                Assert(map.StartRoom.Coordinates == new Point(0, 0), "Start room is at (0,0).");
                Assert(map.GetRoom(0, 0) == map.StartRoom, "Can get start room by coordinates.");

                FieldInfo roomsField = typeof(GameMap).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);
                var rooms = roomsField?.GetValue(map) as Dictionary<Point, Room>;
                Assert(rooms != null, "Internal _rooms dictionary exists.");

                Assert(rooms.Count >= 10, $"Map has a reasonable number of rooms (found {rooms.Count}).");

                bool exitFound = rooms.Values.Any(r => r.IsExitRoom);
                Assert(exitFound, "An exit room exists on the map.");
                Room exitRoom = rooms.Values.FirstOrDefault(r => r.IsExitRoom);
                Assert(exitRoom != null, "Exit room object retrieved.");
                Assert(exitRoom.MonstersInRoom.Any(m => m is Dragon), "Exit room contains a Dragon.");
                Assert(exitRoom.GeneratedType == RoomType.Exit, "Exit room has correct type.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestInteractionLogic
        /// </summary>
        private void TestInteractionLogic()
        {
            StartTest("Player Pickup Item");
            Player player = new Player("Picker", 100, 5, 5);
            Room room = new Room("Item Room", new Point(1, 0), RoomType.Safe, 1);
            Potion potion = new Potion("DropPotion", "P", 10);
            Key key = new Key("DropKey", "DK", 99);
            room.AddItem(potion);
            room.AddItem(key);

            try
            {
                Assert(room.ItemsInRoom.Count == 2, "Items initially in room (potion, key).");
                Assert(player.Inventory.GetAllItems().Count == 0, "Inventory initially empty (total items).");

                player.PickUpItem(potion, room);
                Assert(room.ItemsInRoom.Count == 1, "Potion removed from room after pickup.");
                Assert(player.Inventory.Count == 1, "Inventory non-key count is 1 after pickup.");
                Assert(player.Inventory.GetAllItems().Count == 1, "Inventory total count is 1 after pickup.");
                Assert(player.Inventory.GetItemByName("DropPotion") == potion, "Correct item (DropPotion) found in inventory.");

                player.PickUpItem(key, room);
                Assert(room.ItemsInRoom.Count == 0, "Key removed from room after pickup.");
                Assert(player.Inventory.Count == 1, "Inventory non-key count remains 1 after key pickup.");
                Assert(player.Inventory.GetAllItems().Count == 2, "Inventory total count is 2 after key pickup.");
                Assert(player.Inventory.GetItemByName("DropKey") == key, "Correct item (DropKey) found in inventory.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Player Use Item");
            Player playerUser = new Player("User", 50, 5, 5);
            Potion usePotion = new Potion("UsePotion", "P", 20);
            Food useFood = new Food("UseFood", "F", 10);
            Weapon useSword = new Weapon("CantUseSword", "S", 5);
            playerUser.Inventory.AddItem(usePotion);
            playerUser.Inventory.AddItem(useFood);
            playerUser.Inventory.AddItem(useSword);
            playerUser.TakeDamage(30);

            try
            {
                IUseable itemToUse = playerUser.Inventory.GetUseableItems().FirstOrDefault(i => i is Potion && ((Item)i).Name == "UsePotion");
                Assert(itemToUse != null, "Found usable potion (UsePotion) in inventory.");
                itemToUse.Use(playerUser);
                Assert(playerUser.Health == 40, $"Health is 40 after using potion (was 20, healed 20). Got {playerUser.Health}");
                Assert(playerUser.Inventory.GetItemByName("UsePotion") == null, "Potion removed from inventory after use.");

                itemToUse = playerUser.Inventory.GetUseableItems().FirstOrDefault(i => i is Food && ((Item)i).Name == "UseFood");
                Assert(itemToUse != null, "Found usable food (UseFood) in inventory.");
                itemToUse.Use(playerUser);
                Assert(playerUser.Health == 50, $"Health is 50 after using food (was 40, healed 10). Got {playerUser.Health}");
                Assert(playerUser.Inventory.GetItemByName("UseFood") == null, "Food removed from inventory after use.");

                Item nonUsable = playerUser.Inventory.GetItemByName("CantUseSword");
                Assert(nonUsable != null, "Found weapon (CantUseSword).");
                Assert(!(nonUsable is IUseable), "Weapon is not IUseable.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Player Equip Weapon");
            Player playerEquipper = new Player("Equipper", 100, 5, 5);
            Weapon w1 = new Weapon("Iron Sword", "IS", 6);
            Weapon w2 = new Weapon("Steel Axe", "SA", 9);
            playerEquipper.Inventory.AddItem(w1);
            playerEquipper.Inventory.AddItem(w2);

            try
            {
                playerEquipper.EquipWeapon(w1);
                Assert(playerEquipper.EquippedWeapon == w1, "Equipped Iron Sword.");
                Assert(playerEquipper.Inventory.GetAllItems().Count == 1, "Inventory has 1 item after equipping (Steel Axe remains).");
                Assert(playerEquipper.Inventory.GetItemByName("Steel Axe") == w2, "Steel Axe still in inventory.");

                playerEquipper.EquipWeapon(w2);
                Assert(playerEquipper.EquippedWeapon == w2, "Equipped Steel Axe.");
                Assert(playerEquipper.Inventory.GetAllItems().Count == 1, "Inventory still has 1 item (Iron Sword returned).");
                Assert(playerEquipper.Inventory.GetItemByName("Iron Sword") == w1, "Iron Sword put back into inventory.");

                playerEquipper.EquipWeapon(null);
                Assert(playerEquipper.EquippedWeapon.Name == "Fists", "Equipped Fists after unequipping weapon.");
                Assert(playerEquipper.Inventory.GetAllItems().Count == 2, "Inventory has 2 items (both weapons back).");
                Assert(playerEquipper.Inventory.GetItemByName("Steel Axe") == w2, "Steel Axe put back into inventory.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestPuzzleAndLockLogic
        /// </summary>
        private void TestPuzzleAndLockLogic()
        {
            StartTest("Puzzle Room Setup and Key Reward");
            Room puzzleRoom = new Room("PuzzleDesc", new Point(1, 1), RoomType.Puzzle, 1);
            Player playerPuzzler = new Player("Puzzler", 100, 5, 5);
            try
            {
                Assert(puzzleRoom.IsPuzzleActive, "Puzzle is active on creation.");
                Assert(puzzleRoom.PuzzleKeyId != -1, "Puzzle has a valid Key ID assigned.");
                int keyId = puzzleRoom.PuzzleKeyId;

                puzzleRoom.CompletePuzzle(playerPuzzler);
                Assert(!puzzleRoom.IsPuzzleActive, "Puzzle is inactive after completion.");
                Assert(puzzleRoom.ItemsInRoom.Count >= 1, "At least one item added after puzzle completion.");

                Item reward = puzzleRoom.ItemsInRoom.FirstOrDefault(i => i is Key);
                Assert(reward != null, "Key reward exists in room items.");
                Assert(reward is Key, "Reward item is a Key.");
                Assert(((Key)reward).KeyId == keyId, "Reward key ID matches puzzle ID.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Locked Room and Key Usage (Simulation)");
            GameMap map = new GameMap();
            Room start = new Room("Start", new Point(0, 0), RoomType.Safe, 0);
            Room locked = new Room("Locked Room", new Point(0, 1), RoomType.Monster, 1);

            var mapRooms = new Dictionary<Point, Room> { { start.Coordinates, start }, { locked.Coordinates, locked } };
            FieldInfo roomsField = typeof(GameMap).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);
            roomsField?.SetValue(map, mapRooms);
            typeof(GameMap).GetProperty("StartRoom").SetValue(map, start, null);

            Player playerUnlocker = new Player("Unlocker", 100, 5, 5);
            Key matchingKey = new Key("Door Key", "Opens room (0,1)", 123);
            Key wrongKey = new Key("Wrong Key", "Opens something else", 456);

            locked.IsLocked = true;
            locked.RequiredKeyId = 123;
            start.AddExit(Direction.Forward, locked);
            locked.AddExit(Direction.Back, start);

            Game gameSimulator = new Game();
            typeof(Game).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, playerUnlocker);
            typeof(Game).GetField("_gameMap", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, map);
            typeof(Game).GetField("_currentRoom", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, start);
            typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, start.Coordinates);

            try
            {
                Assert(locked.IsLocked, "Room is initially locked.");

                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward });
                Assert(locked.IsLocked, "Room remains locked without any key.");
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == start.Coordinates, "Player position unchanged without key.");

                playerUnlocker.Inventory.AddItem(wrongKey);
                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward });
                Assert(locked.IsLocked, "Room remains locked with wrong key.");
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == start.Coordinates, "Player position unchanged with wrong key.");
                playerUnlocker.Inventory.RemoveItem(wrongKey);

                playerUnlocker.Inventory.AddItem(matchingKey);
                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward });
                Assert(!locked.IsLocked, "Room is unlocked after moving with correct key.");
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == locked.Coordinates, "Player position updated after unlocking.");
                Assert(playerUnlocker.Inventory.GetItemByName(matchingKey.Name) == matchingKey, "Key still exists in inventory after use.");

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The TestMonsterAI
        /// </summary>
        private void TestMonsterAI()
        {
            StartTest("Monster AI - Dragon No Flee");
            Dragon dragon = new Dragon();
            try
            {
                Assert(!dragon.CanFlee, "Dragon CanFlee property is false.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }

            StartTest("Monster AI - Goblin Can Flee");
            Goblin goblin = new Goblin();
            try
            {
                Assert(goblin.CanFlee, "Goblin CanFlee property is true by default.");
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); }
        }

        /// <summary>
        /// The StartTest
        /// </summary>
        /// <param name="testName">The testName<see cref="string"/></param>
        private void StartTest(string testName)
        {
            Log($"\nStarting Test: {testName}...");
            Console.WriteLine($"Starting Test: {testName}...");
        }

        /// <summary>
        /// The PassTest
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void PassTest(string message = "Passed.")
        {
            Log($"  [PASS] {message}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [PASS] {message}");
            Console.ForegroundColor = ConsoleColor.White;
            _testsPassed++;
        }

        /// <summary>
        /// The FailTest
        /// </summary>
        /// <param name="reason">The reason<see cref="string"/></param>
        private void FailTest(string reason)
        {
            Log($"  [FAIL] {reason}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FAIL] {reason}");
            Console.ForegroundColor = ConsoleColor.White;
            _testsFailed++;
        }

        /// <summary>
        /// The Assert
        /// </summary>
        /// <param name="condition">The condition<see cref="bool"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        private void Assert(bool condition, string description)
        {
            string outcome = condition ? "PASS" : "FAIL";
            Log($"    Assert: {description} - {outcome}");
            try
            {
                if (!condition)
                {
                    Log($"      Assertion Failed Detail: {description}");
                    throw new TestAssertionException($"Assertion Failed: {description}");
                }
            }
            catch (TestAssertionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log($"    Assertion Error during check '{description}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// The Log
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void Log(string message)
        {
            _logWriter?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}");
            _logWriter?.Flush();
        }

        /// <summary>
        /// Defines the <see cref="TestAssertionException" />
        /// </summary>
        private class TestAssertionException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestAssertionException"/> class.
            /// </summary>
            /// <param name="message">The message<see cref="string"/></param>
            public TestAssertionException(string message) : base(message)
            {
            }
        }
    }
}
