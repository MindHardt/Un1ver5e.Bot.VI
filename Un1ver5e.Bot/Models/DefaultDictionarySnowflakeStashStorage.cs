using Disqord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Models
{
    public class DefaultDictionarySnowflakeStashStorage : IStashStorage<Snowflake>
    {
        private readonly Dictionary<Snowflake, IStashData> _storage = new();
        public IStashData? Get(Snowflake key) => _storage.GetValueOrDefault(key);

        public IStashData? Pop(Snowflake key)
        {
            if (_storage.TryGetValue(key, out var data))
            {
                _storage.Remove(key);
                return data;
            }
            else
            {
                return null;
            }
        }

        public void Stash(Snowflake key, IStashData data) => _storage[key] = data;
    }
}
