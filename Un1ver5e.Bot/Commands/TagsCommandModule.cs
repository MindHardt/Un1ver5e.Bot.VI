using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity;
using Disqord.Rest;
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
#pragma warning disable CS0162
            if (false) return Results.Failure("Превышено максимальное количество тегов!"); //TODO: Add tag limit
#pragma warning restore CS0162

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
            Tag? existentTag = _dbctx.Tags.FirstOrDefault(t => t.Name == tagName);
            if (existentTag is not null) 
            {
                if (existentTag.CanBeEditedBy(await Bot.IsOwnerAsync(Context.AuthorId), Context.AuthorId))
                {
                    string modalId = Guid.NewGuid().ToString();
                    var confirmationModal = new LocalInteractionModalResponse()
                        .WithCustomId(modalId)
                        .WithTitle("Чтобы перезаписать тег введите его название.")
                        .WithComponents(
                            LocalComponent.Row(
                                LocalComponent.TextInput("inputName", "Название тега", TextInputComponentStyle.Short)
                                .WithPlaceholder(tagName)));

                    await Context.Interaction.Response().SendModalAsync(confirmationModal);

                    var e = await Context.WaitForInteractionAsync<IModalSubmitInteraction>(m => m.CustomId == modalId);
                    if (e is null || e.Components[0] is not ITextInputComponent field || field.Value != tagName) return Results.Failure("Вы не ответили на модал!");

                    _dbctx.Tags.Update(tag);
                    await _dbctx.SaveChangesAsync();
                    await e.Response().SendMessageAsync(
                        new LocalInteractionMessageResponse()
                        .AddEmbed(
                            new LocalEmbed().WithDescription($"Успешно сохранил тег `{tagName}`")));

                    return Results.Success;
                }
                else return Results.Failure("Тег с таким названием уже существует и у вас нет прав на его перезапись!");
            }
            else
            {
                await _dbctx.Tags.AddAsync(tag);
                await _dbctx.SaveChangesAsync();

                return Response(new LocalEmbed().WithDescription($"Успешно сохранил тег `{tagName}`"));
            }
        }

        [SlashCommand("тег"), Description("Отправляет тег.")]
        public IResult SendTagCommand(
            [Name("Название-тега"), Description("Название тега. Подождите немного для предоставления вариантов.")] 
            string tagName)
        {
            Tag? tag = _dbctx.Tags.FirstOrDefault(tag => tag.Name == tagName);
            if (tag is null) return Results.Failure($"Тег {tagName} не найден!");

            var response = tag.CreateMessage<LocalInteractionMessageResponse>();

            return Response(response);
        }

        [AutoComplete("тег")]
        public void SendTagAutocomplete(
            [Name("Название-тега")] AutoComplete<string> tagName)
        {
            if (tagName.IsFocused)
            {
                var matches = _dbctx.Tags
                    .ThatAreSeenIn(Context.GuildId)
                    .Select(tag => tag.Name)
                    .Where(name => Regex.IsMatch(name, tagName.RawArgument!))
                    .Take(Disqord.Discord.Limits.ApplicationCommand.MaxOptionAmount);

                tagName.Choices.AddRange(matches);
            }
        }
    }
}
