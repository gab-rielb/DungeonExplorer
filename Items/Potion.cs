namespace DungeonExplorer.Items
{
    using System;

    /// <summary>
    /// Defines the <see cref="Potion" />
    /// </summary>
    public class Potion : Item, IUseable
    {
        /// <summary>
        /// Gets the HealAmount
        /// </summary>
        public int HealAmount { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Potion"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        /// <param name="healAmount">The healAmount<see cref="int"/></param>
        public Potion(string name, string description, int healAmount) : base(name, description)
        {
            if (healAmount <= 0) throw new ArgumentOutOfRangeException(nameof(healAmount), "Heal amount must be positive."); // The heal amount should be a positive integer.
            HealAmount = healAmount;
            IsUsable = true;
        }

        /// <summary>
        /// The Use
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        public void Use(Player player)
        {
            if (player.Health < player.MaxHealth) // Check if the player is not at full health
            {
                int actualHeal = player.Heal(HealAmount); // Heal the player and get the actual amount healed

                if (actualHeal > 0) // If the player was healed
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{player.Name} used {Name} and restored {actualHeal} health. Current health: {player.Health}/{player.MaxHealth}.");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Inventory.RemoveItem(this);
                }
                else // If the potion had no effect
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{player.Name} used {Name}, but it had no effect (maybe health was already full?).");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else // If the player is at full health
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{player.Name} cannot use {Name}. Health is already full.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{Name}: {Description} (Heals: {HealAmount})"; // Override ToString to include heal amount
        }
    }
}
