namespace Arklens.Entities.Properties.Attributes
{
    public record Intelligence : AttributeStat
    {
        public Intelligence(int value) : base(value)
        {
        }

        public override string Acronym => "ИНТ";

        public static implicit operator Intelligence(int value) => new(value);
    }
}
