namespace DungeonExplorer.Items
{
    using System;

    /// <summary>
    /// Defines the <see cref="Food" />
    /// </summary>
    public class Food : Item, IUseable
    {
        /// <summary>
        /// Gets the HealAmount
        /// </summary>
        public int HealAmount { get; private set; } // The amount of health restored when the food is consumed.

        /// <summary>
        /// Initialises a new instance of the <see cref="Food"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        /// <param name="healAmount">The healAmount<see cref="int"/></param>
        public Food(string name, string description, int healAmount) : base(name, description)
        {
            if (healAmount <= 0) throw new ArgumentOutOfRangeException(nameof(healAmount), "Heal amount must be positive.");
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
                    Console.WriteLine($"{player.Name} consumed {Name} and restored {actualHeal} health. Current health: {player.Health}/{player.MaxHealth}.");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.Inventory.RemoveItem(this);
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
        public override string ToString() // Override ToString to include heal amount
        {
            return $"{Name}: {Description} (Heals: {HealAmount})";
        }
    }
}
