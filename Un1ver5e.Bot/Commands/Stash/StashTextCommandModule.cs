using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Bot.Commands.Text;
using Qmmands;
using Qmmands.Text;
using Qommon;
using Un1ver5e.Bot.Commands.DeleteThisButton;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Commands.Stash
{

    public class StashTextCommandModule : DiscordTextGuildModuleBase
    {
        private readonly IStashStorage<Snowflake> _storage;

        public StashTextCommandModule(IStashStorage<Snowflake> storage)
        {
            _storage = storage;
        }

        [TextCommand("запомни")]
        public IResult RememberThis()
        {
            IMessage? msg = Context.Message.ReferencedMessage.GetValueOrDefault();

            if (msg is null) return Results.Failure("Не удалось получить реплай.");

            IStashData data = new DefaultStashData(msg, Context);
            _storage.Stash(Context.AuthorId, data);

            var response = new LocalMessage()
                .AddEmbed(msg.GetDisplay(Context.GuildId))
                .AddComponent(DeleteThisButtonExtensions.GetDeleteButtonRow());

            return Response(response);
        }
    }
}
