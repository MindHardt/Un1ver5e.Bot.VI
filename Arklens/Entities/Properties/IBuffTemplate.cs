namespace Arklens.Entities.Properties
{
    public interface IBuffTemplate
    {
        public IEnumerable<ICreature> Targets { get; }
        public Func<ICreature, IEnumerable<IStat>> StatProvider { get; }
        public abstract void Apply();
    }
}
