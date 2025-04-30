namespace DungeonExplorer.Monsters
{
    using System;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Ogre" />
    /// </summary>
    public class Ogre : Monster
    {
        /// <summary>
        /// Defines the BASE_HEALTH
        /// </summary>
        public const int BASE_HEALTH = 70;

        /// <summary>
        /// Defines the BASE_DAMAGE
        /// </summary>
        public const int BASE_DAMAGE = 12;

        /// <summary>
        /// Defines the BASE_XP
        /// </summary>
        public const int BASE_XP = 80;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ogre"/> class.
        /// </summary>
        public Ogre()
            : base("Ogre", BASE_HEALTH, BASE_DAMAGE, BASE_XP)
        {
            InitialiseLoot();
        }

        /// <summary>
        /// The InitialiseLoot
        /// </summary>
        private void InitialiseLoot()
        {
            Loot.Add(new Weapon("Large Club", "A heavy wooden club.", 8));
            Loot.Add(new Food("Chunk of Meat", "Tough, but filling.", 25));
            if (_random.Next(100) < 25)
                Loot.Add(new Potion("Healing Potion", "Restores 30 health.", 30));
            else
                Loot.Add(new Potion("Murky Concoction", "Smells bad, heals okay.", 20));
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public override void Attack(IDamageable target)
        {
            if (!target.IsAlive || !this.IsAlive) return;

            string targetName = (target is Creature c) ? c.Name : "the target";

            if (_random.Next(100) < 25)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{Name} roars and brings its club down HARD on {targetName}!");
                Console.ForegroundColor = ConsoleColor.White;
                int powerDamage = _random.Next(Damage, (int)(Damage * 1.6) + 1);
                powerDamage = Math.Max(0, powerDamage);
                target.TakeDamage(powerDamage);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{Name} swings its club clumsily at {targetName}!");
                Console.ForegroundColor = ConsoleColor.White;
                int calculatedDamage = _random.Next(Damage / 2, Damage + 1);
                calculatedDamage = Math.Max(0, calculatedDamage);
                target.TakeDamage(calculatedDamage);
            }
        }
    }
}
