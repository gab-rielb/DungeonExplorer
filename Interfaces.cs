using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Interface for anything that can be damaged.
    /// </summary>
    public interface IDamageable
    {
        int Health { get; } // Read-only property for current health
        bool isAlive { get; } // Check if the entity is still alive
        void TakeDamage(int damage); // Method to apply damage
    }
    /// <summary>
    /// Interface for anything that can be collected.
    /// </summary>
    public interface ICollectable
    {
        string Name { get; } // Name of the item
        string Description { get; } // Description of the item
        void PickUp(Player player); // Action when the item is picked up
    }
    /// <summary>
    /// Interface for anything that can be used.
    /// </summary>
    public interface IUseable
    {
        void Use(Player player); // Action when the item is used
    }
}
