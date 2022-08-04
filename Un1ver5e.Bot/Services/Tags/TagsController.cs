using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Bot.Commands.Components;
using Disqord.Bot.Commands.Text;
using Disqord.Extensions.Interactivity;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;
using Qmmands.Text;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Services.Tags
{
    public static class TagsController
    {
        public static async ValueTask InitiateTagCreation(IInteraction interaction, Snowflake messageId, bool publicityDisabled = true)
        {
            string modalId = $"create_tag:{messageId.RawValue}";

            LocalInteractionModalResponse modal = new LocalInteractionModalResponse()
                .WithCustomId(modalId)
                .WithTitle("Создание тега")
                .WithComponents(
                    LocalComponent.Row(
                        new LocalTextInputComponent()
                            .WithCustomId("name")
                            .WithLabel("Имя")
                            .WithMaximumInputLength(Tag.MaxNameLength)
                            .WithStyle(TextInputComponentStyle.Short)
                            .WithPlaceholder(Guid.NewGuid().ToString())),
                    LocalComponent.Row(
                        new LocalSelectionComponent()
                            .WithCustomId("publicity")
                            .WithIsDisabled(publicityDisabled)
                            .WithPlaceholder("Серверный")
                            .WithOptions(
                                new LocalSelectionComponentOption()
                                    .WithLabel("Серверный")
                                    .WithValue("false")
                                    .WithIsDefault(),
                                new LocalSelectionComponentOption()
                                    .WithLabel("Публичный")
                                    .WithValue("true")))
                        );

            await interaction.Response().SendModalAsync(modal);


        }

        
        public class ApplicationCommands : DiscordApplicationGuildModuleBase
        {
            [MessageCommand("Создать тег")]
            public async ValueTask CreateTag(IMessage msg)
            {
                if (msg is not IUserMessage userMsg) throw new ArgumentException("Нельзя создать тег из системного сообщения!");

                await InitiateTagCreation(Context.Interaction, userMsg.Id, await Bot.IsOwnerAsync(Context.AuthorId));
            }
        }
        public class TextCommands : DiscordTextGuildModuleBase
        {
            [TextCommand("create_tag")]
            public async ValueTask CreateTag()
            {
                if (Context.Message.ReferencedMessage.HasValue == false) 
                    throw new ArgumentException("Эта команда применяется в реплаях.");

                string messageLink = Discord.MessageJumpLink(Context.GuildId, Context.ChannelId, Context.Message.Id);
                string buttonId = Guid.NewGuid().ToString();

                LocalButtonComponent button = new()
                {
                    CustomId = buttonId,
                    Label = "Тык"
                };

                LocalMessage msg = new()
                {
                    Content = $"Чтобы создать тег из [этого сообщения]({messageLink}) нажмите на кнопку.",
                    Components = new LocalRowComponent[]
                    {
                        LocalComponent.Row(button)
                    }
                };
                await Response(msg);
            }
        }
        public class ComponentCommands : DiscordComponentGuildModuleBase
        {
            [ButtonCommand("create_tag_*")]
            public async ValueTask RespondToTextCreateTag(ulong messageId) => await InitiateTagCreation(Context.Interaction, messageId, await Bot.IsOwnerAsync(Context.AuthorId));

        }
    }

}
