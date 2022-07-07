namespace Arklens.Entities.Properties.Attributes
{
    public record Dexterity : AttributeStat
    {
        public override string Acronym => "ЛВК";

        public static implicit operator Dexterity(int value)
        {
            return new Dexterity() { Value = value };
        }
    }
}
