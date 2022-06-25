using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Services.RateOptionsProvider
{
    public interface IRateOptionsProvider
    {
        public string GetOption(Random random);
    }
}
