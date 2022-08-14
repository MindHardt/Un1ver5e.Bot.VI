namespace Arklens.Entities.Properties.Attributes
{
    public record Strength : AttributeStat
    {
        public Strength(int value) : base(value)
        {
        }

        public override string Acronym => "СИЛ";

        public static implicit operator Strength(int value) => new(value);
    }
}
