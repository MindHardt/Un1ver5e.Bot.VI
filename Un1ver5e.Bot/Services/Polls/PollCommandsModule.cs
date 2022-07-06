using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity.Menus;
using Qmmands;
using Un1ver5e.Bot.Services.Polls;

namespace Un1ver5e.Bot.Services
{
    public class PollCommandsModule : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("poll")]
        [Description("Голосовалки")]
        public IResult StartPollCommand(
            [Name("Название"), Description("Текст сверху опроса")] string header,
            [Name("Тип"), Description("Ну вы сами знаете")]
            [Choice("Анонимное", "true"), Choice("Публичное", "false")] string anonimous,
            [Name("Время"), Description("Время через которое завершится опрос. Не более суток")] string timeStamp,
            [Name("Варианты"), Description("Опции через запятую")] string options)
        {
            if (TimeSpan.TryParse(timeStamp, out TimeSpan time) == false && time > TimeSpan.FromHours(24)) throw new ArgumentOutOfRangeException(nameof(timeStamp));

            bool isAnon = anonimous == "true";
            return Menu(new DefaultInteractionMenu(new PollView(header, isAnon, time, options.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)), Context.Interaction)
            {
                AuthorId = null
            });
        }
    }
}
