using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Un1ver5e.Bot.Services;
using Un1ver5e.Bot.Services.Dice;

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
                .UseSerilog((_, services, logger) =>
                {
                    string filePath = $"Logs/Log-.log";

                    logger
                    .MinimumLevel.ControlledBy(services.GetRequiredService<LoggingLevelSwitch>())
                    .WriteTo.Console()
                    .WriteTo.File(filePath, rollingInterval: RollingInterval.Day, shared: true);
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Environment.CurrentDirectory);
                    config.AddJsonFile("config.json");
                    config.AddCommandLine(args);
                })
                .ConfigureServices((_, services) =>
                {
                    services
                    .AddSingleton<Random>()
                    .AddSingleton<LoggingLevelSwitch>()

                    .AddSingleton<IDiceService, DefaultDiceService>()

                    .AddWebhookClientFactory()
                    .AddSingleton<WebHookFeed>();
                })
                .ConfigureDiscordBot((context, bot) =>
                {
                    IConfigurationSection config = context.Configuration.GetSection("discord_config");

                    string splash = config.GetSection("splashes").Get<string[]>().GetRandomElement();
                    string token = config["token"];
                    string[] prefixes = config.GetSection("prefixes").Get<string[]>();

                    bot.Activities = new LocalActivity[] { new(splash, Disqord.ActivityType.Watching) };
                    bot.Token = token;
                    bot.Prefixes = prefixes;
                    bot.Intents |= GatewayIntent.DirectMessages | GatewayIntent.DirectReactions;
                })
                .Build();
        }

    }
}
