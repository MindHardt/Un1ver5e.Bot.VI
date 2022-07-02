using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Webhook;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
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
                .UseSerilog((_, services, logger) =>
                {
                    string filePath = $"Logs/Log-.log";

                    logger
                    .MinimumLevel.ControlledBy(services.GetRequiredService<LoggingLevelSwitch>())
                    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                    .WriteTo.File(filePath, rollingInterval: RollingInterval.Day, shared: true);
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

                    .AddTransient<IRateOptionsProvider, ConfigurationRateOptionsProvider>()

                    .AddSingleton<IDiceService, DefaultDiceService>()

                    .AddWebhookClientFactory()

                    .AddDbContext<BotContext>(options =>
                    {
                        string connstr = string.Join(';', ctx.Configuration.GetSection("db_connstr_args").Get<string[]>());

                        options.UseNpgsql(connstr);
                    });

                })
                .ConfigureDiscordBot<Un1ver5eBot>((context, bot) =>
                {
                    IConfigurationSection config = context.Configuration.GetSection("discord_config");

                    string splash = config.GetSection("splashes").Get<string[]>().GetRandomElement();
                    string token = config["token"];
                    string[] prefixes = config.GetSection("prefixes").Get<string[]>();

                    bot.Activities = new LocalActivity[] { new(splash, ActivityType.Watching) };
                    bot.Token = token;
                    bot.Prefixes = prefixes;
                    bot.Intents |= GatewayIntent.DirectMessages | GatewayIntent.DirectReactions;
                })
                .Build();
        }

    }
}
