namespace Arklens.Entities.Properties.Attributes
{
    public record Dexterity : AttributeStat
    {
        public Dexterity(int value) : base(value)
        {
        }

        public override string Acronym => "ЛВК";

        public static implicit operator Dexterity(int value) => new(value);
    }
}
