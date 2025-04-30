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
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="capacity">The capacity<see cref="int"/></param>
        public Inventory(int capacity = 25)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Inventory capacity must be positive.");
            _items = new List<Item>();
            _capacity = capacity;
        }

        /// <summary>
        /// Gets the Count
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets the Capacity
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// Gets a value indicating whether IsFull
        /// </summary>
        public bool IsFull => _items.Count >= _capacity;

        /// <summary>
        /// The AddItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool AddItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (IsFull)
            {
                return false;
            }
            _items.Add(item);
            return true;
        }

        /// <summary>
        /// The RemoveItem
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool RemoveItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _items.Remove(item);
        }

        /// <summary>
        /// The GetItemByName
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="Item"/></returns>
        public Item GetItemByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The GetAllItems
        /// </summary>
        /// <returns>The <see cref="List{Item}"/></returns>
        public List<Item> GetAllItems()
        {
            return new List<Item>(_items);
        }

        /// <summary>
        /// The GetWeapons
        /// </summary>
        /// <returns>The <see cref="List{Weapon}"/></returns>
        public List<Weapon> GetWeapons()
        {
            return _items.OfType<Weapon>().ToList();
        }

        /// <summary>
        /// The GetUseableItems
        /// </summary>
        /// <returns>The <see cref="List{IUseable}"/></returns>
        public List<IUseable> GetUseableItems()
        {
            return _items.OfType<IUseable>().ToList();
        }

        /// <summary>
        /// The GetPotions
        /// </summary>
        /// <returns>The <see cref="List{Potion}"/></returns>
        public List<Potion> GetPotions()
        {
            return _items.OfType<Potion>().ToList();
        }

        /// <summary>
        /// The GetFood
        /// </summary>
        /// <returns>The <see cref="List{Food}"/></returns>
        public List<Food> GetFood()
        {
            return _items.OfType<Food>().ToList();
        }

        /// <summary>
        /// The GetStrongestWeapon
        /// </summary>
        /// <returns>The <see cref="Weapon"/></returns>
        public Weapon GetStrongestWeapon()
        {
            return _items.OfType<Weapon>()
                         .OrderByDescending(w => w.Damage)
                         .FirstOrDefault();
        }

        /// <summary>
        /// The GetItemsSortedByName
        /// </summary>
        /// <returns>The <see cref="List{Item}"/></returns>
        public List<Item> GetItemsSortedByName()
        {
            return _items.OrderBy(i => i.Name).ToList();
        }

        /// <summary>
        /// The DisplayInventory
        /// </summary>
        public void DisplayInventory()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n--- Inventory ({Count}/{Capacity}) ---");
            if (Count == 0)
            {
                Console.WriteLine("Empty");
            }
            else
            {
                var sortedItems = GetItemsSortedByName();
                var groupedItems = sortedItems
                    .GroupBy(i => i.ToString())
                    .Select(g => new { ItemInfo = g.Key, Count = g.Count() });

                foreach (var group in groupedItems)
                {
                    Console.WriteLine($"- {group.ItemInfo}{(group.Count > 1 ? $" (x{group.Count})" : "")}");
                }

            }
            Console.WriteLine("--------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

}
