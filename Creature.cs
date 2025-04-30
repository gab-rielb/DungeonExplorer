namespace DungeonExplorer
{
    using System;

    /// <summary>
    /// Defines the <see cref="Creature" />
    /// </summary>
    public abstract class Creature : IDamageable
    {
        /// <summary>
        /// Defines the _name
        /// </summary>
        protected string _name;

        /// <summary>
        /// Defines the _health
        /// </summary>
        protected int _health;

        /// <summary>
        /// Defines the _maxHealth
        /// </summary>
        protected int _maxHealth;

        /// <summary>
        /// Defines the _damage
        /// </summary>
        protected int _damage;

        /// <summary>
        /// Defines the _random
        /// </summary>
        protected static Random _random = new Random();

        /// <summary>
        /// Gets the Name
        /// </summary>
        public virtual string Name => _name;

        /// <summary>
        /// Gets the Health
        /// </summary>
        public virtual int Health => _health;

        /// <summary>
        /// Gets a value indicating whether IsAlive
        /// </summary>
        public bool IsAlive => _health > 0;

        /// <summary>
        /// Gets the MaxHealth
        /// </summary>
        public int MaxHealth => _maxHealth;

        /// <summary>
        /// Gets the Damage
        /// </summary>
        public int Damage => _damage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creature"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="health">The health<see cref="int"/></param>
        /// <param name="maxHealth">The maxHealth<see cref="int"/></param>
        /// <param name="damage">The damage<see cref="int"/></param>
        protected Creature(string name, int health, int maxHealth, int damage)
        {
            _name = string.IsNullOrWhiteSpace(name) ? "Unknown Creature" : name.Trim();
            _maxHealth = Math.Max(1, maxHealth);

            _health = Math.Min(_maxHealth, Math.Max(0, health));
            _damage = Math.Max(0, damage);
        }

        /// <summary>
        /// The Attack
        /// </summary>
        /// <param name="target">The target<see cref="IDamageable"/></param>
        public abstract void Attack(IDamageable target);

        /// <summary>
        /// The TakeDamage
        /// </summary>
        /// <param name="damage">The damage<see cref="int"/></param>
        public virtual void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            int actualDamage = Math.Max(0, damage);
            _health -= actualDamage;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{_name} took {actualDamage} damage!");
            Console.ForegroundColor = ConsoleColor.White;

            if (!IsAlive)
            {
                _health = 0;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{_name} has been defeated!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{_name}'s health is now {_health}/{_maxHealth}.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
