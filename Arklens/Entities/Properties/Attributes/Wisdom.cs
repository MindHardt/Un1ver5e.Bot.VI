namespace Arklens.Entities.Properties.Attributes
{
    public record Wisdom : AttributeStat
    {
        public Wisdom(int value) : base(value)
        {
        }

        public override string Acronym => "МДР";

        public static implicit operator Wisdom(int value) => new(value);
    }
}
