using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Commands
{
    public class GenerateCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("сгенерируй")]
        [Description("Нейросетки делают картинки")]
        public async ValueTask<IResult> Generate(
            [Name("Тема"), Description("Картинку чего генерируем.")]
            [Choice("котик",     "https://thiscatdoesnotexist.com/")]
            [Choice("лошадь",    "https://thishorsedoesnotexist.com/")]
            [Choice("картина",   "https://thisartworkdoesnotexist.com/")]
            [Choice("человек",   "https://thispersondoesnotexist.com/image")]
            string url)
        {
            await Deferral(isEphemeral: false);

            using HttpClient client = new();
            Stream pic = await client.GetStreamAsync(url);

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .AddAttachment(new(pic, "generated.jpg"))
                .AddEmbed(new LocalEmbed()
                    .WithTitle($"Источник: {url}")
                    .WithImageUrl("attachment://generated.jpg"));

            return Response(resp);
        }


    }
}
