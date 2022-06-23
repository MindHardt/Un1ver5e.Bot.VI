using Disqord;
using Disqord.Webhook;
using Microsoft.Extensions.Configuration;

namespace Un1ver5e.Bot.Services
{
    public class WebHookFeed
    {
        private readonly IWebhookClient client;

        public WebHookFeed(IConfiguration config, IWebhookClientFactory factory)
        {
            client = factory.CreateClient(config["webhook_url"]);
        }

        public async ValueTask SendMessage(LocalWebhookMessage msg)
        {
            await client.ExecuteAsync(msg);
        }
    }
}
