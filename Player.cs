using System;
using System.Collections.Generic;

namespace DungeonExplorer
{
    /// <summary>
    /// Represents the player character in the game.
    /// </summary>
    public class Player
    {
        private string _name; // Player's defined name
        private int _health; // Player's health points
        private List<string> _inventory; // Player's inventory

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name {
            get
            {
                return _name;
            } 
            set
            {
                // Validate the name (not null, empty, or too long)
                if (string.IsNullOrWhiteSpace(value) || value.Length > 25)
                {
                    Console.WriteLine("Erroneous input, default player name used instead.");
                    _name = "default_player"; // Default name if invalid input
                }
                else 
                {
                    _name = value;
                }
            } 
        }

        /// <summary>
        /// Gets or sets the player's health points.
        /// </summary>
        public int Health 
        {
            get
            {
                return _health;
            }
            set
            {
                // Ensure health is within the range 0-100
                if (value < 1 || value > 100) 
                {
                    _health = 0; // Set health to 0 if out of range
                }
                else
                {
                    _health = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the player's inventory.
        /// </summary>
        public List<string> Inventory
        {
            get 
            { 
                return _inventory;
            }
            set 
            {
                _inventory = value;
            }
        }

        /// <summary>
        /// Constructor for the player class. Initialises a new instance of the Player class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="health">The player's starting health.</param>
        /// <param name="inventory">The player's starting inventory.</param>
        public Player(string name, int health, List<string> inventory) 
        {
            _name = name;
            _health = health;
            _inventory = inventory;
        }


        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="item">The name of the item to add.</param>
        public void PickUpItem(string item)
        {
            _inventory.Add(item);
        }

        /// <summary>
        /// Returns a string representation of the player's inventory contents.
        /// </summary>
        /// <returns>A string listing the items int the inventory, or "Empty" if the inventory is empty.</returns>
        public string InventoryContents()
        {
            if (_inventory.Count == 0)
            {
                return "Empty";
            }
            return string.Join(", ", _inventory); // Join the items in the inventory with a comma
        }
    }
}