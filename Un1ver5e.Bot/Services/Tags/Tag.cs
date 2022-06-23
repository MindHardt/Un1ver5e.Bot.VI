using Disqord;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Json;

namespace Un1ver5e.Bot.Services.Tags
{
    public record Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual string Content { get; set; }
        public virtual string Attachments { get; set; }
        public virtual ulong AuthorId { get; set; }

        public virtual LocalMessage Format()
        {
            using (HttpClient client = new())
            {
                return new LocalMessage
                {
                    Content = Content,
                    Attachments = System.Text.Json.JsonSerializer.Deserialize<string[]>(Attachments)!.Select(async a => new LocalAttachment(await client.GetStreamAsync(a), a)).Select(t => t.Result).ToList() as IList<LocalAttachment>,
                };
            }
        }

        public Tag(IUserMessage message, IUser author, string? name = null)
        {
            Name = name ?? Guid.NewGuid().ToString();
            Content = message.Content;
            Attachments = System.Text.Json.JsonSerializer.Serialize(message.Attachments.Select(a => a.Url).ToArray());
            AuthorId = author.Id.RawValue;
        }

        public Tag() { }
    }
}
