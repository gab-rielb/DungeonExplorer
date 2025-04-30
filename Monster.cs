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
        /// Initializes a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="initialHealth">The initialHealth<see cref="int"/></param>
        /// <param name="damage">The damage<see cref="int"/></param>
        /// <param name="experienceValue">The experienceValue<see cref="int"/></param>
        protected Monster(string name, int initialHealth, int damage, int experienceValue)
            : base(name, initialHealth, initialHealth, damage)
        {
            ExperienceValue = experienceValue;
            Loot = new List<Item>();
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{Name} attacks {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;

            int calculatedDamage = _random.Next(1, Damage + 1);
            if (calculatedDamage < 0) calculatedDamage = 0;

            target.TakeDamage(calculatedDamage);
        }

        /// <summary>
        /// The GetLootDrop
        /// </summary>
        /// <returns>The <see cref="Item"/></returns>
        public virtual Item GetLootDrop()
        {
            if (Loot == null || !Loot.Any()) return null;

            int chance = _random.Next(100);
            if (chance < 60)
            {
                int itemIndex = _random.Next(Loot.Count);
                return Loot[itemIndex];
            }
            return null;
        }

        /// <summary>
        /// The PerformAction
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        /// <param name="currentRoom">The currentRoom<see cref="Room"/></param>
        /// <param name="gameMap">The gameMap<see cref="GameMap"/></param>
        public virtual void PerformAction(Player player, Room currentRoom, GameMap gameMap)
        {
            if (!IsAlive || !player.IsAlive) return;

            CurrentPosition = currentRoom.Coordinates;

            bool isInSideBranch = CurrentPosition.X != 0;

            if (isInSideBranch && CanFlee && (double)_health / _maxHealth < 0.30 && _random.Next(100) < 50)
            {
                AttemptFlee(currentRoom, gameMap, player);
            }
            else
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
        protected virtual void AttemptFlee(Room currentRoom, GameMap gameMap, Player player)
        {
            int xDirectionBack = -Math.Sign(CurrentPosition.X);
            if (xDirectionBack == 0)
            {
                Attack(player);
                return;
            }

            string directionBack = (xDirectionBack == 1) ? Direction.Right : Direction.Left;
            Point fleeToCoord = new Point(CurrentPosition.X + xDirectionBack, CurrentPosition.Y);
            Room fleeToRoom = gameMap.GetRoom(fleeToCoord);

            if (fleeToRoom != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{Name} is badly wounded and attempts to flee {directionBack.ToLower()}!");
                Console.ForegroundColor = ConsoleColor.White;

                bool removed = currentRoom.MonstersInRoom.Remove(this);
                if (removed)
                {
                    fleeToRoom.AddMonster(this);
                    this.CurrentPosition = fleeToCoord;
                    this.HealToPercent(50);
                    this.CanFlee = false;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{Name} retreated to {fleeToCoord} and recovered slightly! It won't flee again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Failed to remove {Name} from room {currentRoom.Coordinates} during flee attempt.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Attack(player);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{Name} tries to flee {directionBack.ToLower()} but finds the way blocked!");
                Console.ForegroundColor = ConsoleColor.White;
                Attack(player);
            }
        }

        /// <summary>
        /// The HealToPercent
        /// </summary>
        /// <param name="percent">The percent<see cref="int"/></param>
        protected void HealToPercent(int percent)
        {
            if (!IsAlive) return;
            percent = Math.Max(0, Math.Min(percent, 100));

            int targetHealth = (int)Math.Ceiling((double)_maxHealth * percent / 100.0);
            int healAmount = targetHealth - _health;

            if (healAmount > 0)
            {
                _health += healAmount;
                _health = Math.Min(_health, _maxHealth);
            }
        }

        /// <summary>
        /// The ApplyHealthBoost
        /// </summary>
        /// <param name="boostedMaxHealth">The boostedMaxHealth<see cref="int"/></param>
        public void ApplyHealthBoost(int boostedMaxHealth)
        {
            if (boostedMaxHealth > 0)
            {
                _maxHealth = boostedMaxHealth;
                _health = boostedMaxHealth;
            }
        }
    }
}
