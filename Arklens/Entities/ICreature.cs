using Arklens.Entities.Properties.Attributes;

namespace Arklens.Entities
{
    public interface ICreature
    {
        public Strength Strength { get; }
        public Dexterity Dexterity { get; }
        public Constitution Constitution { get; }
        public Intelligence Intelligence { get; }
        public Wisdom Wisdom { get; }
        public Charisma Charisma { get; }


        public string AvatarUrl { get; set; }
    }
}
