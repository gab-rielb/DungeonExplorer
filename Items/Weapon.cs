namespace DungeonExplorer.Items
{
    using System;

    /// <summary>
    /// Defines the <see cref="Weapon" />
    /// </summary>
    public class Weapon : Item
    {
        /// <summary>
        /// Gets the Damage
        /// </summary>
        public int Damage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Weapon"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        /// <param name="damage">The damage<see cref="int"/></param>
        public Weapon(string name, string description, int damage)

            : base(name, description)
        {

            if (damage < 0)

                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");

            Damage = damage;

            IsUsable = false;
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {

            return $"{base.ToString()} (Damage: {Damage})";
        }
    }

}
