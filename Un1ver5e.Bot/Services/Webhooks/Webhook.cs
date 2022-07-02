namespace Un1ver5e.Bot.Services.Webhooks
{
    public record Webhook
    {
        public int Id { get; set; }
        public string Url { get; init; } = null!;
    }
}
