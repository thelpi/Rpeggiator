namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a permanent structure.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PermanentStructure : Sprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="datas">Dynamic json datas.</param>
        public PermanentStructure(dynamic datas) : base((object)datas) { }
    }
}
