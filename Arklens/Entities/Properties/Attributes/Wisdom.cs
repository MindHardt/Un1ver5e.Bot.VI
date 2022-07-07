namespace Arklens.Entities.Properties.Attributes
{
    public record Wisdom : AttributeStat
    {
        public override string Acronym => "МДР";

        public static implicit operator Wisdom(int value)
        {
            return new Wisdom() { Value = value };
        }
    }
}
