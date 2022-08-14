namespace Arklens.Entities.Properties.Attributes
{
    public abstract record AttributeStat : IStat
    {
        /// <summary>
        /// The root value got from throwing 3d6. Usually goes between 3..18
        /// </summary>
        public int Value { get; set; }
        public abstract string Acronym { get; }
        public IList<IBuff> Buffs { get; } = new List<IBuff>();

        /// <summary>
        /// Gets raw value, i.e. value calculated from only <see cref="Value"/>
        /// </summary>
        /// <returns></returns>
        public int GetRawValue() => (Value / 2) - 5;

        /// <summary>
        /// Gets full modifier of this <see cref="AttributeStat"/>.
        /// </summary>
        /// <returns></returns>
        public int GetSum()
        {
            return GetRawValue() + ((IStat)this).GetBuffSum();
        }

        public AttributeStat(int value)
        {
            Value = value;
        }
    }
}
