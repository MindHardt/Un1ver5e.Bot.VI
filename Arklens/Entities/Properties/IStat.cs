namespace Arklens.Entities.Properties
{
    public interface IStat
    {
        /// <summary>
        /// All buffs applied to this <see cref="IStat"/>.
        /// </summary>
        public IList<IBuff> Buffs { get; }

        /// <summary>
        /// Gets sum of this <see cref="IStat"/>.
        /// </summary>
        /// <returns></returns>
        public int GetSum();

        /// <summary>
        /// Gets sum of highest buffs of each source.
        /// </summary>
        /// <returns></returns>
        public int GetBuffSum()
        {
            return Buffs
                .GroupBy(b => b.Source)
                .Select(g => g.Max(b => b.Value))
                .Sum();
        }
    }
}
