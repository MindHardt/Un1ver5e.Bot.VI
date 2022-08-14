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
using Serilog.Events;
using Serilog.Sinks.Discord;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Models;
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
                    //Here I get a logging level switch collection object which stores switches, allowing to change verbosity of different sinks during runtime
                    ILoggingLevelSwitchCollection switches = services.GetRequiredService<ILoggingLevelSwitchCollection>();

                    //CONSOLE LOGGER
                    LoggingLevelSwitch consoleSwitch = switches.AddSwitch("Console", new LoggingLevelSwitch(LogEventLevel.Information));
                    ILogger consoleLogger = new LoggerConfiguration()
                        .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                        .MinimumLevel.ControlledBy(consoleSwitch)
                        .CreateLogger();
                    logger.WriteTo.Logger(consoleLogger);

                    //FILE LOGGER
                    const string filePath = $"Logs/Log-.log";
                    LoggingLevelSwitch fileSwitch = switches.AddSwitch("File", new LoggingLevelSwitch(LogEventLevel.Information));
                    ILogger fileLogger = new LoggerConfiguration()
                        .WriteTo.File(filePath, rollingInterval: RollingInterval.Day, shared: true)
                        .MinimumLevel.ControlledBy(fileSwitch)
                        .CreateLogger();
                    logger.WriteTo.Logger(fileLogger);

                    //DISCORD WEBHOOK LOGGER
                    string? webhook = ctx.Configuration.GetRequiredSection("discord_config").GetSection("logs_webhook_url").Get<string>();
                    if (webhook is not null)
                    {
                        Match match = Regex.Match(webhook, "https://discord.com/api/webhooks/(?<Id>\\d*)/(?<Token>.*)");
                        if (match.Success == false) throw new FormatException("Bad webhook url, please check your config.");

                        ulong id = ulong.Parse(match.Groups["Id"].Value);
                        string token = match.Groups["Token"].Value;

                        LoggingLevelSwitch discordSwitch = switches.AddSwitch("Discord", new LoggingLevelSwitch(LogEventLevel.Warning));
                        ILogger discordLogger = new LoggerConfiguration()
                            .WriteTo.Discord(id, token)
                            .MinimumLevel.ControlledBy(discordSwitch)
                            .CreateLogger();
                        logger.WriteTo.Logger(discordLogger);
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

                    .AddSingleton<IDiceService, DefaultDiceService>()
                    .AddScoped<IRateOptionsProvider, BotContextRateOptionsProvider>()
                    .AddSingleton<IStashStorage<Snowflake>, DefaultDictionarySnowflakeStashStorage>()
                    .AddSingleton<ILoggingLevelSwitchCollection, DefaultDictionaryLoggingLevelSwitchCollection>()

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
    }
}
