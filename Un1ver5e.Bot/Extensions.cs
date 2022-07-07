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
            Queue<string> strings = new Queue<string>(collection);

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
    }
}
