using Disqord;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Un1ver5e.Bot.Services.RateOptionsProvider;

namespace Un1ver5e.Bot.Commands
{
    public class RateCommand : DiscordApplicationGuildModuleBase
    {
        //RATE
        [MessageCommand("Оценить")]
        public async ValueTask<IResult> RateCommandAsync(IMessage message)
        {
            await Deferral(false);
            //Just ensuring

            int randomSeed = (int)message.Id.RawValue;
            string rateoption = Bot.Services.GetRequiredService<IRateOptionsProvider>().GetOption(new Random(randomSeed));
            //Each call on the same message will result the same


            string msgLink = $"[Исходное сообщение]({Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, message.Id)})";
            //Link to original message

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(rateoption)
                    .WithDescription(msgLink))
                .AddComponent(DeleteThisButtonCommandModule.GetDeleteButtonRow());

            return Response(response);
        }
    }
}
