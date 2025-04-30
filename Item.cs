namespace DungeonExplorer
{
    using System;

    /// <summary>
    /// Defines the <see cref="Item" />
    /// </summary>
    public abstract class Item
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
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Item name cannot be null or empty.", nameof(name)); // The name cannot be null or empty.
            if (string.IsNullOrWhiteSpace(description)) description = "An indescribable item."; // The description cannot be null or empty.

            Name = name.Trim();
            Description = description.Trim();
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{Name}: {Description}"; // Override ToString to include name and description
        }
    }
}
