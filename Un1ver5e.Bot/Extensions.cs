using Disqord;
using System.Text;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// Contains various extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Shuffles the collection, making it random-ordered. This is not lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The original collection.</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, Random? randomOverride = null)
        {
            Random random = randomOverride ?? Random.Shared;
            return collection.OrderBy((e) => random.Next());
        }

        /// <summary>
        /// Gets random element of a <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IEnumerable<T> collection, Random? randomOverride = null)
        {
            Random random = randomOverride ?? Random.Shared;
            return collection.MaxBy((e) => random.Next())!;
        }

        /// <summary>
        /// Formats string as a Discord Codeblock
        /// </summary>
        /// <param name="original">The original text</param>
        /// <param name="lang">The language used for formatting, i.e. "CS", "XML", "JSON"...</param>
        /// <returns></returns>
        public static string AsCodeBlock(this string original, string? lang = null) => $"```{lang ?? string.Empty}\n{original}```";

        /// <summary>
        /// Formats <paramref name="time"/> as a discord timestamp, which dynamically changes according to current system time of a discord user.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToRelativeDiscordTime(this DateTimeOffset time) => $"<t:{time.ToUnixTimeSeconds()}:R>";

        /// <summary>
        /// Chunks <paramref name="collection"/> into pages at maximum length of <paramref name="maxPageLen"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="maxPageLen"></param>
        /// <returns></returns>
        public static IList<string> ChunkAtPages(this IEnumerable<string> collection, int maxPageLen)
        {
            Queue<string> strings = new(collection);

            List<string> processed = new();

            StringBuilder sb = new(maxPageLen);
            while (strings.Any())
            {
                string current = strings.Dequeue();

                if (sb.Length + current.Length > maxPageLen)
                {
                    processed.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append(current);
            }
            processed.Add(sb.ToString());

            return processed;
        }

        /// <summary>
        /// Gets a <see cref="LocalEmbed"/> object which displays data about <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guildId">The guild id of the <paramref name="message"/>. Don't leave it null if message was inside a guild or the embed url will be broken.</param>
        /// <returns></returns>
        public static LocalEmbed GetDisplay(this IMessage message, Snowflake? guildId = null)
            => new LocalEmbed()
                .WithDescription(message.Content)
                .WithTimestamp(message.CreatedAt())
                .WithAuthor(message.Author)
                .AddField("Ссылка на сообщение", $"[Тык]({Disqord.Discord.MessageJumpLink(guildId, message.ChannelId, message.Id)})");
    }
}
