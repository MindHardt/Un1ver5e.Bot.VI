using Arklens.Entities.Properties.Attributes;

namespace Arklens.Entities
{
    public /*abstract*/ record Character : ICreature
    {
        //ATTRIBUTES
        public Strength Strength { get; set; } = 10;

        public Dexterity Dexterity { get; set; } = 10;

        public Constitution Constitution { get; set; } = 10;

        public Intelligence Intelligence { get; set; } = 10;

        public Wisdom Wisdom { get; set; } = 10;

        public Charisma Charisma { get; set; } = 10;

        public string Name { get; set; } = "John Doe";
        public string AvatarUrl { get; set; } = "https://i.ibb.co/cNpGGcc/Face.png";
    }
}
