namespace DungeonExplorer.Monsters
{
    using System;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the Goblin monster type.
    /// </summary>
    public class Goblin : Monster
    {
        /// <summary>
        /// Base health for Goblins.
        /// </summary>
        public const int BASE_HEALTH = 25;

        /// <summary>
        /// Base damage for Goblins.
        /// </summary>
        public const int BASE_DAMAGE = 5;

        /// <summary>
        /// Base experience points awarded for defeating a Goblin.
        /// </summary>
        public const int BASE_XP = 30;

        /// <summary>
        /// Initialises a new instance of the <see cref="Goblin"/> class.
        /// </summary>
        public Goblin()
            : base("Goblin", BASE_HEALTH, BASE_DAMAGE, BASE_XP) // CORRECTED: Updated constructor call
        {
            InitialiseLoot();
            // Goblins use default CanFlee = true
        }

        /// <summary>
        /// Initialises the Goblin's potential loot drops.
        /// </summary>
        private void InitialiseLoot() // Corrected spelling
        {
            Loot.Add(new Potion("Crude Potion", "Barely effective.", 10));
            Loot.Add(new Food("Mouldy Bread", "A damp loaf.", 10));
            // 10% chance to also carry a rusty dagger
            if (_random.Next(100) < 10)
                Loot.Add(new Weapon("Rusty Dagger", "Small and chipped.", 3));
        }

        /// <summary>
        /// Goblin's specific attack behaviour. Overrides the base monster attack.
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/> to attack.</param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{Name} lunges wildly at {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;

            // Goblins have slightly more variable damage, from half up to base+1
            int calculatedDamage = _random.Next(Damage / 2, Damage + 2);
            calculatedDamage = Math.Max(0, calculatedDamage); // Ensure damage is not negative

            target.TakeDamage(calculatedDamage);
        }
    }
}