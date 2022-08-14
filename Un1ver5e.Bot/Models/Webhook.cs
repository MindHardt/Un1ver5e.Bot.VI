namespace Un1ver5e.Bot.Models
{
    public record Webhook
    {
        public int Id { get; set; }
        public string Url { get; init; } = null!;
    }
}
