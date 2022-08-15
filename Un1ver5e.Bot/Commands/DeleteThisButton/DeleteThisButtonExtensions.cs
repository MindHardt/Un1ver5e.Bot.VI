using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Commands.DeleteThisButton
{
    public static class DeleteThisButtonExtensions
    {
        /// <summary>
        /// Adds a "delete this" button to this message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="msg"></param>
        /// <param name="restrictedToId"></param>
        /// <returns></returns>
        public static TMessage AddDeleteThisButton<TMessage>(this TMessage msg, ulong? restrictedToId = null)
            where TMessage : LocalMessageBase
            => msg.AddComponent(GetDeleteButtonRow(restrictedToId));

        /// <summary>
        /// Gets a button which will delete its message upon press.
        /// </summary>
        /// <returns></returns>
        public static LocalButtonComponent GetDeleteButton(ulong? restrictedToId = null) => new()
        {
            Emoji = LocalEmoji.Unicode("🗑"),
            CustomId = restrictedToId is null ? DeleteThisButtonCustomId.Value : $"{DeleteThisButtonCustomId.Value}:{restrictedToId}",
            Style = restrictedToId is null ? LocalButtonComponentStyle.Danger : LocalButtonComponentStyle.Primary
        };

        /// <summary>
        /// Gets a button which will delete its message upon press.
        /// </summary>
        /// <returns>A <see cref="LocalRowComponent"/> which only contains delete button.</returns>
        public static LocalRowComponent GetDeleteButtonRow(ulong? restrictedToId = null) => LocalComponent.Row(GetDeleteButton(restrictedToId));
    }
}
