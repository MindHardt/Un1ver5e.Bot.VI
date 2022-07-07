namespace Arklens.Entities.Properties.Attributes
{
    public record Charisma : AttributeStat
    {
        public override string Acronym => "ХАР";

        public static implicit operator Charisma(int value)
        {
            return new Charisma() { Value = value };
        }
    }
}
