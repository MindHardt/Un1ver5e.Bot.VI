using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Webhook;
using Qmmands;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Tags;
using Un1ver5e.Bot.Services.Webhooks;

namespace Un1ver5e.Bot.Services
{
    public class TagsCommandsModule : DiscordApplicationModuleBase
    {
        private readonly BotContext _dbctx;

        public TagsCommandsModule(BotContext dbctx)
        {
            _dbctx = dbctx;
        }

        //Create
        [MessageCommand("Create Tag")]
        public async ValueTask<IResult> BroadCastCommand(IMessage msg)
        {
            if (msg is not IUserMessage usermsg) throw new ArgumentException();

            await Deferral(true);

            Tag tag = new(usermsg, Context.Author);

            await _dbctx.Tags.AddAsync(tag);
            await _dbctx.SaveChangesAsync();

            return Response(tag.Name);
        }
    }
}
