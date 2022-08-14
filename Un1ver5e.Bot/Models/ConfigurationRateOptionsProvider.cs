using Microsoft.Extensions.Configuration;

//This is not used now
namespace Un1ver5e.Bot.Models
{
    public class ConfigurationRateOptionsProvider : IRateOptionsProvider
    {
        private readonly string[] _rateOpts;
        public ConfigurationRateOptionsProvider(IConfiguration config)
        {
            _rateOpts = config.GetSection("rate_options").Get<string[]>() ?? new[] { "No splash here" };
        }
        public string GetOption(Random random) => _rateOpts.GetRandomElement(random);
    }
}
