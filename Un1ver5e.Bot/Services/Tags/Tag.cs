using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Microsoft.EntityFrameworkCore;

namespace Un1ver5e.Bot.Services.Tags
{
    [Index(nameof(Name), IsUnique = true)]
    public record Tag
    {
        public const int MaxNameLength = 48;
        private static readonly string[] s_reservedNames =
        {
            "all", "latest"
        };
        private string _name = null!;

        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                string formatted = value.ToLower();
                if (CheckName(formatted) == false) throw new ArgumentException("Name is prohibited!", nameof(value));
                _name = formatted;
            }
        }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
        public string Content { get; set; }
        public ulong AuthorId { get; set; }
        public ulong GuildId { get; set; } //Tags can only be created in guilds; only owner(s) can make global tags

        public LocalMessageBase PasteTo(LocalMessageBase msg) => msg
            .WithContent(Content)
            .AddComponent(LocalComponent.Row(new LocalButtonComponent()
            {
                CustomId = Guid.NewGuid().ToString(),
                IsDisabled = true,
                Label = $"Тег [{Name}]",
                Style = LocalButtonComponentStyle.Secondary
            }));

        public async ValueTask<LocalEmbed> GetDisplayAsync(DiscordBotBase bot)
        {
            IUser? author = bot.GetUser(AuthorId);
            if (author is null) author = await bot.FetchUserAsync(AuthorId);

            return new LocalEmbed()
            {
                Author = new LocalEmbedAuthor()
                {
                    IconUrl = author!.GetAvatarUrl(),
                    Name = author!.Name
                },
                Description = Content,
                Title = $"> Тег `{Name}`",
                Timestamp = new DateTimeOffset(CreatedAt),
                Fields = new LocalEmbedField[]
                {
                    new LocalEmbedField()
                    {
                        Name = "Публичный",
                        Value = IsPublic ? "🌝 Да" : "🌚 Нет"
                    }
                }
            };
        }

        public bool CanBeSeen(ulong guildId) => IsPublic || GuildId == guildId;

        public bool CanBeEditedBy(ulong userId) => AuthorId == userId;

        public Tag(IUserMessage message, ulong authorId, ulong guildId)
        {
            Name = Guid.NewGuid().ToString();
            Content = message.Content;
            AuthorId = authorId;
            GuildId = guildId;
            CreatedAt = DateTime.UtcNow;
            IsPublic = false;
        }

#pragma warning disable CS8618 //possible-null warnings
        public Tag() { } //ctor for EntityFramework
#pragma warning restore

        private static bool CheckName(string name) =>
            name.Length <= MaxNameLength ||
            s_reservedNames.Contains(name.ToLower()) == false;

    }
}
