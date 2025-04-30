namespace DungeonExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DungeonExplorer.Items;

    /// <summary>
    /// Defines the <see cref="Inventory" />
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Defines the _items
        /// </summary>
        private List<Item> _items;

        /// <summary>
        /// Defines the _capacity
        /// </summary>
        private int _capacity;

        /// <summary>
        /// Initialises a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="capacity">The capacity<see cref="int"/></param>
        public Inventory(int capacity = 25)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Inventory capacity must be positive."); // The capacity should be a positive integer.
            _items = new List<Item>();
            _capacity = capacity;
        }

        /// <summary>
        /// Gets the Count
        /// </summary>
        public int Count => _items.Count(i => !(i is Key));

        /// <summary>
        /// Gets the Capacity
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// Gets a value indicating whether IsFull
        /// </summary>
        public bool IsFull => Count >= _capacity;

        /// <summary>
        /// The AddItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool AddItem(Item item) // Adds an item to the inventory
        {
            if (item == null) throw new ArgumentNullException(nameof(item)); // The item cannot be null.

            if (item is Key) // Keys do not count towards capacity
            {
                _items.Add(item); // Add the key to the inventory
                return true; // Item added successfully
            }
            if (IsFull) // The inventory is full
            {
                return false; // Cannot add the item
            }
            _items.Add(item); // Add the item to the inventory
            return true; // Item added successfully
        }

        /// <summary>
        /// The RemoveItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool RemoveItem(Item item) // Removes an item from the inventory
        {
            if (item == null) throw new ArgumentNullException(nameof(item)); // The item cannot be null
            return _items.Remove(item); // Remove the item from the inventory
        }

        /// <summary>
        /// The GetItemByName
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="Item"/></returns>
        public Item GetItemByName(string name) // Gets an item by its name
        {
            if (string.IsNullOrWhiteSpace(name)) return null; // The name cannot be null or empty
            return _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); // Find the item by name
        }

        /// <summary>
        /// The GetAllItems
        /// </summary>
        /// <returns>The <see cref="List{Item}"/></returns>
        public List<Item> GetAllItems() // Gets all items in the inventory
        {
            return new List<Item>(_items); // Return a copy of the items list
        }

        /// <summary>
        /// The GetWeapons
        /// </summary>
        /// <returns>The <see cref="List{Weapon}"/></returns>
        public List<Weapon> GetWeapons() // Gets all weapons in the inventory
        {
            return _items.OfType<Weapon>().ToList(); // Return a list of weapons
        }

        /// <summary>
        /// The GetUseableItems
        /// </summary>
        /// <returns>The <see cref="List{IUseable}"/></returns>
        public List<IUseable> GetUseableItems() // Gets all useable items in the inventory
        {
            return _items.OfType<IUseable>().ToList(); // Return a list of useable items
        }

        /// <summary>
        /// The GetPotions
        /// </summary>
        /// <returns>The <see cref="List{Potion}"/></returns>
        public List<Potion> GetPotions() // Gets all potions in the inventory
        {
            return _items.OfType<Potion>().ToList(); // Return a list of potions
        }

        /// <summary>
        /// The GetFood
        /// </summary>
        /// <returns>The <see cref="List{Food}"/></returns>
        public List<Food> GetFood() // Gets all food items in the inventory
        {
            return _items.OfType<Food>().ToList(); // Return a list of food items
        }

        /// <summary>
        /// The GetStrongestWeapon
        /// </summary>
        /// <returns>The <see cref="Weapon"/></returns>
        public Weapon GetStrongestWeapon() // Gets the strongest weapon in the inventory
        {
            return _items.OfType<Weapon>()
                         .OrderByDescending(w => w.Damage)
                         .FirstOrDefault(); // Return the strongest weapon
        }

        /// <summary>
        /// The GetItemsSortedByName
        /// </summary>
        /// <returns>The <see cref="List{Item}"/></returns>
        public List<Item> GetItemsSortedByName() // Gets all items sorted by name
        {
            return _items.OrderBy(i => i.Name).ToList(); // Return a list of items sorted by name
        }

        /// <summary>
        /// The DisplayInventory
        /// </summary>
        public void DisplayInventory() // Displays the inventory
        {
            Console.ForegroundColor = ConsoleColor.Cyan; // Set the color to cyan
            Console.WriteLine($"\n--- Inventory ({Count}/{Capacity}) ---"); // Display the inventory header

            var regularItems = _items.Where(i => !(i is Key)).OrderBy(i => i.Name).ToList(); // Get regular items sorted by name

            if (!regularItems.Any()) // No regular items in the inventory
            {
                Console.WriteLine("No regular items.");
            }
            else
            {
                var groupedItems = regularItems
                   .GroupBy(i => i.ToString())
                   .Select(g => new { ItemInfo = g.Key, Count = g.Count() }); // Group items by name and count them

                foreach (var group in groupedItems) // Display grouped items
                {
                    Console.WriteLine($"- {group.ItemInfo}{(group.Count > 1 ? $" (x{group.Count})" : "")}");
                }
            }

            var keys = _items.OfType<Key>().OrderBy(k => k.Name).ToList(); // Get keys sorted by name
            if (keys.Any()) // Display keys
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n--- Keys (Do not use capacity) ---");
                foreach (var key in keys)
                {
                    Console.WriteLine($"- {key.ToString()}");
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
