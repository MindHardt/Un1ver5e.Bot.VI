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
    }
}
