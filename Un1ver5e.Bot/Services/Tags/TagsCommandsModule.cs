using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Bot.Commands.Components;
using Disqord.Extensions.Interactivity;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Disqord.Gateway;
using Disqord.Rest;
using Disqord.Webhook;
using Qmmands;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Tags;
using Un1ver5e.Bot.Services.Webhooks;

namespace Un1ver5e.Bot.Services
{
    public class TagsCommandsModule : DiscordApplicationModuleBase
    {
        private readonly BotContext _dbctx;

        public TagsCommandsModule(BotContext dbctx)
        {
            _dbctx = dbctx;
        }

        //Create
        [MessageCommand("Создать тег")]
        [RequireGuild]
        public async ValueTask<IResult> CreateTagCommand(IMessage msg)
        {
            if (msg is not IUserMessage usermsg) throw new ArgumentException("Нельзя сделать тег из системного сообщения!");

            Tag tag = new(usermsg, Context.Author.Id, Context.GuildId!.Value);

            string modalID = tag.Name;
            var modal = new LocalInteractionModalResponse()
                .WithCustomId(modalID)
                .WithTitle("Название тега")
                .WithComponents(
                    LocalComponent.Row(LocalComponent.TextInput("name", "Имя", TextInputComponentStyle.Short).WithPlaceholder(modalID)));

            await Context.Interaction.Response().SendModalAsync(modal);

            InteractionReceivedEventArgs? e = await Bot.WaitForInteractionAsync(Context.ChannelId, e => e is IModalSubmitInteraction ms && ms.CustomId == modalID);
            IModalSubmitInteraction modalSubmit = (e as IModalSubmitInteraction)!;
            ITextInputComponent field = ((modalSubmit.Components[0] as IRowComponent)!.Components[0] as ITextInputComponent)!;

            string name = field.Value;
            tag.Name = name;

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse().AddEmbed(tag.GetDisplay(Bot)).WithIsEphemeral(true);

            await e.Interaction.Response().SendMessageAsync(response);

            //await _dbctx.Tags.AddAsync(tag);
            //await _dbctx.SaveChangesAsync();

            return default!;
        }



        [SlashCommand("tag")]
        [RequireGuild]
        public async ValueTask<IResult> SendTagCommand(
            [Name("Название"), Description("Название искомого тега")] string tagname)
        {
            await Deferral(false);

            ulong guildId = Context.GuildId!.Value;

            Tag? tag = _dbctx.Tags
                .Where(tag => tag.IsPublic == true || tag.GuildId == guildId)
                .Where(tag => tag.Name == tagname)
                .FirstOrDefault();

            if (tag is null) throw new ArgumentException("Тег не найден. Возможно, он принадлежит другому серверу и не является публичным. Публичными теги могут делать только администраторы бота.", nameof(tagname));

            LocalInteractionMessageResponse response = new();
            tag.PasteTo(response);

            return Response(response);
        }


        //[SlashCommand("modify-tag")]
        //public async ValueTask<IResult> ModifyTagCommand(
        //    [Name("Название"), Description("Название искомого тега")] string name)
        //{
        //    await Deferral(false);

        //    Tag? tag = _dbctx.Tags.Where(tag => tag.Name == name).FirstOrDefault();

        //    if (tag is null) throw new ArgumentException($"Не найден тег {name}");
        //    if (tag.AuthorId != Context.AuthorId && await Bot.IsOwnerAsync(Context.AuthorId) == false) throw new Exception("У вас нет доступа к этому тегу!");

        //    return View(new TagModifyView(Bot, tag));
        //}


        [SlashCommand("list-tags")]
        public async ValueTask<IResult> ListTagsCommand(
            [Name("Фильтр"), Description("Какие теги показываем")]
            [Choice("Свои", "self")]
            [Choice("Все", "all")]  string filter,
            [Name("Regex"), Description("Регекс для поиска")] string regex = ".*")
        {
            await Deferral(false);

            ulong authorId = Context.AuthorId.RawValue;
            IEnumerable<string> foundtags = filter switch
            {
                "self" => _dbctx.Tags
                    .Where(tag => tag.AuthorId == authorId)
                    .Where(tag => Regex.IsMatch(tag.Name, regex))
                    .Select(tag => tag.Name)
                    .OrderBy(name => name),
                "all" => _dbctx.Tags
                    .Where(tag => tag.GuildId == Context.GuildId || tag.IsPublic)
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
