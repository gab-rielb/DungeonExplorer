namespace DungeonExplorer
{
    using System;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Player" />
    /// </summary>
    public class Player : Creature
    {
        /// <summary>
        /// Gets the Inventory
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Gets the EquippedWeapon
        /// </summary>
        public Weapon EquippedWeapon { get; private set; }

        /// <summary>
        /// Gets the ExperiencePoints
        /// </summary>
        public int ExperiencePoints { get; private set; } = 0;

        /// <summary>
        /// Gets the Level
        /// </summary>
        public int Level { get; private set; } = 1;

        /// <summary>
        /// Defines the _xpToNextLevel
        /// </summary>
        private int _xpToNextLevel = 100;

        /// <summary>
        /// Defines the _fists
        /// </summary>
        private readonly Weapon _fists = new Weapon("Fists", "Your bare hands.", 1);

        /// <summary>
        /// Initialises a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="health">The health<see cref="int"/></param>
        /// <param name="baseAttack">The baseAttack<see cref="int"/></param>
        /// <param name="inventoryCapacity">The inventoryCapacity<see cref="int"/></param>
        public Player(string name, int health = 100, int baseAttack = 5, int inventoryCapacity = 25)
            : base(name, health, health, baseAttack)
        {
            _name = ValidateAndSetName(name); // Validate and set the name

            Inventory = new Inventory(inventoryCapacity); // Create a new inventory with the specified capacity
            EquippedWeapon = _fists;
        }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public new string Name => _name;

        /// <summary>
        /// The ValidateAndSetName
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string ValidateAndSetName(string name) // Validate the player name
        {
            string trimmedName = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName) || trimmedName.Length > 25) // Check if the name is empty or too long
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid player name provided (empty or > 25 chars). Using 'Adventurer'.");
                Console.ForegroundColor = ConsoleColor.White;
                return "Adventurer"; // Default name
            }
            return trimmedName; // Set the trimmed name
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target) // Attack method
        {
            if (target == null || !target.IsAlive || !this.IsAlive) return; // Check if the target and player are alive

            string targetName = (target is Creature c) ? c.Name : "the target"; // Get the target name

            int weaponDamage = EquippedWeapon?.Damage ?? 0; // Get weapon damage

            int totalDamage = _damage + weaponDamage; // Calculate total damage
            totalDamage = Math.Max(0, totalDamage); // Ensure damage is not negative

            Console.ForegroundColor = ConsoleColor.Green;
            if (EquippedWeapon != null && EquippedWeapon != _fists) // Check if a weapon is equipped
            {
                Console.WriteLine($"{Name} attacks {targetName} with {EquippedWeapon.Name}!"); // Attack with weapon
            }
            else // Check if using fists
            {
                Console.WriteLine($"{Name} attacks {targetName} with bare fists!"); // Attack with fists
            }

            Console.WriteLine($"Dealing {totalDamage} damage.");
            Console.ForegroundColor = ConsoleColor.White;

            target.TakeDamage(totalDamage); // Apply damage to the target
        }

        /// <summary>
        /// The Heal
        /// </summary>
        /// <param name="amount">The amount<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int Heal(int amount) // Heal method
        {
            if (amount <= 0 || !IsAlive || _health >= _maxHealth) return 0; // Check if the amount is valid and player is alive

            int neededHealth = _maxHealth - _health; // Calculate needed health
            int actualHeal = Math.Min(amount, neededHealth); // Heal the player

            _health += actualHeal; // Ensure health does not exceed max health
            return actualHeal; // Return the actual amount healed
        }

        /// <summary>
        /// The EquipWeapon
        /// </summary>
        /// <param name="weapon">The weapon<see cref="Weapon"/></param>
        public void EquipWeapon(Weapon weapon) // Equip a weapon
        {
            Weapon weaponToUnequip = EquippedWeapon; // Get the current equipped weapon

            if (weapon == null || weapon == _fists) // Check if the weapon is null or fists
            {
                if (weaponToUnequip == _fists) // Check if fists are already equipped
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{Name} is already using fists.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} unequipped {weaponToUnequip.Name}.");
                EquippedWeapon = _fists;

                if (Inventory.AddItem(weaponToUnequip)) // Check if the item can be added to inventory
                {
                    Console.WriteLine($"Stored {weaponToUnequip.Name} in inventory.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not store {weaponToUnequip.Name}, inventory full! It was dropped!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (weapon == weaponToUnequip) // Check if the weapon is already equipped
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{Name} already has {weapon.Name} equipped.");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (weaponToUnequip != _fists) // Check if the current equipped weapon is not fists
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} unequipped {weaponToUnequip.Name}.");
                if (!Inventory.AddItem(weaponToUnequip)) // Check if the item can be added to inventory
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not store {weaponToUnequip.Name}, inventory full! It was dropped!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            EquippedWeapon = weapon; // Equip the new weapon
            Inventory.RemoveItem(weapon); // Remove the weapon from inventory
            Console.ForegroundColor = ConsoleColor.Green; // Display the equipped weapon
            Console.WriteLine($"{Name} equipped {weapon.Name} (Damage: {weapon.Damage}).");
            int totalAttackPower = _damage + weapon.Damage; // Calculate total attack power
            Console.WriteLine($"Total Attack Power: {_damage} (Base) + {weapon.Damage} ({weapon.Name}) = {totalAttackPower}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The AddExperience
        /// </summary>
        /// <param name="amount">The amount<see cref="int"/></param>
        public void AddExperience(int amount) // Add experience points
        {
            if (amount <= 0 || !IsAlive) return; // Check if the amount is valid and player is alive

            ExperiencePoints += amount; // Ensure experience points are valid
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} gained {amount} experience points. Total: {ExperiencePoints}/{_xpToNextLevel}");
            Console.ForegroundColor = ConsoleColor.White;

            while (ExperiencePoints >= _xpToNextLevel && IsAlive) // Check if the player has enough experience points to level up
            {
                LevelUp(); // Level up the player
            }
        }

        /// <summary>
        /// The LevelUp
        /// </summary>
        private void LevelUp() // Level up the player
        {
            Level++; // Increase the player's level
            ExperiencePoints -= _xpToNextLevel; // Subtract the experience points needed for the next level
            _xpToNextLevel = (int)(_xpToNextLevel * 1.5); // Increase the experience points needed for the next level

            int healthIncrease = _random.Next(10, 21); // Roll new max health
            int attackIncrease = _random.Next(2, 5); // Roll new base attack

            _maxHealth += healthIncrease; // Increase max health
            _health = _maxHealth; // Fully heal the player
            _damage += attackIncrease; // Increase base attack

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n*** LEVEL UP! {Name} reached Level {Level}! ***");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Max Health +{healthIncrease} -> {_maxHealth} (Fully Healed!)");
            Console.WriteLine($"Base Attack +{attackIncrease} -> {_damage}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"XP to next level: {_xpToNextLevel} (Current XP: {ExperiencePoints})");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The PickUpItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <param name="room">The room<see cref="Room"/></param>
        public void PickUpItem(Item item, Room room) // Pick up an item from the room
        {
            if (item == null || room == null) return; // Check if the item and room are valid

            if (Inventory.AddItem(item)) // Check if the item can be added to inventory
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} picked up {item.Name}.");
                Console.ForegroundColor = ConsoleColor.White;
                room.RemoveItem(item); // Remove the item from the room
            }
            else // Check if the inventory is full
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (!(item is Key))
                {
                    Console.WriteLine($"{Name} tried to pick up {item.Name}, but inventory is full!");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The DisplayStatus
        /// </summary>
        public void DisplayStatus() // Display the player's status
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--- {Name} (Level {Level}) ---");
            Console.WriteLine($"Health: {_health}/{_maxHealth}");

            int weaponDamageValue = (EquippedWeapon != null) ? EquippedWeapon.Damage : 0; // Get weapon damage value
            string weaponName = (EquippedWeapon != null) ? EquippedWeapon.Name : "Fists"; // Get weapon name

            if (EquippedWeapon != null && EquippedWeapon != _fists) // Check if a weapon is equipped
            {
                Console.WriteLine($"Attack Power: {_damage} (Base) + {weaponDamageValue} (from {weaponName}) = {_damage + weaponDamageValue} Total");
            }
            else // Check if using fists
            {
                Console.WriteLine($"Attack Power: {_damage} (Base) + {weaponDamageValue} ({weaponName}) = {_damage + weaponDamageValue} Total");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"XP: {ExperiencePoints}/{_xpToNextLevel}");
            Console.ForegroundColor = ConsoleColor.White;

            Inventory.DisplayInventory(); // Display the inventory
        }
    }
}
