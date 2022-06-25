using Disqord;
using Disqord.Bot;
using Disqord.Bot.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace Un1ver5e.Bot
{
    internal class Un1ver5eBot : DiscordBot
    {
        protected override LocalMessageBase? CreateFailureMessage(IDiscordCommandContext context)
        {
            if (context is IDiscordInteractionCommandContext)
                return new LocalInteractionMessageResponse().WithIsEphemeral(true);

            return new LocalMessage();
        }

        protected override string? FormatFailureReason(IDiscordCommandContext context, IResult result) => result switch
        {
            CommandNotFoundResult
                => $"Команда не найдена!",

            TypeParseFailedResult res 
                => $"Не удалось преобразовать аргумент **{res.Parameter.Name}**\n Текст:`{new string(res.Value.ToArray())}`",

            ChecksFailedResult res 
                => "Не пройдены проверки:\n" + string.Join('\n', res.FailedChecks.Select(x => x.Value.FailureReason)).AsCodeBlock(),

            ParameterChecksFailedResult res 
                => $"Аргумент не прошел проверку(и) **{res.Parameter.Name}**\n"
                + string.Join('\n', res.FailedChecks.Select(x => x.Value.FailureReason)).AsCodeBlock(),

            ExceptionResult res
                => $"Произошла ошибка: **{res.Exception.Message}**",

            _ => result.FailureReason
        };

        protected override ValueTask<IResult> OnBeforeExecuted(IDiscordCommandContext context)
        {
            return base.OnBeforeExecuted(context);
        }

        /// <summary>
        /// The default ctor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="services"></param>
        /// <param name="client"></param>
        public Un1ver5eBot(
        IOptions<DiscordBotConfiguration> options,
        ILogger<DiscordBot> logger,
        IServiceProvider services,
        DiscordClient client)
        : base(options, logger, services, client)
        { }
    }
}
