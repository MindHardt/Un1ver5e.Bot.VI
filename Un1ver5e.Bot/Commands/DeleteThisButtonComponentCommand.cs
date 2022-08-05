using Disqord;
using Disqord.Bot.Commands.Components;
using Disqord.Rest;

namespace Un1ver5e.Bot.Commands
{
    public class DeleteThisButtonComponentCommand : DiscordComponentModuleBase
    {
        private const string DeleteButtonCustomId = "mo_delete_this";

        /// <summary>
        /// Gets a button which will delete its message upon press.
        /// </summary>
        /// <returns></returns>
        public static LocalButtonComponent GetDeleteButton() => new()
        {
            Emoji = LocalEmoji.Unicode("🗑"),
            CustomId = DeleteButtonCustomId,
            Style = LocalButtonComponentStyle.Danger
        };

        /// <summary>
        /// Gets a button which will delete its message upon press.
        /// </summary>
        /// <returns>A <see cref="LocalRowComponent"/> which only contains delete button.</returns>
        public static LocalRowComponent GetDeleteButtonRow() => LocalComponent.Row(GetDeleteButton());

        [ButtonCommand(DeleteButtonCustomId)]
        public async ValueTask DeleteMessageButtonCommand()
        {
            IMessage msg = (Context.Interaction as IComponentInteraction)!.Message;

            await Bot.DeleteMessageAsync(msg.ChannelId, msg.Id);
        }
    }
}
