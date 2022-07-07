using Arklens.Entities;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Services.Arklens
{
    public class ArklensCommandsModule : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("al-test")]
        [Description("Тест аркленса")]
        public IResult TestCharacter()
        {
            return Response(new Character().CreateDisplay());
        }
    }
}
