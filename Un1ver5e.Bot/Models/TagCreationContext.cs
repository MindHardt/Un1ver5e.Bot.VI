using Disqord;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Models
{
    public class TagCreationContext
    {
        public IInteraction OriginalInteraction { get; init; }
        public Tag RawTag { get; }

        public void AssignName(string name) => RawTag.Name = name;

        public async ValueTask SaveIn(BotContext ctx)
        {
            ctx.Tags.Add(RawTag);
            await ctx.SaveChangesAsync();
        }

        public TagCreationContext(IInteraction originalInteraction, string content, ulong authorId, ulong guildId)
        {
            OriginalInteraction = originalInteraction;
            RawTag = new Tag(content, authorId, guildId);
        }
    }
}
