using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Webhook;
using Microsoft.Extensions.Logging;
using Qmmands;
using Un1ver5e.Bot.Models;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Commands
{
    public class WebhookCommandsModule : DiscordApplicationModuleBase
    {
        private readonly BotContext _dbctx;
        private readonly IWebhookClientFactory _factory;

        public WebhookCommandsModule(BotContext dbctx, IWebhookClientFactory factory)
        {
            _dbctx = dbctx;
            _factory = factory;
        }

        //BROADCAST
        [MessageCommand("Объявить")]
        [RequireBotOwner]
        public async ValueTask<IResult> BroadCastCommand(IMessage message)
        {
            await Deferral(true);

            LocalWebhookMessage msg = new LocalWebhookMessage().WithContent(message.Content);

            await foreach (Webhook webhook in _dbctx.Webhooks.AsAsyncEnumerable())
            {
                try
                {
                    IWebhookClient client = _factory.CreateClient(webhook.Url);

                    await client.ExecuteAsync(msg);
                }
                catch (Exception)
                {
                    Logger.LogWarning("An exception occured while broadcasting to webhook {webhook}", $"...{webhook.Url[^16]}");
                }
            }

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                    .WithIsEphemeral(true)
                    .WithContent("Успешно 📢");

            return Response(resp);
        }

        //ADDWEBHOOK
        [SlashCommand("add-webhook")]
        [Description("Добавляет боту вебхук. Не используй если не понимаешь зачем.")]
        [RequireAuthorPermissions(Permissions.ManageWebhooks)]
        public async ValueTask<IResult> AddWebhookCommand(
            [Name("Вебхук"), Description("URL вебхука")] string webhookurl)
        {
            await Deferral(true);

            LocalWebhookMessage msg = new LocalWebhookMessage()
                .AddEmbed(new LocalEmbed()
                    .WithAuthor(Bot.CurrentUser!)
                    .WithTitle("Получил этот вебхук! 💾"));

            await _dbctx.Webhooks.AddAsync(new Webhook() { Url = webhookurl });
            await _dbctx.SaveChangesAsync();

            IWebhookClient client = _factory.CreateClient(webhookurl);
            await client.ExecuteAsync(msg);

            return Response(new LocalEmbed().WithTitle("Успешно!"));
        }
    }
}
