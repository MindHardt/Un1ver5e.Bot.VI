using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Webhook;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Discord;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Services.Database;
using Un1ver5e.Bot.Services.Dice;
using Un1ver5e.Bot.Services.RateOptionsProvider;

namespace Un1ver5e.Bot
{
    public static class BotHostBuilder
    {
        public static IHost CreateHost(string[] args)
        {
            // All host configuration goes here.
            return new HostBuilder()
                .UseDefaultServiceProvider(config =>
                {
                    config.ValidateOnBuild = true;
                    config.ValidateScopes = true;
                })
                .UseSerilog((ctx, services, logger) =>
                {
                    string filePath = $"Logs/Log-.log";

                    logger
                    .MinimumLevel.ControlledBy(services.GetRequiredService<LoggingLevelSwitch>())
                    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                    .WriteTo.File(filePath, rollingInterval: RollingInterval.Day, shared: true);

                    string? webhook = ctx.Configuration.GetRequiredSection("discord_config").GetSection("logs_webhook_url").Get<string>();
                    if (webhook is not null)
                    {
                        (ulong Id, string Token) webhookData = ParseWebhookUrl(webhook);
                        logger.WriteTo.Discord(webhookData.Id, webhookData.Token, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);
                    }
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Environment.CurrentDirectory);
                    config.AddJsonFile("config.json");
                    config.AddCommandLine(args);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services
                    .AddSingleton<Random>()
                    .AddSingleton<LoggingLevelSwitch>()

                    .AddTransient<IRateOptionsProvider, BotContextRateOptionsProvider>()

                    .AddSingleton<IDiceService, DefaultDiceService>()

                    .AddWebhookClientFactory()

                    .AddDbContext<BotContext>(options =>
                    {
                        string[]? connstrArgs = ctx.Configuration.GetSection("db_connstr_args").Get<string[]>();
                        ArgumentNullException.ThrowIfNull(connstrArgs);

                        string connstr = string.Join(';', connstrArgs);

                        options.UseNpgsql(connstr);
                    });

                })
                .ConfigureDiscordBot<Un1ver5eBot>((context, bot) =>
                {
                    IConfigurationSection config = context.Configuration.GetRequiredSection("discord_config");

                    string? token = config["token"]; ;
                    string[] prefixes = config.GetSection("prefixes").Get<string[]>() ?? Array.Empty<string>();

                    bot.Token = token;
                    bot.Prefixes = prefixes;
                    bot.Intents |= GatewayIntents.DirectMessages | GatewayIntents.DirectReactions;
                })
                .Build();
        }

        private static (ulong id, string token) ParseWebhookUrl(string url)
        {
            Match match = Regex.Match(url, "https://discord.com/api/webhooks/(?<Id>\\d*)/(?<Token>.*)");

            ulong id = ulong.Parse(match.Groups["Id"].Value);
            string token = match.Groups["Token"].Value;

            return (id, token);
        }
    }
}
