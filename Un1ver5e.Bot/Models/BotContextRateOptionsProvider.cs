using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Models
{
    public class BotContextRateOptionsProvider : IRateOptionsProvider
    {
        private readonly IServiceProvider provider;

        public BotContextRateOptionsProvider(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async ValueTask<string> GetOptionAsync(Random random)
        {
            using var scope = provider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<BotContext>();

            int optionsCount = await ctx.RateOptions.CountAsync();
            int index = random.Next(optionsCount);

            string option = await ctx.RateOptions
                .OrderBy(ro => ro.Id)
                .Skip(index)
                .Select(ro => ro.Text)
                .FirstAsync();

            return option;
        }
    }
}
