namespace DungeonExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Monster" />
    /// </summary>
    public abstract class Monster : Creature
    {
        /// <summary>
        /// Gets or sets the ExperienceValue
        /// </summary>
        public int ExperienceValue { get; protected set; }

        /// <summary>
        /// Gets or sets the Loot
        /// </summary>
        public List<Item> Loot { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanFlee
        /// </summary>
        public bool CanFlee { get; protected set; } = true;

        /// <summary>
        /// Gets or sets the CurrentPosition
        /// </summary>
        protected Point CurrentPosition { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="initialHealth">The initialHealth<see cref="int"/></param>
        /// <param name="damage">The damage<see cref="int"/></param>
        /// <param name="experienceValue">The experienceValue<see cref="int"/></param>
        protected Monster(string name, int initialHealth, int damage, int experienceValue)
            : base(name, initialHealth, initialHealth, damage)
        {
            ExperienceValue = experienceValue >= 0 ? experienceValue : 0;
            Loot = new List<Item>();
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target) // Attack method to be implemented by derived classes
        {
            if (!target.IsAlive || !this.IsAlive) return; // Check if the target and player are alive

            string targetName = (target is Creature c) ? c.Name : "the target"; // Get the target name

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{Name} attacks {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;

            int calculatedDamage = _random.Next(1, Damage + 1); // Calculate damage
            calculatedDamage = Math.Max(0, calculatedDamage); // Ensure damage is not negative

            target.TakeDamage(calculatedDamage); // Apply damage to the target
        }

        /// <summary>
        /// The GetLootDrop
        /// </summary>
        /// <returns>The <see cref="Item"/></returns>
        public virtual Item GetLootDrop() // Drop loot after defeat
        {
            if (Loot == null || !Loot.Any()) return null; // Check if loot is available

            int chance = _random.Next(100); // Random chance for loot drop
            if (chance < 60) // 60% chance to drop loot
            {
                int itemIndex = _random.Next(Loot.Count); // Get a random index from the loot list
                return Loot[itemIndex]; // Return the loot item
            }
            return null;
        }

        /// <summary>
        /// The PerformAction
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        /// <param name="currentRoom">The currentRoom<see cref="Room"/></param>
        /// <param name="gameMap">The gameMap<see cref="GameMap"/></param>
        public virtual void PerformAction(Player player, Room currentRoom, GameMap gameMap) // Perform an action based on the monster's statistics
        {
            if (!IsAlive || !player.IsAlive) return; // Check if the monster and player are alive

            CurrentPosition = currentRoom.Coordinates; // Set the current position of the monster

            if (CanFlee && (double)_health / _maxHealth < 0.30 && _random.Next(100) < 50) // 50% chance to flee if health is below 30%
            {
                AttemptFlee(currentRoom, gameMap, player); // Attempt to flee
            }
            else // otherwise, attack the player
            {
                Attack(player);
            }
        }

        /// <summary>
        /// The AttemptFlee
        /// </summary>
        /// <param name="currentRoom">The currentRoom<see cref="Room"/></param>
        /// <param name="gameMap">The gameMap<see cref="GameMap"/></param>
        /// <param name="player">The player<see cref="Player"/></param>
        protected virtual void AttemptFlee(Room currentRoom, GameMap gameMap, Player player) // Attempt to flee from the current room
        {
            string directionToFlee = null; // Default direction to flee
            Point fleeToCoord = CurrentPosition; // Default to current position

            if (CurrentPosition.X > 0) directionToFlee = Direction.Left; // Flee left
            else if (CurrentPosition.X < 0) directionToFlee = Direction.Right; // Flee right

            Room fleeToRoom = null; // Get the room to flee to

            if (!string.IsNullOrEmpty(directionToFlee) && currentRoom.Exits.TryGetValue(directionToFlee, out fleeToRoom)) // Check if the room exists
            {
                fleeToCoord = fleeToRoom.Coordinates; // Set the coordinates to flee to
            }
            else
            {
                fleeToRoom = null;
            }

            if (fleeToRoom != null) // Check if the room to flee to is valid
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{Name} is badly wounded and attempts to flee {directionToFlee.ToLower()}!");
                Console.ForegroundColor = ConsoleColor.White;

                bool removed = currentRoom.MonstersInRoom.Remove(this); // Remove the monster from the current room
                if (removed) // Check if the monster was removed successfully
                {
                    fleeToRoom.AddMonster(this); // Add the monster to the new room
                    this.CurrentPosition = fleeToCoord; // Update the current position
                    this.HealToPercent(50); // Heal to 50% of max health
                    this.CanFlee = false; // Set CanFlee to false

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{Name} retreated to {fleeToCoord} and recovered slightly! It won't flee again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Failed to remove {Name} from room {currentRoom.Coordinates} during flee attempt. It attacks instead!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Attack(player);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{Name} tries to flee but finds the way blocked! It fights on!");
                Console.ForegroundColor = ConsoleColor.White;
                Attack(player);
            }
        }

        /// <summary>
        /// The HealToPercent
        /// </summary>
        /// <param name="percent">The percent<see cref="int"/></param>
        protected void HealToPercent(int percent) // Heal the monster to a specific percentage of its max health
        {
            if (!IsAlive) return; // Check if the monster is alive
            percent = Math.Max(0, Math.Min(percent, 100)); // Clamp the percentage between 0 and 100

            int targetHealth = (int)Math.Ceiling((double)_maxHealth * percent / 100.0); // Calculate target health
            int healAmount = targetHealth - _health; // Calculate heal amount

            if (healAmount > 0) // Check if heal amount is positive
            {
                _health += healAmount; // Heal the monster
                _health = Math.Min(_health, _maxHealth); // Ensure health does not exceed max health
            }
        }

        /// <summary>
        /// The ApplyHealthBoost
        /// </summary>
        /// <param name="boostedMaxHealth">The boostedMaxHealth<see cref="int"/></param>
        public void ApplyHealthBoost(int boostedMaxHealth) // Apply a health boost to the monster
        {
            if (boostedMaxHealth > 0) // Check if the boosted max health is valid
            {
                _maxHealth = boostedMaxHealth; // Set the new max health
                _health = boostedMaxHealth; // Heal the monster to the new max health
            }
        }
    }
}
