using Disqord;
using Disqord.Bot.Commands;

namespace Un1ver5e.Bot.Models
{
    public record DefaultStashData : IStashData
    {
        public IMessage Message { get; }
        public IDiscordCommandContext Context { get; }

        public DefaultStashData(IMessage message, IDiscordCommandContext context)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
