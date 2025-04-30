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
        /// Initialises a new instance of the <see cref="DungeonTester"/> class.
        /// </summary>
        /// <param name="logFileName">The logFileName<see cref="string"/></param>
        public DungeonTester(string logFileName = "test_log.txt")
        {
            _logFilePath = logFileName;
        }

        /// <summary>
        /// The RunAllTests
        /// </summary>
        public void RunAllTests() // This method runs all the tests and logs the results to a file.
        {
            try
            {
                using (_logWriter = new StreamWriter(_logFilePath, append: true)) 
                {
                    Log($"\n--- Starting Test Run: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---"); // Log the start time of the test run.

                    TestPlayerCreationAndStats(); // Run tests for player creation and stats.
                    TestItemCreation(); // Run tests for item creation.
                    TestInventoryManagement(); // Run tests for inventory management.
                    TestCombatSystem(); // Run tests for the combat system.
                    TestLevelingSystem(); // Run tests for the leveling system.
                    TestRoomAndMapBasics(); // Run tests for room and map basics.
                    TestInteractionLogic(); // Run tests for interaction logic.
                    TestPuzzleAndLockLogic(); // Run tests for puzzle and lock logic.
                    TestMonsterAI(); // Run tests for monster AI.
                    Log($"\n--- Test Run Finished: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---"); // Log the end time of the test run.
                    Log($"Result: {_testsPassed} Passed, {_testsFailed} Failed."); // Log the results of the test run.

                    Console.WriteLine($"\nTest run complete. Results logged to '{_logFilePath}'.");
                    Console.WriteLine($"Passed: {_testsPassed}, Failed: {_testsFailed}");
                }
            }
            catch (Exception ex) // Handle any exceptions that occur during the test run.
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
            StartTest("Player Creation and Stats"); // This method tests the creation of a player and checks their initial stats.
            Player player = null; // Initialise player to null.
            try
            {
                player = new Player("Tester", 100, 5, 15); // Create a new player with name "Tester", 100 health, 5 damage, and 15 inventory capacity.
                Assert(player.Name == "Tester", "Player name set correctly."); // Check if the player's name is set correctly.
                Assert(player.Health == 100, "Player initial health correct."); // Check if the player's initial health is correct.
                Assert(player.MaxHealth == 100, "Player initial max health correct."); // Check if the player's initial max health is correct.
                Assert(player.Damage == 5, "Player base damage correct."); // Check if the player's base damage is correct.
                Assert(player.Level == 1, "Player initial level correct."); // Check if the player's initial level is correct.
                Assert(player.ExperiencePoints == 0, "Player initial XP correct."); // Check if the player's initial experience points are correct.
                Assert(player.Inventory.Capacity == 15, "Player inventory capacity correct."); // Check if the player's inventory capacity is correct.
                Assert(player.EquippedWeapon != null && player.EquippedWeapon.Name == "Fists", "Player starts with Fists equipped."); // Check if the player starts with fists equipped.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestItemCreation
        /// </summary>
        private void TestItemCreation()
        {
            StartTest("Item Creation"); // This method tests the creation of various items and checks their properties.
            try
            {
                Weapon sword = new Weapon("Iron Sword", "A basic sword.", 8); // Create a new weapon with name "Iron Sword", description "A basic sword.", and 8 damage.
                Assert(sword.Name == "Iron Sword" && sword.Damage == 8 && !sword.IsUsable, "Weapon created correctly."); // Check if the weapon is created correctly.

                Potion potion = new Potion("Healing Potion", "Heals.", 30); // Create a new potion with name "Healing Potion", description "Heals.", and 30 heal amount.
                Assert(potion.Name == "Healing Potion" && potion.HealAmount == 30 && potion.IsUsable, "Potion created correctly."); // Check if the potion is created correctly.

                Food bread = new Food("Bread", "Sustenance.", 15); // Create a new food item with name "Bread", description "Sustenance.", and 15 heal amount.
                Assert(bread.Name == "Bread" && bread.HealAmount == 15 && bread.IsUsable, "Food created correctly."); // Check if the food item is created correctly.

                Key key = new Key("Rusty Key", "Opens something.", 123); // Create a new key with name "Rusty Key", description "Opens something.", and ID 123.
                Assert(key.Name == "Rusty Key" && key.KeyId == 123 && !key.IsUsable, "Key created correctly."); // Check if the key is created correctly.

                MiscItem junk = new MiscItem("Odd Stone", "A smooth stone."); // Create a new miscellaneous item with name "Odd Stone" and description "A smooth stone.".
                Assert(junk.Name == "Odd Stone" && !junk.IsUsable, "Misc Item created correctly."); // Check if the miscellaneous item is created correctly.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Item Creation - Invalid Arguments"); // This method tests the creation of items with invalid arguments and checks if exceptions are thrown.
            try // Try to create items with invalid arguments.
            {
                try // Create a weapon with negative damage.
                {
                    new Weapon("Bad", "Neg Dmg", -5); // This should throw an exception.
                    FailTest("Negative weapon damage did not throw expected ArgumentOutOfRangeException."); // Fail the test if no exception is thrown.
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative weapon damage correctly threw ArgumentOutOfRangeException."); } // Check if the exception is thrown.
                catch (Exception ex) { FailTest($"Incorrect exception type for negative damage: {ex.GetType()}. Expected ArgumentOutOfRangeException."); } // Handle any other exceptions.

                try // Create a potion with negative heal amount.
                {
                    new Potion("Bad", "Neg Heal", -10); // This should throw an exception.
                    FailTest("Negative potion heal did not throw expected ArgumentOutOfRangeException."); // Fail the test if no exception is thrown.
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative potion heal correctly threw ArgumentOutOfRangeException."); } // Check if the exception is thrown.
                catch (Exception ex) { FailTest($"Incorrect exception type for negative heal: {ex.GetType()}. Expected ArgumentOutOfRangeException."); } // Handle any other exceptions.

                try // Create a food item with negative heal amount.
                {
                    new Food("Bad", "Neg Food Heal", -1); // This should throw an exception.
                    FailTest("Negative food heal did not throw expected ArgumentOutOfRangeException."); // Fail the test if no exception is thrown.
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative food heal correctly threw ArgumentOutOfRangeException."); } // Check if the exception is thrown.
                catch (Exception ex) { FailTest($"Incorrect exception type for negative food heal: {ex.GetType()}. Expected ArgumentOutOfRangeException."); } // Handle any other exceptions.

                try // Try to create a key with negative ID.
                {
                    new Key("Bad", "Neg Key ID", -1); // This should throw an exception.
                    FailTest("Negative key ID did not throw expected ArgumentOutOfRangeException."); // Fail the test if no exception is thrown.
                }
                catch (ArgumentOutOfRangeException) { PassTest("Negative key ID correctly threw ArgumentOutOfRangeException."); } // Check if the exception is thrown.
                catch (Exception ex) { FailTest($"Incorrect exception type for negative key ID: {ex.GetType()}. Expected ArgumentOutOfRangeException."); } // Handle any other exceptions.
            }
            catch (Exception ex) { FailTest($"General exception during invalid argument tests: {ex.Message}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestInventoryManagement
        /// </summary>
        private void TestInventoryManagement() // This method tests the inventory management system, including adding and removing items, checking capacity, and using LINQ methods.
        {
            StartTest("Inventory Add/Remove/Capacity (Keys Ignore Capacity)"); // This method tests the addition and removal of items in the inventory, including checking capacity and key handling.
            Player player = new Player("InvTester", 100, 5, 3); // Create a new player with name "InvTester", 100 health, 5 damage, and 3 inventory capacity.
            Weapon sword = new Weapon("Sword", "S", 5); // Create a new weapon with name "Sword", description "S", and 5 damage.
            Potion potion = new Potion("Potion", "P", 10); // Create a new potion with name "Potion", description "P", and 10 heal amount.
            Food bread = new Food("Bread", "B", 5); // Create a new food item with name "Bread", description "B", and 5 heal amount.
            Key key = new Key("Key", "K", 1); // Create a new key with name "Key", description "K", and ID 1.

            try // Add items to the inventory and check capacity.
            {
                Assert(player.Inventory.AddItem(sword), "Added first item (Sword)."); // Check if the first item is added successfully.
                Assert(player.Inventory.Count == 1, "Non-key count is 1."); // Check if the non-key count is 1.
                Assert(player.Inventory.AddItem(potion), "Added second item (Potion)."); // Check if the second item is added successfully.
                Assert(player.Inventory.Count == 2, "Non-key count is 2."); // Check if the non-key count is 2.
                Assert(player.Inventory.AddItem(bread), "Added third item (Bread)."); // Check if the third item is added successfully.
                Assert(player.Inventory.Count == 3, "Non-key count is 3."); // Check if the non-key count is 3.
                Assert(player.Inventory.IsFull, "Inventory is full (for non-keys)."); // Check if the inventory is full for non-key items.
                Assert(player.Inventory.AddItem(key), "Successfully added item (Key) to full non-key inventory."); // Check if the key is added successfully.
                Assert(player.Inventory.Count == 3, "Non-key count remains 3 after adding key."); // Check if the non-key count remains 3.
                Assert(player.Inventory.GetAllItems().Count == 4, "Total item count (incl. key) is now 4."); // Check if the total item count is 4.
                Assert(player.Inventory.GetItemByName("Key") == key, "Key exists in inventory after adding."); // Check if the key exists in the inventory.
                Assert(player.Inventory.RemoveItem(potion), "Removed potion."); // Check if the potion is removed successfully.
                Assert(player.Inventory.Count == 2, "Non-key count is 2 after removal."); // Check if the non-key count is 2.
                Assert(player.Inventory.GetAllItems().Count == 3, "Total item count is 3 after removal."); // Check if the total item count is 3.
                Assert(!player.Inventory.IsFull, "Inventory is not full (for non-keys) after removal."); // Check if the inventory is not full for non-key items.
                Assert(player.Inventory.GetItemByName("Sword") == sword, "Found sword by name."); // Check if the sword is found by name.
                Assert(player.Inventory.GetItemByName("Potion") == null, "Did not find removed potion."); // Check if the removed potion is not found.
                Assert(player.Inventory.GetItemByName("Key") == key, "Key still exists after removing potion."); // Check if the key still exists after removing the potion.
                Assert(!player.Inventory.RemoveItem(potion), "Failed to remove already removed potion."); // Check if the removal of the already removed potion fails.
                Assert(player.Inventory.RemoveItem(key), "Removed key."); // Check if the key is removed successfully.
                Assert(player.Inventory.Count == 2, "Non-key count remains 2 after removing key."); // Check if the non-key count remains 2.
                Assert(player.Inventory.GetAllItems().Count == 2, "Total item count is 2 after removing key."); // Check if the total item count is 2.
                Assert(player.Inventory.GetItemByName("Key") == null, "Key no longer exists after removal."); // Check if the key no longer exists after removal.
                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Inventory LINQ Methods"); // This method tests the LINQ methods for filtering and sorting items in the inventory.
            Player playerLinq = new Player("LinqTester", 100, 5, 10); // Create a new player with name "LinqTester", 100 health, 5 damage, and 10 inventory capacity.
            Weapon swordLinq = new Weapon("Sword", "S", 5); // Create a new weapon with name "Sword", description "S", and 5 damage.
            Weapon daggerLinq = new Weapon("Dagger", "D", 4); // Create a new weapon with name "Dagger", description "D", and 4 damage.
            Weapon axeLinq = new Weapon("Axe", "A", 7); // Create a new weapon with name "Axe", description "A", and 7 damage.
            Potion potionLinq = new Potion("Potion", "P", 10); // Create a new potion with name "Potion", description "P", and 10 heal amount.
            Food breadLinq = new Food("Bread", "B", 5); // Create a new food item with name "Bread", description "B", and 5 heal amount.
            Key keyLinq = new Key("Keycard", "KC", 101); // Create a new key with name "Keycard", description "KC", and ID 101.

            playerLinq.Inventory.AddItem(swordLinq); // Add the sword to the inventory.
            playerLinq.Inventory.AddItem(daggerLinq); // Add the dagger to the inventory.
            playerLinq.Inventory.AddItem(axeLinq); // Add the axe to the inventory.
            playerLinq.Inventory.AddItem(potionLinq); // Add the potion to the inventory.
            playerLinq.Inventory.AddItem(breadLinq); // Add the bread to the inventory.
            playerLinq.Inventory.AddItem(keyLinq); // Add the key to the inventory.

            try // Try to check the inventory contents and properties.
            {
                Assert(playerLinq.Inventory.GetWeapons().Count == 3, "Found 3 weapons."); // Check if there are 3 weapons in the inventory.
                Assert(playerLinq.Inventory.GetPotions().Count == 1, "Found 1 potion."); // Check if there is 1 potion in the inventory.
                Assert(playerLinq.Inventory.GetFood().Count == 1, "Found 1 food."); // Check if there is 1 food item in the inventory.
                Assert(playerLinq.Inventory.GetUseableItems().Count == 2, "Found 2 useable items (potion + food)."); // Check if there are 2 useable items in the inventory.
                Assert(playerLinq.Inventory.GetStrongestWeapon() == axeLinq, "Found Axe as strongest weapon."); // Check if the strongest weapon is the axe.
                Assert(playerLinq.Inventory.GetAllItems().Count == 6, "GetAllItems includes key (total 6)."); // Check if the total item count is 6.

                var sorted = playerLinq.Inventory.GetItemsSortedByName(); // Sort the items by name.
                Assert(sorted.Count == 6, "Sorted list has 6 items."); // Check if the sortedd list has 6 items.
                Assert(sorted[0].Name == "Axe", "Sorted list starts with Axe."); // Check if the first index is correct.
                Assert(sorted[1].Name == "Bread", "Sorted list second is Bread."); // Check if the second index is correct.
                Assert(sorted[2].Name == "Dagger", "Sorted list third is Dagger."); // Check if the third index is correct.
                Assert(sorted[3].Name == "Keycard", "Sorted list fourth is Keycard."); // Check if the fourth index is correct.
                Assert(sorted[4].Name == "Potion", "Sorted list fifth is Potion."); // Check if the fifth index is correct.
                Assert(sorted[5].Name == "Sword", "Sorted list last is Sword."); // Check if the sixth index is correct.

                PassTest(); // Pass the test.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestCombatSystem
        /// </summary>
        private void TestCombatSystem()
        {
            StartTest("Player Taking Damage/Healing"); // This method tests the player's ability to take damage and heal, including edge cases for health limits.
            Player player = new Player("Combatant", 50, 5, 10); // Create a new player with name "Combatant", 50 health, 5 damage, and 10 inventory capacity.
            try // Take damage and heal the player.
            {
                player.TakeDamage(10); // Take 10 damage.
                Assert(player.Health == 40, "Took 10 damage (50 -> 40)."); // Check if the health is correct.
                player.TakeDamage(100); // Take lethal damage.
                Assert(player.Health == 0, "Health clamped at 0 after lethal damage (40 -> 0)."); // Check if the health is clamped at 0.
                Assert(!player.IsAlive, "Player IsAlive is false after lethal damage."); // Check if the player is dead.
                player.TakeDamage(5); // Take more damage while dead.
                Assert(player.Health == 0, "Taking damage when dead has no effect (remains 0)."); // Check if the health remains 0.
                Player playerHeal = new Player("Healer", 50, 5, 10); // Create a new player with name "Healer", 50 health, 5 damage, and 10 inventory capacity.
                playerHeal.TakeDamage(30); // Take 30 damage.
                Assert(playerHeal.Heal(15) == 15, "Heal(15) returned 15."); // Check if the heal amount is correct.
                Assert(playerHeal.Health == 35, "Health is 35 after healing (20 + 15)."); // Check if the health is correct.
                Assert(playerHeal.Heal(20) == 15, "Heal(20) returned 15 (healing capped at max)."); // Check if the heal amount is capped at max.
                Assert(playerHeal.Health == 50, "Health is 50 (max) after healing past max (35 + 15)."); // Check if the health is capped at max.
                Assert(playerHeal.Heal(10) == 0, "Heal(10) returned 0 (healing when full)."); // Check if the heal amount is 0.
                Assert(playerHeal.Health == 50, "Health remains 50 when healing at full."); // Check if the health remains 50.
                player.Heal(10); // Attempt to heal while dead.
                Assert(player.Health == 0, "Healing when dead has no effect."); // Check if the health remains 0.
                Assert(!player.IsAlive, "Player remains dead after attempting heal."); // Check if the player is still dead.

                PassTest();
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Monster Taking Damage"); // This method tests the monster's ability to take damage and heal, including edge cases for health limits.
            Goblin goblin = new Goblin(); // Create a new goblin monster.
            try
            {
                int initialGoblinHealth = goblin.Health; // Get the initial health of the goblin.
                goblin.TakeDamage(10); // Take 10 damage.
                Assert(goblin.Health == initialGoblinHealth - 10, $"Goblin took 10 damage ({initialGoblinHealth} -> {initialGoblinHealth - 10})."); // Check if the health is correct.
                goblin.TakeDamage(50); // Deals lethal damage.
                Assert(goblin.Health == 0, "Goblin health clamped at 0 after lethal damage."); // Check if the health is clamped at 0.
                Assert(!goblin.IsAlive, "Goblin IsAlive is false after lethal damage."); // Check if the goblin is dead.
                goblin.TakeDamage(5); // Take more damage while dead.
                Assert(goblin.Health == 0, "Goblin health remains 0 when damaged while dead."); // Check if the health remains 0.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Player Attack"); // This method tests the player's attack functionality, including equipping and unequipping weapons.
            Player attacker = new Player("Attacker", 100, 5, 10); // Create a new player with name "Attacker", 100 health, 5 damage, and 10 inventory capacity.
            Goblin target = new Goblin(); // Create a new goblin monster.
            Weapon sword = new Weapon("Sword", "S", 8); // Create a new weapon with name "Sword", description "S", and 8 damage.
            attacker.Inventory.AddItem(sword); // Add the sword to the inventory.
            attacker.EquipWeapon(sword); // Equip the sword.

            try // Try to attack the target with the equipped weapon.
            {
                int initialHealth = target.Health; // Get the initial health of the target.
                int expectedDamage = attacker.Damage + attacker.EquippedWeapon.Damage; // Calculate the expected damage.
                attacker.Attack(target); // Attack the target.
                Assert(target.Health == initialHealth - expectedDamage, $"Target health reduced by player+weapon damage (expected {initialHealth - expectedDamage}, got {target.Health})."); // Check if the target's health is reduced correctly.

                target = new Goblin(); // Create a new goblin monster.
                initialHealth = target.Health; // Get the initial health of the target.
                Weapon currentWeapon = attacker.EquippedWeapon; // Get the currently equipped weapon.
                attacker.EquipWeapon(null); // Unequip the weapon.

                Assert(attacker.EquippedWeapon.Name == "Fists", "Unequipping results in Fists."); // Check if the equipped weapon is fists.
                Assert(attacker.Inventory.GetAllItems().Contains(currentWeapon), "Unequipped weapon returned to inventory."); // Check if the unequipped weapon is returned to the inventory.

                expectedDamage = attacker.Damage + attacker.EquippedWeapon.Damage; // Calculate the expected damage with fists.
                attacker.Attack(target); // Attack the target with fists.
                Assert(target.Health == initialHealth - expectedDamage, $"Target health reduced by base+fists damage (expected {initialHealth - expectedDamage}, got {target.Health})."); // Check if the target's health is reduced correctly.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Monster Attack (Basic Check - Damage Varies)"); // This method tests the monster's attack functionality, including damage variation.
            Player playerTarget = new Player("Target", 100, 1, 10); // Create a new player with name "Target", 100 health, 1 damage, and 10 inventory capacity.
            Goblin monsterAttacker = new Goblin(); // Create a new goblin monster.
            Ogre ogreAttacker = new Ogre(); // Create a new ogre monster.
            try // Try to attack the player with the goblin and ogre monsters.
            {
                int initialPlayerHealthGoblin = playerTarget.Health; // Get the initial health of the player.
                monsterAttacker.Attack(playerTarget); // Attack the player with the goblin.
                Assert(playerTarget.Health < initialPlayerHealthGoblin, "Player health decreased after Goblin attack."); // Check if the player's health is decreased.
                PassTest("Goblin attacked, player health decreased (random damage applied)."); // Test passed.

                int initialPlayerHealthOgre = playerTarget.Health; // Get the initial health of the player.
                ogreAttacker.Attack(playerTarget); // Attack the player with the ogre.
                Assert(playerTarget.Health < initialPlayerHealthOgre, "Player health decreased further after Ogre attack."); // Check if the player's health is decreased.
                PassTest("Ogre attacked, player health decreased (random damage applied)."); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestLevelingSystem
        /// </summary>
        private void TestLevelingSystem()
        {
            StartTest("Player Levelling Up"); // This method tests the player's leveling system, including experience points and health/damage adjustments.
            Player player = new Player("LeveleR", 100, 5, 10); // Create a new player with name "LeveleR", 100 health, 5 damage, and 10 inventory capacity.
            try // Try to level up the player.
            {
                int initialMaxHp = player.MaxHealth; // Get the initial maximum health of the player.
                int initialDmg = player.Damage; // Get the initial damage of the player.

                player.AddExperience(50); // Add 50 experience points.
                Assert(player.Level == 1, "Gained 50 XP, still Level 1."); // Check if the player is still level 1.
                Assert(player.ExperiencePoints == 50, "XP is 50."); // Check if the experience points are 50.

                player.AddExperience(75); // Add 75 experience points.
                Assert(player.Level == 2, "Leveled up to 2 (125 >= 100)."); // Check if the player leveled up to 2.
                Assert(player.ExperiencePoints == 25, $"Remaining XP correct after level up (expected 25, got {player.ExperiencePoints})."); // Check if the experience points are 25.
                Assert(player.Health == player.MaxHealth, "Health fully restored on level up."); // Check if the health is fully restored.
                Assert(player.MaxHealth > initialMaxHp, $"Max health increased (was {initialMaxHp}, now {player.MaxHealth})."); // Check if the maximum health increased.
                Assert(player.Damage > initialDmg, $"Base damage increased (was {initialDmg}, now {player.Damage})."); // Check if the base damage increased.

                int xpForLevel3 = 150; // Calculate the experience points needed for level 3.
                player.AddExperience(xpForLevel3 - player.ExperiencePoints); // Add experience points to reach level 3.
                Assert(player.Level == 3, "Leveled up to 3."); // Check if the player leveled up to 3.
                Assert(player.ExperiencePoints == 0, "XP is exactly 0 after reaching level 3 threshold."); // Check if the experience points are 0.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestRoomAndMapBasics
        /// </summary>
        private void TestRoomAndMapBasics()
        {
            StartTest("Room Creation and Content"); // This method tests the creation of a room and checks its properties, including adding/removing monsters and items.
            Room room = new Room("Test Room", new Point(1, 1), RoomType.Safe, 1); // Create a new room with name "Test Room", coordinates (1, 1), type "Safe", and ID 1.
            Monster goblin = new Goblin(); // Create a new goblin monster.
            Item potion = new Potion("Potion", "P", 10); // Create a new potion with name "Potion", description "P", and 10 heal amount.
            try
            {
                room.AddMonster(goblin); // Adds the goblin to the room.
                Assert(room.MonstersInRoom.Count == 1 && room.MonstersInRoom[0] == goblin, "Monster added to room."); // Check if the goblin is added to the room.
                room.AddItem(potion); // Adds the potion to the room.
                Assert(room.ItemsInRoom.Count == 1 && room.ItemsInRoom[0] == potion, "Item added to room."); // Check if the potion is added to the room.
                room.RemoveItem(potion); // Removes the potion from the room.
                Assert(room.ItemsInRoom.Count == 0, "Item removed from room."); // Check if the potion is removed from the room.
                room.RemoveMonster(goblin); // Removes the goblin from the room.
                Assert(room.MonstersInRoom.Count == 0, "Monster removed from room."); // Check if the goblin is removed from the room.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("GameMap Generation (Basic Checks)"); // This method tests the generation of a game map and checks its properties, including room count and exit room.
            GameMap map = new GameMap(); // Create a new game map.
            try // Generate the map and check its properties.
            {
                map.GenerateMap(2); // Generate the map with 2 levels.
                Assert(map.StartRoom != null, "Start room was created."); // Check if the start room is created.
                Assert(map.StartRoom.Coordinates == new Point(0, 0), "Start room is at (0,0)."); // Check if the start room is at (0, 0).
                Assert(map.GetRoom(0, 0) == map.StartRoom, "Can get start room by coordinates."); // Check if the start room can be retrieved by coordinates.

                FieldInfo roomsField = typeof(GameMap).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance); // Get the private _rooms field from the GameMap class.
                var rooms = roomsField?.GetValue(map) as Dictionary<Point, Room>; // Get the value of the _rooms field as a dictionary of Point and Room.
                Assert(rooms != null, "Internal _rooms dictionary exists."); // Check if the _rooms dictionary exists.

                Assert(rooms.Count >= 10, $"Map has a reasonable number of rooms (found {rooms.Count})."); // Check if the number of rooms is reasonable.

                bool exitFound = rooms.Values.Any(r => r.IsExitRoom); // Check if there is an exit room in the map.
                Assert(exitFound, "An exit room exists on the map."); // Check if an exit room exists.
                Room exitRoom = rooms.Values.FirstOrDefault(r => r.IsExitRoom); // Get the exit room from the map.
                Assert(exitRoom != null, "Exit room object retrieved."); // Check if the exit room object is retrieved.
                Assert(exitRoom.MonstersInRoom.Any(m => m is Dragon), "Exit room contains a Dragon."); // Check if the exit room contains a dragon monster.
                Assert(exitRoom.GeneratedType == RoomType.Exit, "Exit room has correct type."); // Check if the exit room has the correct type.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestInteractionLogic
        /// </summary>
        private void TestInteractionLogic() // This method tests the interaction logic between players and items, including picking up, using, and equipping items.
        {
            StartTest("Player Pickup Item"); // This method tests the player's ability to pick up items from a room and check inventory contents.
            Player player = new Player("Picker", 100, 5, 5); // Create a new player with name "Picker", 100 health, 5 damage, and 5 inventory capacity.
            Room room = new Room("Item Room", new Point(1, 0), RoomType.Safe, 1); // Create a new room with name "Item Room", coordinates (1, 0), type "Safe", and ID 1.
            Potion potion = new Potion("DropPotion", "P", 10); // Create a new potion with name "DropPotion", description "P", and 10 heal amount.
            Key key = new Key("DropKey", "DK", 99); // Create a new key with name "DropKey", description "DK", and ID 99.
            room.AddItem(potion); // Adds the potion to the room.
            room.AddItem(key); // Adds the key to the room.

            try // Try to pick up items from the room and check inventory contents.
            {
                Assert(room.ItemsInRoom.Count == 2, "Items initially in room (potion, key)."); // Check if there are 2 items in the room.
                Assert(player.Inventory.GetAllItems().Count == 0, "Inventory initially empty (total items)."); // Check if the inventory is empty.

                player.PickUpItem(potion, room); // Pick up the potion from the room.
                Assert(room.ItemsInRoom.Count == 1, "Potion removed from room after pickup."); // Check if the potion is removed from the room.
                Assert(player.Inventory.Count == 1, "Inventory non-key count is 1 after pickup."); // Check if the non-key count is 1.
                Assert(player.Inventory.GetAllItems().Count == 1, "Inventory total count is 1 after pickup."); // Check if the total item count is 1.
                Assert(player.Inventory.GetItemByName("DropPotion") == potion, "Correct item (DropPotion) found in inventory."); // Check if the correct item is found in the inventory.

                player.PickUpItem(key, room); // Pick up the key from the room.
                Assert(room.ItemsInRoom.Count == 0, "Key removed from room after pickup."); // Check if the key is removed from the room.
                Assert(player.Inventory.Count == 1, "Inventory non-key count remains 1 after key pickup."); // Check if the non-key count remains 1.
                Assert(player.Inventory.GetAllItems().Count == 2, "Inventory total count is 2 after key pickup."); // Check if the total item count is 2.
                Assert(player.Inventory.GetItemByName("DropKey") == key, "Correct item (DropKey) found in inventory."); // Check if the correct item is found in the inventory.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Player Use Item"); // This method tests the player's ability to use items from the inventory, including potions and food.
            Player playerUser = new Player("User", 50, 5, 5); // Create a new player with name "User", 50 health, 5 damage, and 5 inventory capacity.
            Potion usePotion = new Potion("UsePotion", "P", 20); // Create a new potion with name "UsePotion", description "P", and 20 heal amount.
            Food useFood = new Food("UseFood", "F", 10); // Create a new food item with name "UseFood", description "F", and 10 heal amount.
            Weapon useSword = new Weapon("CantUseSword", "S", 5); // Create a new weapon with name "CantUseSword", description "S", and 5 damage.
            playerUser.Inventory.AddItem(usePotion); // Adds the potion to the inventory.
            playerUser.Inventory.AddItem(useFood); // Adds the food to the inventory.
            playerUser.Inventory.AddItem(useSword); // Adds the sword to the inventory.
            playerUser.TakeDamage(30); // Take damage to test healing.

            try // Try to use items from the inventory and check health and inventory contents.
            {
                IUseable itemToUse = playerUser.Inventory.GetUseableItems().FirstOrDefault(i => i is Potion && ((Item)i).Name == "UsePotion"); // Find the potion in the inventory.
                Assert(itemToUse != null, "Found usable potion (UsePotion) in inventory."); // Check if the potion is found in the inventory.
                itemToUse.Use(playerUser); // Use the potion.
                Assert(playerUser.Health == 40, $"Health is 40 after using potion (was 20, healed 20). Got {playerUser.Health}"); // Check if the health is correct.
                Assert(playerUser.Inventory.GetItemByName("UsePotion") == null, "Potion removed from inventory after use."); // Check if the potion is removed from the inventory.

                itemToUse = playerUser.Inventory.GetUseableItems().FirstOrDefault(i => i is Food && ((Item)i).Name == "UseFood"); // Find the food in the inventory.
                Assert(itemToUse != null, "Found usable food (UseFood) in inventory."); // Check if the food is found in the inventory.
                itemToUse.Use(playerUser); // Use the food.
                Assert(playerUser.Health == 50, $"Health is 50 after using food (was 40, healed 10). Got {playerUser.Health}"); // Check if the health is correct.
                Assert(playerUser.Inventory.GetItemByName("UseFood") == null, "Food removed from inventory after use."); // Check if the food is removed from the inventory.

                Item nonUsable = playerUser.Inventory.GetItemByName("CantUseSword"); // Find the sword in the inventory.
                Assert(nonUsable != null, "Found weapon (CantUseSword)."); // Check if the sword is found in the inventory.
                Assert(!(nonUsable is IUseable), "Weapon is not IUseable."); // Check if the sword is not usable.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Player Equip Weapon"); // This method tests the player's ability to equip and unequip weapons, including inventory management.
            Player playerEquipper = new Player("Equipper", 100, 5, 5); // Create a new player with name "Equipper", 100 health, 5 damage, and 5 inventory capacity.
            Weapon w1 = new Weapon("Iron Sword", "IS", 6); // Create a new weapon with name "Iron Sword", description "IS", and 6 damage.
            Weapon w2 = new Weapon("Steel Axe", "SA", 9); // Create a new weapon with name "Steel Axe", description "SA", and 9 damage.
            playerEquipper.Inventory.AddItem(w1); // Adds the first weapon to the inventory.
            playerEquipper.Inventory.AddItem(w2); // Adds the second weapon to the inventory.

            try // Try to equip and unequip weapons and check inventory contents.
            {
                playerEquipper.EquipWeapon(w1); // Equip the first weapon.
                Assert(playerEquipper.EquippedWeapon == w1, "Equipped Iron Sword."); // Check if the equipped weapon is the first one.
                Assert(playerEquipper.Inventory.GetAllItems().Count == 1, "Inventory has 1 item after equipping (Steel Axe remains)."); // Check if the inventory has 1 item.
                Assert(playerEquipper.Inventory.GetItemByName("Steel Axe") == w2, "Steel Axe still in inventory."); // Check if the second weapon is still in the inventory.

                playerEquipper.EquipWeapon(w2); // Equip the second weapon.
                Assert(playerEquipper.EquippedWeapon == w2, "Equipped Steel Axe."); // Check if the equipped weapon is the second one.
                Assert(playerEquipper.Inventory.GetAllItems().Count == 1, "Inventory still has 1 item (Iron Sword returned)."); // Check if the inventory has 1 item.
                Assert(playerEquipper.Inventory.GetItemByName("Iron Sword") == w1, "Iron Sword put back into inventory."); // Check if the first weapon is put back into the inventory.

                playerEquipper.EquipWeapon(null); // Unequip the weapon.
                Assert(playerEquipper.EquippedWeapon.Name == "Fists", "Equipped Fists after unequipping weapon."); // Check if the equipped weapon is fists.
                Assert(playerEquipper.Inventory.GetAllItems().Count == 2, "Inventory has 2 items (both weapons back)."); // Check if the inventory has 2 items.
                Assert(playerEquipper.Inventory.GetItemByName("Steel Axe") == w2, "Steel Axe put back into inventory."); // Check if the second weapon is put back into the inventory.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestPuzzleAndLockLogic
        /// </summary>
        private void TestPuzzleAndLockLogic() // This method tests the puzzle and lock logic in the game, including puzzle completion and key usage.
        {
            StartTest("Puzzle Room Setup and Key Reward"); // This method tests the setup of a puzzle room and the reward system for completing the puzzle.
            Room puzzleRoom = new Room("PuzzleDesc", new Point(1, 1), RoomType.Puzzle, 1); // Create a new puzzle room with name "PuzzleDesc", coordinates (1, 1), type "Puzzle", and ID 1.
            Player playerPuzzler = new Player("Puzzler", 100, 5, 5); // Create a new player with name "Puzzler", 100 health, 5 damage, and 5 inventory capacity.
            try // Try to set up the puzzle room and check its properties.
            {
                Assert(puzzleRoom.IsPuzzleActive, "Puzzle is active on creation."); // Check if the puzzle is active.
                Assert(puzzleRoom.PuzzleKeyId != -1, "Puzzle has a valid Key ID assigned."); // Check if the puzzle has a valid key ID.
                int keyId = puzzleRoom.PuzzleKeyId; // Get the key ID of the puzzle.

                puzzleRoom.CompletePuzzle(playerPuzzler); // Complete the puzzle.
                Assert(!puzzleRoom.IsPuzzleActive, "Puzzle is inactive after completion."); // Check if the puzzle is inactive.
                Assert(puzzleRoom.ItemsInRoom.Count >= 1, "At least one item added after puzzle completion."); // Check if at least one item is added after puzzle completion.

                Item reward = puzzleRoom.ItemsInRoom.FirstOrDefault(i => i is Key); // Find the reward item in the room.
                Assert(reward != null, "Key reward exists in room items."); // Check if the reward item exists in the room.
                Assert(reward is Key, "Reward item is a Key."); // Check if the reward item is a key.
                Assert(((Key)reward).KeyId == keyId, "Reward key ID matches puzzle ID."); // Check if the reward key ID matches the puzzle ID.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Locked Room and Key Usage (Simulation)"); // This method tests the locked room and key usage logic, including player movement and inventory management.
            GameMap map = new GameMap(); // Create a new game map.
            Room start = new Room("Start", new Point(0, 0), RoomType.Safe, 0); // Create a new start room with name "Start", coordinates (0, 0), type "Safe", and ID 0.
            Room locked = new Room("Locked Room", new Point(0, 1), RoomType.Monster, 1); // Create a new locked room with name "Locked Room", coordinates (0, 1), type "Monster", and ID 1.

            var mapRooms = new Dictionary<Point, Room> { { start.Coordinates, start }, { locked.Coordinates, locked } }; // Create a dictionary of rooms with their coordinates as keys.
            FieldInfo roomsField = typeof(GameMap).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance); // Get the private _rooms field from the GameMap class.
            roomsField?.SetValue(map, mapRooms); // Set the value of the _rooms field to the dictionary of rooms.
            typeof(GameMap).GetProperty("StartRoom").SetValue(map, start, null); // Set the start room property of the map to the start room.

            Player playerUnlocker = new Player("Unlocker", 100, 5, 5); // Create a new player with name "Unlocker", 100 health, 5 damage, and 5 inventory capacity.
            Key matchingKey = new Key("Door Key", "Opens room (0,1)", 123); // Create a new key with name "Door Key", description "Opens room (0,1)", and ID 123.
            Key wrongKey = new Key("Wrong Key", "Opens something else", 456); // Create a new key with name "Wrong Key", description "Opens something else", and ID 456.

            locked.IsLocked = true; // Set the locked status of the room to true.
            locked.RequiredKeyId = 123; // Set the required key ID of the room to 123.
            start.AddExit(Direction.Forward, locked); // Add an exit from the start room to the locked room.
            locked.AddExit(Direction.Back, start); // Add an exit from the locked room to the start room.

            Game gameSimulator = new Game(); // Create a new game instance.
            typeof(Game).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, playerUnlocker); // Set the player field of the game instance to the player.
            typeof(Game).GetField("_gameMap", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, map); // Set the game map field of the game instance to the map.
            typeof(Game).GetField("_currentRoom", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, start); // Set the current room field of the game instance to the start room.
            typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameSimulator, start.Coordinates); // Set the player position field of the game instance to the start room coordinates.

            try // Try to test the locked room and key usage logic.
            {
                Assert(locked.IsLocked, "Room is initially locked."); // Check if the room is initially locked.

                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward }); // Move the player forward.
                Assert(locked.IsLocked, "Room remains locked without any key."); // Check if the room remains locked.
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == start.Coordinates, "Player position unchanged without key."); // Check if the player position is unchanged.

                playerUnlocker.Inventory.AddItem(wrongKey); // Add the wrong key to the inventory.
                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward }); // Move the player forward.
                Assert(locked.IsLocked, "Room remains locked with wrong key."); // Check if the room remains locked.
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == start.Coordinates, "Player position unchanged with wrong key."); // Check if the player position is unchanged.
                playerUnlocker.Inventory.RemoveItem(wrongKey); // Remove the wrong key from the inventory.

                playerUnlocker.Inventory.AddItem(matchingKey); // Add the matching key to the inventory.
                gameSimulator.GetType().GetMethod("MovePlayer", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameSimulator, new object[] { Direction.Forward }); // Move the player forward.
                Assert(!locked.IsLocked, "Room is unlocked after moving with correct key."); // Check if the room is unlocked.
                Assert(((Point)typeof(Game).GetField("_playerPosition", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameSimulator)) == locked.Coordinates, "Player position updated after unlocking."); // Check if the player position is updated.
                Assert(playerUnlocker.Inventory.GetItemByName(matchingKey.Name) == matchingKey, "Key still exists in inventory after use."); // Check if the key still exists in the inventory.

                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The TestMonsterAI
        /// </summary>
        private void TestMonsterAI() // This method tests the monster AI behavior, including fleeing and attacking.
        {
            StartTest("Monster AI - Dragon No Flee"); // This method tests the monster AI behavior for the Dragon class, including the CanFlee property.
            Dragon dragon = new Dragon(); // Create a new dragon monster.
            try // Try to test the CanFlee property of the dragon.
            {
                Assert(!dragon.CanFlee, "Dragon CanFlee property is false."); // Check if the CanFlee property is false.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.

            StartTest("Monster AI - Goblin Can Flee"); // This method tests the monster AI behavior for the Goblin class, including the CanFlee property.
            Goblin goblin = new Goblin(); // Create a new goblin monster.
            try // Try to test the CanFlee property of the goblin.
            {
                Assert(goblin.CanFlee, "Goblin CanFlee property is true by default."); // // Check if the CanFlee property is true.
                PassTest(); // Test passed.
            }
            catch (Exception ex) { FailTest($"Exception during test: {ex.Message} {ex.StackTrace}"); } // Handle any exceptions that occur during the test.
        }

        /// <summary>
        /// The StartTest
        /// </summary>
        /// <param name="testName">The testName<see cref="string"/></param>
        private void StartTest(string testName) // This method starts a test and logs the test name.
        {
            Log($"\nStarting Test: {testName}..."); // Log the test name.
            Console.WriteLine($"Starting Test: {testName}..."); // Print the test name to the console.
        }

        /// <summary>
        /// The PassTest
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void PassTest(string message = "Passed.") // This method logs the test result as passed and increments the passed test count.
        {
            Log($"  [PASS] {message}"); // Log the test result as passed.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [PASS] {message}");
            Console.ForegroundColor = ConsoleColor.White;
            _testsPassed++; // Increment the passed test count.
        }

        /// <summary>
        /// The FailTest
        /// </summary>
        /// <param name="reason">The reason<see cref="string"/></param>
        private void FailTest(string reason) // This method logs the test result as failed and increments the failed test count.
        {
            Log($"  [FAIL] {reason}"); // Log the test result as failed.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FAIL] {reason}");
            Console.ForegroundColor = ConsoleColor.White;
            _testsFailed++; // Increment the failed test count.
        }

        /// <summary>
        /// The Assert
        /// </summary>
        /// <param name="condition">The condition<see cref="bool"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        private void Assert(bool condition, string description) // This method checks the condition and logs the result. If the condition is false, it throws an exception.
        {
            string outcome = condition ? "PASS" : "FAIL";
            Log($"    Assert: {description} - {outcome}"); // Log the assertion result.
            try // Check the condition and throw an exception if it fails.
            {
                if (!condition) // If the condition is false, log the failure and throw and exception.
                {
                    Log($"      Assertion Failed Detail: {description}"); // Log the failure detail.
                    throw new TestAssertionException($"Assertion Failed: {description}"); // Throw an exception with the failure message.
                }
            }
            catch (TestAssertionException) // Handle the custom assertion exception.
            {
                throw; // Rethrow the exception to indicate a test failure.
            }
            catch (Exception ex)
            {
                Log($"    Assertion Error during check '{description}': {ex.Message}"); // Log any other exceptions that occur during the assertion check.
                throw;
            }
        }

        /// <summary>
        /// The Log
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void Log(string message) // This method logs the message to the console and the log file.
        {
            _logWriter?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}"); // Log the message with a timestamp.
            _logWriter?.Flush(); // Flush the log writer to ensure the message is written immediately.
        }

        /// <summary>
        /// Defines the <see cref="TestAssertionException" />
        /// </summary>
        private class TestAssertionException : Exception // This class defines a custom exception for test assertions.
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="TestAssertionException"/> class.
            /// </summary>
            /// <param name="message">The message<see cref="string"/></param>
            public TestAssertionException(string message) : base(message) 
            {
            }
        }
    }
}
