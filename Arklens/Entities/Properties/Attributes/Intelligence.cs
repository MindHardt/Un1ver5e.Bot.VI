namespace Arklens.Entities.Properties.Attributes
{
    public record Intelligence : AttributeStat
    {
        public override string Acronym => "ИНТ";

        public static implicit operator Intelligence(int value)
        {
            return new Intelligence() { Value = value };
        }
    }
}
