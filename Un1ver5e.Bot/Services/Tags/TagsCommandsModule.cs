using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Disqord.Gateway;
using Disqord.Models;
using Disqord.Rest;
using Disqord.Serialization.Json.Default;
using Qmmands;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Commands;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Tags;
using static Disqord.Discord.Limits;

namespace Un1ver5e.Bot.Services
{
    public class TagsCommandsModule : DiscordApplicationGuildModuleBase
    {
        private readonly BotContext _dbctx;

        public TagsCommandsModule(BotContext dbctx)
        {
            _dbctx = dbctx;
        }

        //CREATE TAG
        [MessageCommand("Создать тег")]
        public async ValueTask CreateTagCommand(IMessage msg)
        {
            if (msg is not IUserMessage usermsg) throw new ArgumentException("Нельзя сделать тег из системного сообщения!");

            Tag tag = new(usermsg, Context.Author.Id, Context.GuildId);

            string interactionID = Guid.NewGuid().ToString();
            //Building Modal
            LocalInteractionModalResponse modal = new LocalInteractionModalResponse()
                .WithCustomId(interactionID)
                .WithTitle("Название тега")
                .WithComponents(
                    LocalComponent.Row(
                        new LocalTextInputComponent()
                            .WithCustomId("name")
                            .WithLabel("Имя")
                            .WithMaximumInputLength(Tag.MaxNameLength)
                            .WithStyle(TextInputComponentStyle.Short)
                            .WithPlaceholder(interactionID)),
                    LocalComponent.Row(
                        new LocalSelectionComponent()
                            .WithCustomId("publicity")
                            .WithIsDisabled(await Bot.IsOwnerAsync(Context.AuthorId) == false)
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

            await Context.Interaction.Response().SendModalAsync(modal);

            //Reading modal (some black magic)
            InteractionReceivedEventArgs modalEventArgs = await Bot.WaitForInteractionAsync(Context.ChannelId, e => e.Interaction is IModalSubmitInteraction ms && ms.CustomId == interactionID);
            if (modalEventArgs is null) return;
            IModalSubmitInteraction modalSubmit = (modalEventArgs.Interaction as IModalSubmitInteraction)!;
            IRowComponent row1 = (modalSubmit.Components[0] as IRowComponent)!;
            ITextInputComponent nameField = (row1.Components[0] as ITextInputComponent)!;

            IRowComponent row2 = (modalSubmit.Components[1] as IRowComponent)!;
            ISelectionComponent publicityField = (row2.Components[0] as ISelectionComponent)!;
            ITransientEntity<ComponentJsonModel> publicityEntity = (publicityField as ITransientEntity<ComponentJsonModel>)!;
            string publicity = publicityEntity.Model["values"].ToType<string[]>()[0]; //This is to be replaced once becomes available in disqord

            string name = nameField.Value;
            tag.Name = name;
            tag.IsPublic = bool.Parse(publicity);

            //"Save or Discard" menu
            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .WithContent(Context.Author.Mention)
                .AddEmbed(await tag.GetDisplayAsync(Bot))
                .AddComponent(new LocalRowComponent()
                    .AddComponent(new LocalButtonComponent()
                    { 
                        CustomId = interactionID,
                        Emoji = LocalEmoji.Unicode("💾")
                    })
                    .AddComponent(DeleteThisButtonCommandModule.GetDeleteButton()));

            await modalEventArgs.Interaction.Response().SendMessageAsync(response);

            InteractionReceivedEventArgs buttonEventArgs = await Bot.WaitForInteractionAsync(Context.ChannelId, e => e.Interaction is IComponentInteraction ms && ms.CustomId == interactionID);
            if (buttonEventArgs is null) return;

            IComponentInteraction buttonPress = (buttonEventArgs.Interaction as IComponentInteraction)!;
            IUserMessage buttonMsg = buttonPress.Message;

            await _dbctx.Tags.AddAsync(tag);
            await _dbctx.SaveChangesAsync();

            await buttonEventArgs.Interaction.Response()
                .ModifyMessageAsync(new LocalInteractionMessageResponse()
                .WithContent($"Успешно сохранил тег `{tag.Name}`!"));
        }


        //GET TAG
        [SlashCommand("tag")]
        [Description("Посылает тег")]
        public async ValueTask<IResult> SendTagCommand(
            [Name("Название"), Description("Название искомого тега")] string tagname)
        {
            await Deferral(false);

            ulong guildId = Context.GuildId;

            Tag? tag = _dbctx.Tags
                .Where(tag => tag.IsPublic || tag.GuildId == guildId)
                .Where(tag => tag.Name == tagname)
                .FirstOrDefault();

            if (tag is null) throw new ArgumentException("Тег не найден. Возможно, он принадлежит другому серверу и не является публичным. Публичные теги могут создавать только администраторы бота.", nameof(tagname));

            LocalInteractionMessageResponse response = new();
            tag.PasteTo(response);

            return Response(response);
        }


        //VIEW AND/OR DELETE TAG
        [SlashCommand("view-tag")]
        [Description("Информация о теге и удаление")]
        public async ValueTask ViewTagCommand(
            [Name("Название"), Description("Название искомого тега")] string tagname)
        {
            await Deferral(false);

            ulong guildId = Context.GuildId;

            Tag? tag = _dbctx.Tags
                .Where(tag => tag.IsPublic || tag.GuildId == guildId)
                .Where(tag => tag.Name == tagname)
                .FirstOrDefault();

            if (tag is null) throw new ArgumentException("Тег не найден. Возможно, он принадлежит другому серверу и не является публичным. Публичные теги могут создавать только администраторы бота.", nameof(tagname));

            bool isAuthor = tag.CanBeEditedBy(Context.AuthorId);
            string deleteTagComponentId = Guid.NewGuid().ToString();
            LocalEmbed embed = await tag.GetDisplayAsync(Bot);
            LocalRowComponent row = new()
            {
                Components =
                {
                    new LocalButtonComponent()
                    {
                        CustomId = deleteTagComponentId,
                        Emoji = LocalEmoji.Unicode("💀"),
                        Style = LocalButtonComponentStyle.Secondary,
                        IsDisabled = isAuthor == false && await Bot.IsOwnerAsync(Context.AuthorId) == false
                    },
                    DeleteThisButtonCommandModule.GetDeleteButton()
                }
            };

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(embed)
                .AddComponent(row);

            await Response(response);

            InteractionReceivedEventArgs e = await Bot.WaitForInteractionAsync(Context.ChannelId, e => e.Interaction is IComponentInteraction comp && comp.CustomId == deleteTagComponentId);
            if (e is null) return;

            //CONFIRMATION MODAL
            LocalInteractionModalResponse modal = new LocalInteractionModalResponse()
                .WithCustomId(deleteTagComponentId)
                .WithTitle("Введите название тега чтобы удалить его.")
                .WithComponents(
                    LocalComponent.Row(
                        new LocalTextInputComponent()
                            .WithCustomId("name")
                            .WithLabel("Название")
                            .WithMaximumInputLength(tag.Name.Length)
                            .WithStyle(TextInputComponentStyle.Short)
                            .WithPlaceholder(tag.Name)));

            await e.Interaction.Response().SendModalAsync(modal);

            //Reading modal
            InteractionReceivedEventArgs modalEventArgs = await Bot.WaitForInteractionAsync(Context.ChannelId, e => e.Interaction is IModalSubmitInteraction ms && ms.CustomId == deleteTagComponentId);
            if (modalEventArgs is null) return;
            IModalSubmitInteraction modalSubmit = (modalEventArgs.Interaction as IModalSubmitInteraction)!;
            IRowComponent row1 = (modalSubmit.Components[0] as IRowComponent)!;
            ITextInputComponent nameField = (row1.Components[0] as ITextInputComponent)!;

            bool isNameCorrect = nameField.Value == tag.Name;

            if (isNameCorrect)
            {
                _dbctx.Tags
                    .Remove(tag);
                await _dbctx.SaveChangesAsync();

                await modalEventArgs.Interaction.Response()
                .SendMessageAsync(new LocalInteractionMessageResponse()
                .WithContent($"Успешно удалил тег `{tag.Name}`!")
                .AddComponent(LocalComponent.Row(DeleteThisButtonCommandModule.GetDeleteButton())));
            }
            else
            {
                await modalEventArgs.Interaction.Response()
                .SendMessageAsync(new LocalInteractionMessageResponse()
                .WithContent($"Неверно введено название тега `{tag.Name}`!")
                .WithIsEphemeral(true));
            }
        }
        

        //GET TAG AUTOCOMPLETE
        [AutoComplete("tag")]
        [AutoComplete("view-tag")]
        public void SendTagAutocomplete(
            [Name("Название")] AutoComplete<string> tagname)
        {
            if (tagname.IsFocused)
            {
                string input = tagname.RawArgument;
                string regex = $".*{input}.*";

                string[] matches = _dbctx.Tags
                    .Where(tag => tag.IsPublic || tag.GuildId == Context.GuildId.RawValue)
                    .Select(tag => tag.Name)
                    .Where(name => Regex.IsMatch(name, regex))
                    .OrderBy(name => Guid.NewGuid())
                    .Take(ApplicationCommands.Options.MaxChoiceAmount) 
                    .ToArray();

                tagname.Choices.AddRange(matches);
            }
        }


        //LIST TAGS
        [SlashCommand("list-tags")]
        public async ValueTask<IResult> ListTagsCommand(
            [Name("Фильтр"), Description("Какие теги показываем")]
            [Choice("Свои", "self")]
            [Choice("Все", "all")]  string filter,
            [Name("Regex"), Description("Регекс для поиска")] string regex = ".*")
        {
            await Deferral(false);

            ulong authorId = Context.AuthorId.RawValue;
            ulong guildId = Context.GuildId.RawValue;
            IEnumerable<string> foundtags = filter switch
            {
                "self" => _dbctx.Tags
                    .Where(tag => tag.AuthorId == authorId)
                    .Where(tag => Regex.IsMatch(tag.Name, regex))
                    .Select(tag => tag.Name)
                    .OrderBy(name => name),
                "all" => _dbctx.Tags
                    .Where(tag => tag.GuildId == guildId || tag.IsPublic)
                    .Where(tag => Regex.IsMatch(tag.Name, regex))
                    .Select(tag => tag.Name)
                    .OrderBy(name => name),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "what the fuck")
            };

            const int maxPageLen = LocalEmbed.MaxDescriptionLength - 6; //6 is for codeblock frame
            const int maxTagNameLen = Tag.MaxNameLength + 1; //1 is for newline symbol

            const int maxPageNamesCount = maxPageLen / maxTagNameLen;

            string[][] rawPages = foundtags
                .Chunk(maxPageNamesCount)
                .ToArray();
            IEnumerable<Page> pages = rawPages
                .Select((text, index) => new Page()
                {
                    Embeds =
                    {
                        new LocalEmbed()
                        {
                            Title = $"Найденные теги:\n**{index + 1}/{rawPages.Length}**",
                            Description = string.Join('\n', text).AsCodeBlock()
                        }
                    }
                });

            return Pages(pages);
        }
    }
}
