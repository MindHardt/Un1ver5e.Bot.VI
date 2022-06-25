using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Services.RateOptionsProvider
{
    public class DefaultRateOptionsProvider : IRateOptionsProvider
    {
        private string[] _rateOpts;
        public DefaultRateOptionsProvider(IConfiguration config)
        {
            _rateOpts = config.GetSection("rate_options").Get<string[]>();
        }
        public string GetOption(Random random) => _rateOpts.GetRandomElement(random);
    }
}
