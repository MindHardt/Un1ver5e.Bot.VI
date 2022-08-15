using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Bot.Commands.Text;
using Qmmands;
using Qmmands.Text;
using Qommon;
using Un1ver5e.Bot.Commands.DeleteThisButton;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Commands
{
    public class StashApplicationCommandModule : DiscordApplicationGuildModuleBase
    {
        private readonly IStashStorage<Snowflake> _storage;

        public StashApplicationCommandModule(IStashStorage<Snowflake> storage)
        {
            _storage = storage;
        }

        [MessageCommand("Запомнить")]
        public IResult RememberThis(IMessage msg)
        {
            IStashData data = new DefaultStashData(msg, Context);
            _storage.Stash(Context.AuthorId, data);

            var response = new LocalInteractionMessageResponse()
                .AddEmbed(msg.GetDisplay(Context.GuildId))
                .AddDeleteThisButton();

            return Response(response);
        }

        [SlashCommand("вспомни")]
        [Description("Показывает вашу сохраненку (Команда \"Запомнить\")")]
        public IResult RemindMe()
        {
            IStashData? data = _storage.Get(Context.AuthorId);

            if (data is null) return Results.Failure("Сохраненка не найдена");

            var response = new LocalInteractionMessageResponse()
                .AddEmbed(data.Message.GetDisplay(data.Context.GuildId))
                .AddDeleteThisButton(Context.AuthorId);

            return Response(response);
        }
    }

    public class StashTextCommandModule : DiscordTextGuildModuleBase
    {
        private readonly IStashStorage<Snowflake> _storage;

        public StashTextCommandModule(IStashStorage<Snowflake> storage)
        {
            _storage = storage;
        }

        [TextCommand("цитата")]
        public IResult RememberThis()
        {
            IMessage? msg = Context.Message.ReferencedMessage.GetValueOrDefault();

            if (msg is null) return Results.Failure("Не удалось получить реплай.");

            IStashData data = new DefaultStashData(msg, Context);
            _storage.Stash(Context.AuthorId, data);

            var response = new LocalMessage()
                .AddEmbed(msg.GetDisplay(Context.GuildId))
                .AddComponent(DeleteThisButtonExtensions.GetDeleteButtonRow());

            return Response(response);
        }
    }
}
