using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Webhook;
using Microsoft.Extensions.Logging;
using Qmmands;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Webhooks;

namespace Un1ver5e.Bot.Services
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

            foreach (Webhook webhook in _dbctx.Webhooks)
            {
                try
                {
                    IWebhookClient client = _factory.CreateClient(webhook.Url);

                    await client.ExecuteAsync(msg);
                }
                catch (Exception)
                {
                    Logger.LogWarning("An exception occured while broadcasting to webhook", webhook.Url);
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
