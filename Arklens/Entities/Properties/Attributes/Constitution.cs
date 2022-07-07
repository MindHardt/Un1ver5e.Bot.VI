namespace Arklens.Entities.Properties.Attributes
{
    public record Constitution : AttributeStat
    {
        public override string Acronym => "ВЫН";

        public static implicit operator Constitution(int value)
        {
            return new Constitution() { Value = value };
        }
    }
}
