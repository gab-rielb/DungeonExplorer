using System;
using System.Collections.Generic;

namespace DungeonExplorer
{
    /// <summary>
    /// Player class intends to be able to create a player for the game with basic information.
    /// 
    /// Attributes:
    /// (string) name
    /// (int) health
    /// (list<string>) inventory
    /// 
    /// Methods:
    /// PickUpItem -> (currently no functionality)
    /// InventoryContents -> Displays the contents of the player inventory
    /// </summary>
    public class Player
    {
        // Player attributes
        private string _name;
        private int _health;
        private List<string> _inventory;

        // Player Accessor Functions
        public string Name {
            get
            {
                return _name;
            } 
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _name = "default_player";
                }
                else 
                {
                    _name = value;
                }
            } 
        }
        public int Health 
        {
            get
            {
                return _health;
            }
            set
            {
                if (value < 1) 
                {
                    _health = 0;
                }
                else
                {
                    _health = value;
                }
            }
        }
        public List<string> Inventory
        {
            get 
            { 
                return _inventory;
            }
            set 
            {
                _inventory.Clear();
                _inventory = value;
            }
        }
        
        // Player Constructor
        public Player(string name, int health, List<string> inventory) 
        {
            Name = name;
            Health = health;
            Inventory = inventory;
        }

        // Player Methods
        public void PickUpItem(string item)
        {

        }
        public string InventoryContents()
        {
            return string.Join(", ", _inventory);
        }
    }
}