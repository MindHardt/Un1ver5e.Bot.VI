using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Commands
{
    public class ChooseCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("выбери")]
        [Description("Когда выбрать ну ваще никак.")]
        public IResult Choose(
            [Name("Варианты")]
            [Description("Любое количество вариантов через пробел.")]
            string options)
        {
            string[] parsedOptions = options.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return Response(new LocalEmbed()
                .WithDescription($"Я выбираю `{parsedOptions.GetRandomElement()}`!")
                .AddField(new LocalEmbedField()
                    .WithName("Варианты")
                    .WithValue(string.Join('\n', parsedOptions).AsCodeBlock())));
        }
    }
}
