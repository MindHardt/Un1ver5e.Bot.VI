using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Bot.Commands.Text;
using Qmmands;
using Qmmands.Text;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Commands
{
    internal static class StashCommandsHelper
    {
        /// <summary>
        /// Gets a message that informs user that his data is saved ok.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="jumpUrl"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public static TMessage GetSavedOkMessage<TMessage>(string jumpUrl, IUser author)
            where TMessage : LocalMessageBase, new() => new TMessage()
                .AddEmbed(new LocalEmbed()
                    .WithDescription($"Сохранил для вас [Сообщение]({jumpUrl})")
                    .WithAuthor(author))
                .AddComponent(DeleteThisButtonCommandModule.GetDeleteButtonRow());

        /// <summary>
        /// Gets a message that says user that they have no data stashed.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public static TMessage GetNoStashFoundMessage<TMessage>()
            where TMessage : LocalMessageBase, new() => new TMessage()
                .AddEmbed(new LocalEmbed()
                    .WithDescription("🛑 У вас нет сохраненки! Чтобы создать ее используйте контекстную команду \"Запомнить\""))
                .AddComponent(DeleteThisButtonCommandModule.GetDeleteButtonRow());

        /// <summary>
        /// Gets a message that containes link to a stashed message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="jumpUrl"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public static TMessage GetStashedMessageLink<TMessage>(string jumpUrl, IUser author)
            where TMessage : LocalMessageBase, new() => new TMessage()
                .AddEmbed(new LocalEmbed()
                    .WithDescription($"Ваше сохраненное [Сообщение]({jumpUrl})")
                    .WithAuthor(author))
                .AddComponent(DeleteThisButtonCommandModule.GetDeleteButtonRow());
    }

    public class StashApplicationCommandModule : DiscordApplicationGuildModuleBase
    {
        [MessageCommand("Запомнить")]
        public IResult RememberThis(IMessage msg)
        {
            IStashData data = new DefaultStashData(msg, Context);
            IStashData.Stash(Context.AuthorId, data);

            string jumpUrl = Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, msg.Id);

            var response = StashCommandsHelper.GetSavedOkMessage<LocalInteractionMessageResponse>(jumpUrl, Context.Author);
            return Response(response);
        }

        [SlashCommand("stash")]
        [Description("Показывает вашу сохраненку (Команда \"Запомнить\")")]
        public IResult RemindMe()
        {
            IStashData? data = IStashData.GetStashData(Context.AuthorId);

            if (data is null) return Response(StashCommandsHelper.GetNoStashFoundMessage<LocalInteractionMessageResponse>());

            string jumpUrl = Discord.MessageJumpLink(data.Context.GuildId, data.Context.ChannelId, data.Message.Id);
            var response = StashCommandsHelper.GetStashedMessageLink<LocalInteractionMessageResponse>(jumpUrl, Context.Author);

            return Response(response);
        }
    }

    public class StashTextCommandModule : DiscordTextGuildModuleBase
    {
        [TextCommand("запомни")]
        public IResult RememberThis()
        {
            if (Context.Message.ReferencedMessage.HasValue == false) throw new ArgumentException("Эта команда используется только в реплаях.");

            IMessage msg = Context.Message.ReferencedMessage.Value!;

            IStashData data = new DefaultStashData(msg, Context);
            IStashData.Stash(Context.AuthorId, data);

            string jumpUrl = Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, msg.Id);

            var response = StashCommandsHelper.GetSavedOkMessage<LocalMessage>(jumpUrl, Context.Author);
            return Response(response);
        }
    }
}
