namespace Arklens.Entities.Properties.Attributes
{
    public record Charisma : AttributeStat
    {
        public Charisma(int value) : base(value)
        {
        }

        public override string Acronym => "ХАР";

        public static implicit operator Charisma(int value) => new(value);
    }
}
