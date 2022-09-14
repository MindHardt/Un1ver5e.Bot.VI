using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity;
using Disqord.Rest;
using Microsoft.EntityFrameworkCore;
using Qmmands;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Models;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Commands.Tags
{
    public class TagsCommandModule : DiscordApplicationGuildModuleBase
    {
        private readonly IStashStorage<Snowflake> _storage;
        private readonly BotContext _dbctx;

        public TagsCommandModule(IStashStorage<Snowflake> storage, BotContext dbctx)
        {
            _storage = storage;
            _dbctx = dbctx;
        }

        [SlashCommand("создать-тег")]
        [Description("Создает тег из вашей сохраненки (команда \"Запомнить\")")]
        public async ValueTask<IResult> CreateTagCommand(
            [Name("Название-тега"), Description("Название тега. Будет сохранено в нижнем регистре"), Maximum(Tag.MaxNameLength)]
            string tagName,
            [Name("Публичность"), Description("Можно ли использовать этот тег везде где есть бот? Публичные теги создают только админы.")]
            [Choice("Серверный", "Серверный")]
            [Choice("Публичный(только для админов)", "Публичный")]
            string publicity)
        {
            await Deferral(false); //This might take a while

            //Checking tag limit
            int tagLimit = (await _dbctx.Users.FirstOrDefaultAsync(u => u.Id == Context.AuthorId.RawValue) ?? new UserData()).TagsCountLimit;
            int tagCount = await _dbctx.Tags.Where(t => t.AuthorId == Context.AuthorId.RawValue).CountAsync();

            if (tagCount >= tagLimit) return Results.Failure("Превышено максимальное количество тегов! Если считаете это несправедливым, свяжитесь с владельцами бота.");

            //Getting stashed message
            IStashData? data = _storage.Get(Context.AuthorId);
            if (data is null) return Results.Failure("У вас нет сохраненки! Используйте текстовую команду \"запомни\" или контекстную \"Запомнить\".");

            //Parsing publicity
            bool? isPublic = publicity switch
            {
                "Публичный" => true,
                "Серверный" => false,
                _ => null
            };
            if (isPublic is null) return Results.Failure("Неверное значение поля \"Публичность\".");
            if (isPublic.Value && await Bot.IsOwnerAsync(Context.AuthorId) == false) return Results.Failure("У вас нет прав на создание публичного тега.");

            //Creating tag object
            Tag tag = new(data.Message.Content, Context.AuthorId, Context.GuildId)
            {
                IsPublic = isPublic.Value,
                Name = tagName
            };

            //Checking if tag with this name already exists in a database
            Tag? existentTag = _dbctx.Tags.FirstOrDefault(t => t.Name == tagName); //TODO: Tag delete && tag rewrite confirmation
            if (existentTag is null || existentTag.CanBeEditedBy(await Bot.IsOwnerAsync(Context.AuthorId), Context.AuthorId))
            {
                if (existentTag is not null) _dbctx.Tags.Remove(existentTag);
                _dbctx.Tags.Add(tag);
                await _dbctx.SaveChangesAsync();

                string action = existentTag is null ? "сохранил" : "перезаписал";
                return Response(new LocalEmbed().WithDescription($"Успешно {action} тег `{tagName}`"));
            }
            else return Results.Failure("Тег с таким названием уже существует и у вас нет прав на его перезапись!");
        }

        [SlashCommand("тег"), Description("Отправляет тег.")]
        public async ValueTask<IResult> SendTagCommandAsync(
            [Name("Название-тега"), Description("Название тега. Подождите немного для предоставления вариантов.")] 
            string tagName)
        {
            Tag? tag = await _dbctx.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);
            if (tag is null) return Results.Failure($"Тег {tagName} не найден!");

            var response = tag.CreateMessage<LocalInteractionMessageResponse>();

            return Response(response);
        }

        [AutoComplete("тег")]
        public async ValueTask SendTagAutocompleteAsync(
            [Name("Название-тега")] AutoComplete<string> tagName)
        {
            if (tagName.IsFocused)
            {
                var matches = await _dbctx.Tags
                    .ThatAreSeenIn(Context.GuildId)
                    .AsQueryable()
                    .OrderBy(tag => EF.Functions.Random())
                    .Select(tag => tag.Name)
                    .Where(name => Regex.IsMatch(name, tagName.RawArgument!))
                    .Take(Disqord.Discord.Limits.ApplicationCommand.Option.MaxChoiceAmount)
                    .ToArrayAsync();

                tagName.Choices.AddRange(matches);
            }
        }
    }
}
