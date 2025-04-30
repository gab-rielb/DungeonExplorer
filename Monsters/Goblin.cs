namespace DungeonExplorer.Monsters
{
    using System;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Goblin" />
    /// </summary>
    public class Goblin : Monster
    {
        /// <summary>
        /// Defines the BASE_HEALTH
        /// </summary>
        public const int BASE_HEALTH = 25;

        /// <summary>
        /// Defines the BASE_DAMAGE
        /// </summary>
        public const int BASE_DAMAGE = 5;

        /// <summary>
        /// Defines the BASE_XP
        /// </summary>
        public const int BASE_XP = 30;

        /// <summary>
        /// Initialises a new instance of the <see cref="Goblin"/> class.
        /// </summary>
        public Goblin()
            : base("Goblin", BASE_HEALTH, BASE_DAMAGE, BASE_XP)
        {
            InitialiseLoot();
        }

        /// <summary>
        /// The InitialiseLoot
        /// </summary>
        private void InitialiseLoot()
        {
            Loot.Add(new Potion("Crude Potion", "Barely effective.", 10));
            Loot.Add(new Food("Mouldy Bread", "A damp loaf.", 10));
            if (_random.Next(100) < 10)
                Loot.Add(new Weapon("Rusty Dagger", "Small and chipped.", 3));
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return; // Check if the target and player are alive

            string targetName = (target is Creature c) ? c.Name : "the target"; // Get the target name

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{Name} lunges wildly at {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;

            int calculatedDamage = _random.Next(Damage / 2, Damage + 2); // Calculate damage
            calculatedDamage = Math.Max(0, calculatedDamage); // Ensure damage is not negative
            target.TakeDamage(calculatedDamage); // Apply damage to the target
        }
    }
}
