namespace DungeonExplorer
{
    /// <summary>
    /// Defines the <see cref="IDamageable" />
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Gets the Health
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Gets a value indicating whether IsAlive
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// The TakeDamage
        /// </summary>
        /// <param name="damage">The damage<see cref="int"/></param>
        void TakeDamage(int damage);
    }

    /// <summary>
    /// Defines the <see cref="IUseable" />
    /// </summary>
    public interface IUseable
    {
        /// <summary>
        /// The Use
        /// </summary>
        /// <param name="player">The player<see cref="Player"/></param>
        void Use(Player player);
    }
}
