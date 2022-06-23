using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Rest;
using Microsoft.Extensions.Configuration;
using Qmmands;
using System.Text;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Dice;

namespace Un1ver5e.Bot.Services
{
    public class BasicApplicationModule : DiscordApplicationModuleBase
    {
        private readonly string[] _rateopts;
        private readonly IDiceService _dice;
        private readonly BotContext _dbctx;
        public BasicApplicationModule(IConfiguration config, IDiceService dice, BotContext dbctx)
        {
            _rateopts = config.GetSection("rate_options").Get<string[]>();
            _dice = dice;
            _dbctx = dbctx;
        }

        //RATE
        [MessageCommand("Rate")]
        [RequireGuild]
        public async ValueTask<IResult> RateCommandAsync(IMessage message)
        {
            await Deferral(false);

            int randomSeed = (int)message.Id.RawValue;

            ulong messageID = message.Id;
            ulong channelID = message.ChannelId;
            ulong guildID = ((await Bot.FetchChannelAsync(channelID)) as IGuildChannel)!.GuildId;

            string msgLink = $"[Исходное сообщение](https://discord.com/channels/{guildID}/{channelID}/{messageID})";

            LocalEmbed embed = new()
            {
                Title = _rateopts.GetRandomElement(new Random(randomSeed)),
                Description = msgLink
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

        //SQLSCRIPT
        [SlashCommand("sqlscript")]
        [Description("Это для создателя")]
        public IResult SqlScriptCommand()
        {
            string script = _dbctx.GetCreateScript();

            byte[] asBytes = Encoding.UTF8.GetBytes(script);

            Stream stream = new MemoryStream(asBytes);
            stream.Position = 0;

            LocalAttachment file = new LocalAttachment(stream, "script.sql");

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .WithIsEphemeral(true)
                .AddAttachment(file);

            return Response(resp);
        }
    }
}
