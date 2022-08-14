using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Diagnostics;
using Un1ver5e.Bot.Services.Dice;

namespace Un1ver5e.Bot.Commands
{
    public class DiceCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("дайс")]
        [Description("Бросает кубик по текстовому описанию.")]
        public IResult Dice(
            [Name("кубик"), Description("Текст кубика, например \"2d6\" или \"1d4+2\".")] string dice)
        {
            IThrowResult result = Bot.Services.GetRequiredService<IDiceService>().ThrowByQuery(dice);

            LocalEmbed embed = new LocalEmbed()
                .WithTitle($"> `{result.GetCompleteSum()}` 🎲 ")
                .AddField(new LocalEmbedField()
                    .WithValue(result.ToString()!.AsCodeBlock())
                    .WithName(dice));

            return Response(embed);
        }
    }
}
