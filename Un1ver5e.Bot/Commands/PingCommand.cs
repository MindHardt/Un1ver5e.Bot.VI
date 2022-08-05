using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;
using System.Diagnostics;

namespace Un1ver5e.Bot.Commands
{
    public class PingCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("ping")]
        [Description("Проверяем живой ли бот")]
        public IResult Ping()
        {
            TimeSpan latency = DateTimeOffset.Now - Context.Interaction.CreatedAt();
            DateTimeOffset launchTime = Process.GetCurrentProcess().StartTime;

            string launchTimeStamp = $"<t:{launchTime.ToUnixTimeSeconds()}:R>";
            string latencyTimeStamp = $"Задержка сокета `{((int)latency.TotalMilliseconds)}`мс.";


            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(latencyTimeStamp)
                    .AddField(new LocalEmbedField()
                        .WithName("Бот запущен")
                        .WithValue(launchTimeStamp)))
                .AddComponent(LocalComponent.Row(DeleteThisButtonCommandModule.GetDeleteButton()));

            return Response(response);
        }
    }
}
