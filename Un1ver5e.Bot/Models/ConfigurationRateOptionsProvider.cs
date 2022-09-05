using Microsoft.Extensions.Configuration;

//This is not used now
namespace Un1ver5e.Bot.Models
{
    public class ConfigurationRateOptionsProvider : IRateOptionsProvider
    {
        private readonly string[] _rateOpts;
        public ConfigurationRateOptionsProvider(IConfiguration config)
        {
            _rateOpts = config.GetSection("rate_options").Get<string[]>() ?? throw new KeyNotFoundException("Could not find rate_options in config.");
        }
        public ValueTask<string> GetOptionAsync(Random random) => ValueTask.FromResult(_rateOpts.GetRandomElement(random));
    }
}
