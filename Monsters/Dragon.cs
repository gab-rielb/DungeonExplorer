namespace DungeonExplorer.Monsters
{
    using System;
    using System.Linq;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Dragon" />
    /// </summary>
    public class Dragon : Monster
    {
        /// <summary>
        /// Defines the DRAGON_HEALTH
        /// </summary>
        private const int DRAGON_HEALTH = 200;

        /// <summary>
        /// Defines the DRAGON_DAMAGE
        /// </summary>
        private const int DRAGON_DAMAGE = 25;

        /// <summary>
        /// Defines the DRAGON_XP
        /// </summary>
        private const int DRAGON_XP = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dragon"/> class.
        /// </summary>
        public Dragon()
            : base("Dragon", DRAGON_HEALTH, DRAGON_DAMAGE, DRAGON_XP)
        {
            Loot.Add(new Weapon("Dragon Scale Sword", "A blade crafted from a razor-sharp scale.", 15));
            Loot.Add(new Potion("Greater Healing Potion", "Restores a large amount of health.", 75));
            Loot.Add(new Food("Dragon Steak", "Grants immense vitality.", 100));
            CanFlee = false;
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            if (_random.Next(100) < 35)
            {
                FireBreath(target);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{Name} claws fiercely at {targetName}!");
                Console.ForegroundColor = ConsoleColor.White;
                int calculatedDamage = _random.Next(Damage / 2, Damage + 1);
                calculatedDamage = Math.Max(0, calculatedDamage);
                target.TakeDamage(calculatedDamage);
            }
        }

        /// <summary>
        /// The FireBreath
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        private void FireBreath(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{Name} unleashes a torrent of SCALDING FIRE at {targetName}!");
            Console.ForegroundColor = ConsoleColor.White;
            int fireDamage = _random.Next(Damage, (int)(Damage * 1.5) + 1);
            fireDamage = Math.Max(0, fireDamage);
            target.TakeDamage(fireDamage);
        }

        /// <summary>
        /// The GetLootDrop
        /// </summary>
        /// <returns>The <see cref="Item"/></returns>
        public override Item GetLootDrop()
        {
            if (Loot == null || !Loot.Any()) return null;
            int itemIndex = _random.Next(Loot.Count);
            return Loot[itemIndex];
        }
    }
}
