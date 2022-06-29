using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity.Menus.Paged;
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
        [MessageCommand("Create Tag")]
        [RequireGuild]

        public async ValueTask<IResult> CreateTagCommand(IMessage msg)
        {
            if (msg is not IUserMessage usermsg) throw new ArgumentException("Нельзя сделать тег из системного сообщения!");

            await Deferral(true);

            Tag tag = new(usermsg, Context.Author.Id, Context.GuildId!.Value);

            await _dbctx.Tags.AddAsync(tag);
            await _dbctx.SaveChangesAsync();

            return Response(tag.Name);
        }

        [SlashCommand("tag")]
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

            return Response(tag.Format());
        }


        [SlashCommand("renametag")]
        public async ValueTask<IResult> RenameTagCommand(
            [Name("Название"), Description("Название искомого тега")] string oldname,
            [Name("Новое"), Description("Новое название тега")] string newname)
        {
            await Deferral(false);

            Tag? tag = _dbctx.Tags.Where(tag => tag.Name == oldname).FirstOrDefault();

            if (tag is null) throw new ArgumentException($"Не найден тег {oldname}");
            if (tag.AuthorId != Context.AuthorId && await Bot.IsOwnerAsync(Context.AuthorId) == false) throw new Exception("У вас нет доступа к этому тегу!");

            tag.Name = newname;

            _dbctx.Tags.Update(tag);
            await _dbctx.SaveChangesAsync();

            LocalEmbed embed = new()
            {
                Fields =
                {
                    new()
                    {
                        Name = "Старое имя",
                        Value = oldname.AsCodeBlock(),
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Новое имя",
                        Value = newname.AsCodeBlock(),
                        IsInline = true
                    }
                },
                Title = "Название тега изменено",
                Timestamp = DateTime.Now,
            };

            return Response(embed);
        }


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
