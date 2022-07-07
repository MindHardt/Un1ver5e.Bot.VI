namespace Arklens.Entities.Properties.Attributes
{
    public record Strength : AttributeStat
    {
        public override string Acronym => "СИЛ";

        public static implicit operator Strength(int value)
        {
            return new Strength() { Value = value };
        }
    }
}
