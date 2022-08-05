using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Services
{
    /// <summary>
    /// Service that randomly sets bot's presence once in a <see cref="RollingInterval"/>
    /// </summary>
    public class DiscordSplashService : DiscordBotService
    {
        public static readonly TimeSpan RollingInterval = TimeSpan.FromHours(24);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Bot.WaitUntilReadyAsync(stoppingToken);

            while (stoppingToken.IsCancellationRequested == false)
            {
                using var scope = Bot.Services.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<BotContext>();

                string activity = ctx.Splashes.OrderBy(s => Guid.NewGuid()).Select(s => s.Text).First();
                await Bot.SetPresenceAsync(new LocalActivity(activity, Disqord.ActivityType.Watching), stoppingToken);
                Logger.LogInformation("Set splash to {0}", activity);

                await Task.Delay(RollingInterval, stoppingToken);
            }
        }
    }
}
