using Disqord;
using Disqord.Bot.Commands.Text;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Qmmands.Text;
using Qommon;
using Un1ver5e.Bot.Commands.DeleteThisButton;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Commands.Rate
{
    public class RateTextCommandModule : DiscordTextGuildModuleBase
    {
        [TextCommand("оцени")]
        public IResult RateCommand()
        {
            IMessage? message = Context.Message.ReferencedMessage.GetValueOrDefault();

            if (message is null) return Results.Failure("Не удалось получить реплай.");

            int randomSeed = (int)message.Id.RawValue;
            string rateoption = Bot.Services.GetRequiredService<IRateOptionsProvider>().GetOption(new Random(randomSeed));
            //Each call on the same message will result the same


            string msgLink = $"[Исходное сообщение]({Disqord.Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, message.Id)})";
            //Link to original message

            LocalMessage response = new LocalMessage()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(rateoption)
                    .WithDescription(msgLink))
                .AddDeleteThisButton();

            return Response(response);
        }
    }
}
