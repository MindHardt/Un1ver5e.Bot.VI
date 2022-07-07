using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qmmands;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.Text;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Dice;
using Un1ver5e.Bot.Services.RateOptionsProvider;

namespace Un1ver5e.Bot.Commands
{
    public class BasicApplicationModule : DiscordApplicationModuleBase
    {
        //RATE
        [MessageCommand("Оценить")]
        [RequireGuild]
        public async ValueTask<IResult> RateCommandAsync(IMessage message)
        {
            await Deferral(false);
            //Just ensuring

            int randomSeed = (int)message.Id.RawValue;
            string rateoption = Bot.Services.GetRequiredService<IRateOptionsProvider>().GetOption(new Random(randomSeed));
            //Each call on the same message will result the same

            ulong messageID = message.Id;
            ulong channelID = Context.ChannelId;
            ulong guildID = Context.GuildId!.Value;

            string msgLink = $"[Исходное сообщение](https://discord.com/channels/{guildID}/{channelID}/{messageID})";
            //Link to original message

            LocalEmbed embed = new()
            {
                Title = rateoption,
                Description = msgLink
            };

            return Response(embed);
        }


        //AVATAR
        [UserCommand("Аватар")]
        [RequireGuild]
        public IResult AvatarCommand(IMember member)
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

            using HttpClient client = new();
            Stream pic = await client.GetStreamAsync(url);

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .AddAttachment(new(pic, "generated.jpg"))
                .AddEmbed(new LocalEmbed()
                    .WithTitle($"Источник: {url}")
                    .WithImageUrl("attachment://generated.jpg"));

            return Response(resp);
        }


        //DICE
        [SlashCommand("dice")]
        [Description("Бросает кубик по текстовому описанию.")]
        public IResult DiceCommand(
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


        //PING
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
