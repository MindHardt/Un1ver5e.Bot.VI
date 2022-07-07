namespace Arklens.Entities.Properties
{
    public interface IBuff
    {
        public string Source { get; }
        public abstract int Value { get; }
    }
}
