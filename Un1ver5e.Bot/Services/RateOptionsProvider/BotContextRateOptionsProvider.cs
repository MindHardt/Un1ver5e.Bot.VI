using Microsoft.Extensions.DependencyInjection;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Services.RateOptionsProvider
{
    public class BotContextRateOptionsProvider : IRateOptionsProvider
    {
        private readonly IServiceProvider provider;

        public BotContextRateOptionsProvider(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public string GetOption(Random random)
        {
            using var scope = provider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<BotContext>();

            int optionsCount = ctx.RateOptions.Count();
            int index = random.Next(optionsCount);

            string option = ctx.RateOptions
                .Skip(index)
                .Select(ro => ro.Text)
                .First();

            return option;
        }
    }
}
