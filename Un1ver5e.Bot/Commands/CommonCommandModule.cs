using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity.Menus;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Diagnostics;
using Un1ver5e.Bot.Commands.DeleteThisButton;
using Un1ver5e.Bot.Models;
using Un1ver5e.Bot.Services.Dice;
using Un1ver5e.Bot.Services.Polls;

namespace Un1ver5e.Bot.Commands
{
    public class CommonCommandModule : DiscordApplicationGuildModuleBase
    {
        //CHOOSE
        [SlashCommand("выбери")]
        [Description("Случайно выбирает из предложенных вариантов.")]
        public IResult Choose(
            [Name("Варианты")]
            [Description("Любое количество вариантов (разделяются запятой).")]
            string options)
        {
            string[] parsedOptions = options.Split(',');

            return Response(new LocalEmbed()
                .WithDescription($"Я выбираю `{parsedOptions.GetRandomElement()}`!")
                .AddField(new LocalEmbedField()
                    .WithName("Варианты")
                    .WithValue(string.Join('\n', parsedOptions).AsCodeBlock())));
        }

        //EMOJI
        [SlashCommand("эмоджи")]
        [Description("Дает ссылку на изображение эмоджи.")]
        public IResult Emoji(
            [Name("Эмоджи")] 
            [Description("Интересующий кастомный эмоджи.")] 
            ICustomEmoji emoji)
        {
            string url = emoji.GetUrl();
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
            Stream image = await client.GetStreamAsync(url);
            const string fileName = "generated.jpg";

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .AddAttachment(new(image, fileName))
                .AddEmbed(new LocalEmbed()
                    .WithTitle($"Источник: {url}")
                    .WithImageUrl($"attachment://{fileName}"));

            return Response(resp);
        }

        //PING
        [SlashCommand("пинг")]
        [Description("Проверяем живой ли бот")]
        public IResult Ping()
        {
            TimeSpan latency = DateTimeOffset.Now - Context.Interaction.CreatedAt();
            DateTimeOffset launchTime = Process.GetCurrentProcess().StartTime;

            string launchTimeStamp = launchTime.ToRelativeDiscordTime();
            string latencyTimeStamp = $"Задержка сокета `{((int)latency.TotalMilliseconds)}`мс.";


            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(latencyTimeStamp)
                    .AddField(new LocalEmbedField()
                        .WithName("Бот запущен")
                        .WithValue(launchTimeStamp)))
                .AddDeleteThisButton();

            return Response(response);
        }

        //POLL
        [SlashCommand("опрос")]
        [Description("Голосовалки")]
        public IResult StartPollCommand(
            [Name("Название"), Description("Текст сверху опроса")] string header,
            [Name("Тип"), Description("Ну вы сами знаете")]
            [Choice("Анонимное", "true"), Choice("Публичное", "false")] string anonymous,
            [Name("Время"), Description("Время через которое завершится опрос. Не более суток")] string timeStamp,
            [Name("Варианты"), Description("Опции через запятую")] string options)
        {
            if (TimeSpan.TryParse(timeStamp, out TimeSpan time) == false && time > TimeSpan.FromHours(24)) return Results.Failure("Неверный формат времени!");

            bool isAnon = anonymous == "true";
            return Menu(new DefaultInteractionMenu(new PollView(header, isAnon, time, options.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)), Context.Interaction)
            {
                AuthorId = null
            });
        }

        //RATE
        [MessageCommand("Оценить")]
        public async ValueTask<IResult> RateCommandAsync(IMessage message)
        {
            await Deferral(false);
            //Just ensuring

            int randomSeed = (int)message.Id.RawValue;
            string rateoption = await Bot.Services.GetRequiredService<IRateOptionsProvider>().GetOptionAsync(new Random(randomSeed));
            //Each call on the same message will result the same


            string msgLink = $"[Исходное сообщение]({Disqord.Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, message.Id)})";
            //Link to original message

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithTitle(rateoption)
                    .WithDescription(msgLink))
                .AddDeleteThisButton();

            return Response(response);
        }

        //QUOTE COPY
        [MessageCommand("Цитировать")]
        public IResult RememberThis(IMessage msg)
        {
            var storage = Context.Services.GetRequiredService<IStashStorage<Snowflake>>();

            IStashData data = new DefaultStashData(msg, Context);
            storage.Stash(Context.AuthorId, data);

            var response = new LocalInteractionMessageResponse()
                .AddEmbed(msg.GetDisplay(Context.GuildId))
                .AddDeleteThisButton();

            return Response(response);
        }

        //QUOTE PASTE
        [SlashCommand("цитата")]
        [Description("Показывает вашу сохраненку (Команда \"Цитата\")")]
        public IResult RemindMe()
        {
            var storage = Context.Services.GetRequiredService<IStashStorage<Snowflake>>();

            IStashData? data = storage.Get(Context.AuthorId);

            if (data is null) return Results.Failure("Сохраненка не найдена");

            var response = new LocalInteractionMessageResponse()
                .AddEmbed(data.Message.GetDisplay(data.Context.GuildId))
                .AddDeleteThisButton(Context.AuthorId);

            return Response(response);
        }

        //DICE
        [SlashCommand("дайс")]
        [Description("Бросает кубик по текстовому описанию.")]
        public IResult Dice(
            [Name("кубик")]
            [Description("Текст кубика, например \"2d6\" или \"1d4+2\".")] 
            string dice)
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
