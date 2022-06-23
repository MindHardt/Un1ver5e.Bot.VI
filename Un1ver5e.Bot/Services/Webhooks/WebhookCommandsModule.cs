using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Webhook;
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
        [SlashCommand("broadcast")]
        [Description("Новости.")]
        [RequireBotOwner]
        public async ValueTask<IResult> BroadCastCommand(
            [Name("Текст"), Description("Текст сообщения.")] string text)
        {
            await Deferral(true);

            LocalWebhookMessage msg = new LocalWebhookMessage().WithContent(text);

            foreach (Webhook webhook in _dbctx.Webhooks)
            {
                IWebhookClient client = _factory.CreateClient(webhook.Url);

                await client.ExecuteAsync(msg);
            }

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                    .WithIsEphemeral(true)
                    .WithContent("Успешно 📢");

            return Response(resp);
        }

        //ADDWEBHOOK
        [SlashCommand("add-webhook")]
        [Description("Добавляет боту вебхук. Не используй если не понимаешь зачем.")]
        [RequireAuthorPermissions(Permission.ManageChannels)]
        public async ValueTask<IResult> AddWebhookCommand(
            [Name("Вебхук"), Description("URL вебхука")] string webhookurl)
        {
            await Deferral(true);

            LocalWebhookMessage msg = new LocalWebhookMessage()
                .AddEmbed(new LocalEmbed()
                    .WithAuthor(Bot.CurrentUser)
                    .WithTitle("Получил этот вебхук! 💾"));

            await _dbctx.Webhooks.AddAsync(new Webhook() { Url = webhookurl });
            await _dbctx.SaveChangesAsync();

            IWebhookClient client = _factory.CreateClient(webhookurl);
            await client.ExecuteAsync(msg);

            return Response(new LocalEmbed().WithTitle("Успешно!"));
        }
    }
}
