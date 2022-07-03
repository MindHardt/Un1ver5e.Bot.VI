using Disqord;
using Disqord.Bot.Commands.Components;
using Disqord.Rest;

namespace Un1ver5e.Bot.Commands
{
    public class DeleteThisButtonCommandModule : DiscordComponentModuleBase
    {
        private const string DeleteButtonCustomId = "mo_delete_this";
        public static LocalButtonComponent GetDeleteButton() => new()
        {
            Emoji = LocalEmoji.Unicode("🗑"),
            CustomId = DeleteButtonCustomId,
            Style = LocalButtonComponentStyle.Danger
        };

        [ButtonCommand(DeleteButtonCustomId)]
        public async ValueTask DeleteMessageButtonCommand()
        {
            IMessage msg = (Context.Interaction as IComponentInteraction)!.Message;

            await Bot.DeleteMessageAsync(msg.ChannelId, msg.Id);
        }
    }
}
