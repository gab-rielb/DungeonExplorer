namespace DungeonExplorer
{
    using System;

    /// <summary>
    /// Defines the <see cref="Item" />
    /// </summary>
    public abstract class Item : ICollectable
    {
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsUsable
        /// </summary>
        public bool IsUsable { get; protected set; } = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        protected Item(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Item name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                description = "An indescribable item.";

            Name = name;
            Description = description;
        }

        /// <summary>
        /// The PickUp
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        public virtual void PickUp(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (player.Inventory.AddItem(this))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{player.Name} picked up {Name}.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{player.Name} tried to pick up {Name}, but the inventory is full.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}
