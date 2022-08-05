using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Services.Database
{
    public static class BotContextExtensions
    {
        /// <summary>
        /// Excludes non-public <see cref="Tag"/>s that belong to any guild other than <paramref name="guildId"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public static IEnumerable<Tag> ThatAreSeenIn(this IEnumerable<Tag> source, ulong guildId) => source.Where(tag => tag.IsPublic || tag.GuildId == guildId);
    }
}
