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
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="health">The health<see cref="int"/></param>
        /// <param name="baseAttack">The baseAttack<see cref="int"/></param>
        /// <param name="inventoryCapacity">The inventoryCapacity<see cref="int"/></param>
        public Player(string name, int health = 100, int baseAttack = 5, int inventoryCapacity = 25)
            : base(name, health, health, baseAttack)
        {
            Name = name;
            Inventory = new Inventory(inventoryCapacity);
            EquippedWeapon = _fists;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public new string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 25)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid player name provided (empty or > 25 chars). Using 'Adventurer'.");
                    Console.ForegroundColor = ConsoleColor.White;
                    _name = "Adventurer";
                }
                else
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target)
        {
            if (target == null || !target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            int actualDamage;

            Console.ForegroundColor = ConsoleColor.Green;
            if (EquippedWeapon != null && EquippedWeapon != _fists)
            {
                actualDamage = _damage + EquippedWeapon.Damage;
                Console.WriteLine($"{Name} attacks {targetName} with {EquippedWeapon.Name}!");
            }
            else
            {
                actualDamage = _damage;
                Console.WriteLine($"{Name} attacks {targetName} with bare fists!");
            }

            actualDamage = Math.Max(0, actualDamage);

            Console.WriteLine($"Dealing {actualDamage} damage.");
            Console.ForegroundColor = ConsoleColor.White;

            target.TakeDamage(actualDamage);
        }

        /// <summary>
        /// The Heal
        /// </summary>
        /// <param name="amount">The amount<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int Heal(int amount)
        {
            if (amount <= 0 || !IsAlive) return 0;

            int neededHealth = _maxHealth - _health;
            int actualHeal = Math.Min(amount, neededHealth);

            if (actualHeal <= 0)
            {
                return 0;
            }
            else
            {
                _health += actualHeal;
                return actualHeal;
            }
        }

        /// <summary>
        /// The EquipWeapon
        /// </summary>
        /// <param name="weapon">The weapon<see cref="Weapon"/></param>
        public void EquipWeapon(Weapon weapon)
        {
            Weapon weaponToUnequip = EquippedWeapon;

            if (weapon == null || weapon == _fists)
            {
                if (weaponToUnequip == _fists)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{Name} is already using fists.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} unequipped {weaponToUnequip.Name}.");
                EquippedWeapon = _fists;

                if (Inventory.AddItem(weaponToUnequip))
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

            if (weapon == weaponToUnequip)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{Name} already has {weapon.Name} equipped.");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (weaponToUnequip != _fists)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} unequipped {weaponToUnequip.Name}.");
                if (Inventory.AddItem(weaponToUnequip))
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
            }

            EquippedWeapon = weapon;
            Inventory.RemoveItem(weapon);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} equipped {weapon.Name} (Damage: {weapon.Damage}).");
            int totalAttackPower = _damage + weapon.Damage;
            Console.WriteLine($"Total Attack Power: {_damage} (Base) + {weapon.Damage} ({weapon.Name}) = {totalAttackPower}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The AddExperience
        /// </summary>
        /// <param name="amount">The amount<see cref="int"/></param>
        public void AddExperience(int amount)
        {
            if (amount <= 0 || !IsAlive) return;

            ExperiencePoints += amount;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} gained {amount} experience points. Total: {ExperiencePoints}/{_xpToNextLevel}");
            Console.ForegroundColor = ConsoleColor.White;

            while (ExperiencePoints >= _xpToNextLevel && IsAlive)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// The LevelUp
        /// </summary>
        private void LevelUp()
        {
            Level++;
            ExperiencePoints -= _xpToNextLevel;
            _xpToNextLevel = (int)(_xpToNextLevel * 1.5);

            int healthIncrease = _random.Next(10, 21);
            int attackIncrease = _random.Next(2, 5);

            _maxHealth += healthIncrease;
            _health = _maxHealth;
            _damage += attackIncrease;

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
        public void PickUpItem(Item item, Room room)
        {
            if (item == null || room == null) return;

            if (Inventory.AddItem(item))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{Name} picked up {item.Name}.");
                Console.ForegroundColor = ConsoleColor.White;
                room.RemoveItem(item);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} tried to pick up {item.Name}, but inventory is full!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The DisplayStatus
        /// </summary>
        public void DisplayStatus()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--- {Name} (Level {Level}) ---");
            Console.WriteLine($"Health: {_health}/{_maxHealth}");

            int weaponDamageValue;
            string weaponName;

            if (EquippedWeapon != null && EquippedWeapon != _fists)
            {
                weaponDamageValue = EquippedWeapon.Damage;
                weaponName = EquippedWeapon.Name;
                Console.WriteLine($"Attack Power: {_damage} (Base) + {weaponDamageValue} (from {weaponName}) = {_damage + weaponDamageValue} Total");
            }
            else
            {
                weaponDamageValue = 0;
                weaponName = "Fists";
                Console.WriteLine($"Attack Power: {_damage} (Base, using {weaponName})");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"XP: {ExperiencePoints}/{_xpToNextLevel}");
            Console.ForegroundColor = ConsoleColor.White;
            Inventory.DisplayInventory();
        }
    }
}
