namespace Un1ver5e.Bot.Services.Dice
{
    /// <summary>
    /// Represents service capable of throwing dices from text description.
    /// </summary>
    public interface IDiceService
    {
        /// <summary>
        /// Throws dice defined by text query, allowing modifyers
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IThrowResult ThrowByQuery(string query);

        /// <summary>
        /// Creates a <see cref="IDice"/> from text definition.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IDice? CreateDice(string text);
    }
}
