using Disqord;
using Disqord.Bot.Commands;

namespace Un1ver5e.Bot.Models
{
    /// <summary>
    /// Represents data that is being saved with <see cref="StashCommandModule.RememberThis(IMessage)"/> command.
    /// </summary>
    public interface IStashData
    {
        public IMessage Message { get; }
        public IDiscordCommandContext Context { get; }

        private static Dictionary<Snowflake, IStashData> stashData = new();

        /// <summary>
        /// Gets a <see cref="IStashData"/> object which is associated with <paramref name="snowflake"/> if it exists.
        /// </summary>
        /// <param name="snowflake"></param>
        /// <returns></returns>
        public static IStashData? GetStashData(Snowflake snowflake) => stashData.GetValueOrDefault(snowflake);
        /// <summary>
        /// Stashed <paramref name="data"/> and associates it with <paramref name="snowflake"/>.
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="data"></param>
        public static void Stash(Snowflake snowflake, IStashData data) => stashData[snowflake] = data;
    }
}
