using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Commands
{
    public class EmojiCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("emoji")]
        [Description("Дает ссылку на изображение эмоджи.")]
        public IResult Emoji(
            [Name("Эмоджи"), Description("Интересующий кастомный эмоджи.")] ICustomEmoji emoji)
        {
            string url = emoji.GetUrl(CdnAssetFormat.Png, 128);
            string name = emoji.Name!;

            LocalEmbed embed = new()
            {
                ImageUrl = url,
                Description = url,
                Title = $"Ссылка на {name}"
            };

            return Response(embed);
        }


    }
}
