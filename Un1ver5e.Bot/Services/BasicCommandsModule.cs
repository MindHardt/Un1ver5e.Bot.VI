using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.Configuration;
using Qmmands;
using Un1ver5e.Bot.Services.Dice;

namespace Un1ver5e.Bot.Services
{
    public class BasicCommandsModule : DiscordApplicationModuleBase
    {
        private readonly string[] _rateopts;
        private readonly WebHookFeed _feed;
        private readonly IDiceService _dice;
        public BasicCommandsModule(IConfiguration config, WebHookFeed feed, IDiceService dice)
        {
            _rateopts = config.GetSection("rate_options").Get<string[]>();
            this._feed = feed;
            this._dice = dice;
        }

        //RATE
        [MessageCommand("Rate")]
        public IResult RateCommand(IMessage message)
        {
            int randomSeed = (int)message.Id.RawValue;

            LocalEmbed embed = new()
            {
                Title = _rateopts.GetRandomElement(new Random(randomSeed))
            };

            return Response(embed);
        }

        //AVATAR
        [SlashCommand("avatar")]
        [Description("Аватар")]
        [RequireGuild]
        public IResult AvatarCommand(
            [Name("Пользователь"), Description("Тот, чью аватарку безбожно воруем.")] IMember member)
        {
            string url = member.GetAvatarUrl(CdnAssetFormat.Png, 1024);
            string nick = member.Nick ?? member.Name;

            LocalEmbed embed = new()
            {
                ImageUrl = url,
                Description = url,
                Title = $"Аватар {nick}"
            };

            return Response(embed);
        }

        //EMOJI
        [SlashCommand("emoji")]
        [Description("Дает ссылку на изображение эмоджи.")]
        [RequireGuild]
        public IResult EmojiCommand(
            [Name("Эмоджи"), Description("Интересующий кастомный эмоджи.")] ICustomEmoji emoji)
        {
            string url = emoji.GetUrl(CdnAssetFormat.Png, 128);
            string name = emoji.Name;

            LocalEmbed embed = new()
            {
                ImageUrl = url,
                Description = url,
                Title = $"Ссылка на {name}"
            };

            return Response(embed);
        }

        //GENERATE
        [SlashCommand("generate")]
        [Description("Нейросетки делают картинки")]
        public async ValueTask<IResult> GenerateCommand(
            [Name("Тема"), Description("Картинку чего генерируем.")]
            [Choice("котик",     "https://thiscatdoesnotexist.com/")]
            [Choice("лошадь",    "https://thishorsedoesnotexist.com/")]
            [Choice("картина",   "https://thisartworkdoesnotexist.com/")]
            string url)
        {
            await Deferral(isEphemeral: false);

            using (HttpClient client = new())
            {
                Stream pic = await client.GetStreamAsync(url);

                //TODO: put image into embed
                LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                    .AddAttachment(new(pic, "generated.jpg"))
                    .AddEmbed(new LocalEmbed()
                        .WithTitle($"Источник: {url}")
                        .WithImageUrl("attachment://generated.jpg"));

                return Response(resp);
            }
        }

        //BROADCAST
        [SlashCommand("broadcast")]
        [Description("Новости.")]
        [RequireBotOwner]
        public async ValueTask<IResult> BroadCastCommand(
            [Name("Текст"), Description("Текст сообщения.")] string text)
        {
            LocalWebhookMessage msg = new LocalWebhookMessage().WithContent(text);

            await _feed.SendMessage(msg);

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .WithIsEphemeral(true)
                .WithContent("Успешно 📢");

            return Response(resp);
        }

        //DICE
        [SlashCommand("dice")]
        [Description("Бросает кубик по текстовому описанию.")]
        public IResult DiceCommand(
            [Name("кубик"), Description("Текст кубика, например \"2d6\" или \"1d4+2\".")] string dice)
        {
            IThrowResult result = _dice.ThrowByQuery(dice);

            LocalEmbed embed = new LocalEmbed()
                .WithTitle($"> 🎲 `{result.GetCompleteSum()}`")
                .AddField(new LocalEmbedField()
                    .WithValue(result.ToString()!.AsCodeBlock())
                    .WithName(dice));

            return Response(embed);
        }

    }
}
