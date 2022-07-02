﻿using Disqord;
using Disqord.Bot.Commands.Components;
using Disqord.Rest;

namespace Un1ver5e.Bot.Commands
{
    public class DeleteThisButtonCommandModule : DiscordComponentModuleBase
    {
        public static LocalButtonComponent GetDeleteButton() => new()
        {
            Emoji = LocalEmoji.Unicode("🗑"),
            CustomId = "mo_delete_this",
            Style = LocalButtonComponentStyle.Danger
        };

        [ButtonCommand("mo_delete_this")]
        public async ValueTask DeleteMessageButtonCommand()
        {
            IMessage msg = (Context.Interaction as IComponentInteraction)!.Message;

            await Bot.DeleteMessageAsync(msg.ChannelId, msg.Id);
        }
    }
}