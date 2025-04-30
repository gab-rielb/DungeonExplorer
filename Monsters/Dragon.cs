namespace DungeonExplorer.Monsters
{
    using System;
    using System.Linq;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the Dragon boss monster.
    /// </summary>
    public class Dragon : Monster
    {
        // Constants for Dragon stats
        private const int DRAGON_HEALTH = 200;
        private const int DRAGON_DAMAGE = 25;
        private const int DRAGON_XP = 500;

        /// <summary>
        /// Initialises a new instance of the <see cref="Dragon"/> class.
        /// </summary>
        public Dragon()
            : base("Dragon", DRAGON_HEALTH, DRAGON_DAMAGE, DRAGON_XP) // CORRECTED: Updated constructor call
        {
            // Define Dragon-specific loot
            Loot.Add(new Weapon("Dragon Scale Sword", "A blade crafted from a razor-sharp scale.", 15));
            Loot.Add(new Potion("Greater Healing Potion", "Restores a large amount of health.", 75));
            Loot.Add(new Food("Dragon Steak", "Grants immense vitality.", 100));
            // Boss monster cannot flee
            CanFlee = false;
        }

        /// <summary>
        /// Dragon's attack behaviour, including a chance for Fire Breath.
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/> to attack.</param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            // 35% chance to use Fire Breath special attack
            if (_random.Next(100) < 35)
            {
                FireBreath(target);
            }
            else // 65% chance for a normal claw attack
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{Name} claws fiercely at {targetName}!");
                Console.ForegroundColor = ConsoleColor.White;
                // Standard attack damage range (half base up to base)
                int calculatedDamage = _random.Next(Damage / 2, Damage + 1);
                calculatedDamage = Math.Max(0, calculatedDamage); // Ensure non-negative
                target.TakeDamage(calculatedDamage);
            }
        }

        /// <summary>
        /// The Dragon's Fire Breath special attack, dealing increased damage.
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/> of the fire breath.</param>
        private void FireBreath(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return; // Re-check status

            string targetName = (target is Creature c) ? c.Name : "the target";

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{Name} unleashes a torrent of SCALDING FIRE at {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;
            // Fire breath deals higher damage (base up to 150% base)
            int fireDamage = _random.Next(Damage, (int)(Damage * 1.5) + 1);
            fireDamage = Math.Max(0, fireDamage); // Ensure non-negative
            target.TakeDamage(fireDamage);
        }

        /// <summary>
        /// Dragon's loot drop logic. Guarantees one random item from its specific loot table.
        /// </summary>
        /// <returns>An <see cref="Item"/> from the Dragon's loot table.</returns>
        public override Item GetLootDrop()
        {
            if (Loot == null || !Loot.Any()) return null;
            // Boss guarantees a drop
            int itemIndex = _random.Next(Loot.Count); // Pick one random item from its defined loot
            return Loot[itemIndex];
        }
    }
}