using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Un1ver5e.Bot.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public record Tag
    {
        /// <summary>
        /// Maximum <see cref="Name"/> length. Exceeding this will result in <see cref="ArgumentException"/>.
        /// </summary>
        public const int MaxNameLength = 48;
        private string name = null!;
        private string content = null!;

        /// <summary>
        /// Id for <see cref="Microsoft.EntityFrameworkCore"/>
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Name of this tag. Its length must be below <see cref="MaxNameLength"/> and will be saved in lowercase.
        /// </summary>
        [DisallowNull]
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("Имя не может быть пустым!", nameof(value));
                if (value.Length > MaxNameLength) throw new ArgumentException($"Имя тега не должно превышать {MaxNameLength} символов!", nameof(value));
                name = value.ToLower();
            }
        }

        /// <summary>
        /// The actual message content of this tag. Must not be empty or null.
        /// </summary>
        [DisallowNull]
        public string Content
        {
            get => content;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("Нельзя создать тег из сообщения без текста! Все прикрепленные файлы следует вставлять в виде ссылок.", nameof(value));
                content = value;
            }
        }

        /// <summary>
        /// Defines whether this tag can be used in every guild bot is in or only in the guild where it was created.
        /// </summary>
        public bool IsPublic { get; set; } = false;

        /// <summary>
        /// The tag author's discord Id.
        /// </summary>
        public ulong AuthorId { get; set; }

        /// <summary>
        /// The discord Id of the guild in which this tag was created. For non-public tags, tag usage is restricted to this guild only.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The exact time when tag was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Formats this <see cref="Tag"/> as a <typeparamref name="TMessage"/> and returns a new object of this type.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public TMessage CreateMessage<TMessage>()
            where TMessage : LocalMessageBase, new()
            => new TMessage()
            .WithContent(Content)
            .AddComponent(LocalComponent.Row(new LocalButtonComponent()
            {
                CustomId = Guid.NewGuid().ToString(),
                IsDisabled = true,
                Label = $"Тег [{Name}]",
                Style = LocalButtonComponentStyle.Secondary
            }));

        /// <summary>
        /// Defines whether this <see cref="Tag"/> can be edited by the specified user.
        /// </summary>
        /// <param name="isOwner">External check for bot's owner</param>
        /// <param name="authorId"></param>
        /// <returns></returns>
        public bool CanBeEditedBy(bool isOwner, ulong authorId) => isOwner || AuthorId == authorId;

        /// <summary>
        /// Creates a tag with specified data whos <see cref="Name"/> is randomized via <see cref="Guid.NewGuid()"/>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="authorId"></param>
        /// <param name="guildId"></param>
        public Tag(string content, ulong authorId, ulong guildId)
        {
            Name = Guid.NewGuid().ToString();
            Content = content;
            AuthorId = authorId;
            GuildId = guildId;
            CreatedAt = DateTime.UtcNow;
            IsPublic = false;
        }

        public Tag() { } //ctor for EntityFramework

    }
}
