namespace DungeonExplorer.Items
{
    using System;

    /// <summary>
    /// Defines the <see cref="Key" />
    /// </summary>
    public class Key : Item
    {
        /// <summary>
        /// Gets the KeyId
        /// </summary>
        public int KeyId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        /// <param name="keyId">The keyId<see cref="int"/></param>
        public Key(string name, string description, int keyId) : base(name, description)
        {

            if (keyId < 0) throw new ArgumentOutOfRangeException(nameof(keyId), "Key ID cannot be negative.");

            KeyId = keyId;

            IsUsable = false;
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
