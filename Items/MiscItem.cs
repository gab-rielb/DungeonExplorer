namespace DungeonExplorer.Items
{
    /// <summary>
    /// Defines the <see cref="MiscItem" />
    /// </summary>
    public class MiscItem : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiscItem"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="description">The description<see cref="string"/></param>
        public MiscItem(string name, string description)

            : base(name, description)
        {

            IsUsable = false;
        }
    }

}
