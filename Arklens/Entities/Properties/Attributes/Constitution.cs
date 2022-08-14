namespace Arklens.Entities.Properties.Attributes
{
    public record Constitution : AttributeStat
    {
        public Constitution(int value) : base(value)
        {
        }

        public override string Acronym => "ВЫН";

        public static implicit operator Constitution(int value) => new(value);
    }
}
