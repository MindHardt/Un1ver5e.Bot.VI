using Disqord;
using Disqord.Bot.Commands.Components;
using Disqord.Rest;
using Qmmands;

namespace Un1ver5e.Bot.Commands.DeleteThisButton
{
    public class DeleteThisButtonComponentCommand : DiscordComponentModuleBase
    {
        //This fires when a button restricted to author is pressed
        [ButtonCommand(DeleteThisButtonCustomId.Value + ":*")]
        public async ValueTask<IResult> DeleteMessageButtonCommand(Snowflake restrictedToId)
        {
            if (Context.AuthorId == restrictedToId)
            {
                IMessage msg = (Context.Interaction as IComponentInteraction)!.Message;

                await Bot.DeleteMessageAsync(msg.ChannelId, msg.Id);
                return Results.Success;
            }
            return Results.Failure("Кнопки самоуничтожения синего цвета доступны только тому, кто вызвал сообщение, красные доступны всем.");
        }

        //This fires when a public button is pressed
        [ButtonCommand(DeleteThisButtonCustomId.Value)]
        public async ValueTask DeleteMessageButtonCommand()
        {
            IMessage msg = (Context.Interaction as IComponentInteraction)!.Message;

            await Bot.DeleteMessageAsync(msg.ChannelId, msg.Id);
        }
    }
}
