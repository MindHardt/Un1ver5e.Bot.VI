using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Diagnostics;
using Un1ver5e.Bot.Services.Dice;
using Un1ver5e.Bot.Services.RateOptionsProvider;

namespace Un1ver5e.Bot.Commands
{
    public class BasicApplicationModule : DiscordApplicationGuildModuleBase
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


            string msgLink = $"[Исходное сообщение]({Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, message.Id)})";
            //Link to original message

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(rateoption)
                    .WithDescription(msgLink))
                .AddComponent(DeleteThisButtonCommandModule.GetDeleteButtonRow());

            return Response(response);
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
            string name = emoji.Name!;

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
