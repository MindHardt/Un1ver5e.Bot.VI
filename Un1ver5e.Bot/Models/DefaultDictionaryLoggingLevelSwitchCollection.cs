using Serilog.Core;

namespace Un1ver5e.Bot.Models
{
    public class DefaultDictionaryLoggingLevelSwitchCollection : ILoggingLevelSwitchCollection
    {
        private Dictionary<string, LoggingLevelSwitch> _storage = new();

        public LoggingLevelSwitch AddSwitch(string name, LoggingLevelSwitch? pregeneratedValue = null)
        {
            var actualValue = pregeneratedValue ?? new LoggingLevelSwitch();
            _storage[name] = actualValue;
            return actualValue;
        }

        public IReadOnlyDictionary<string, LoggingLevelSwitch> GetAllSwitches() => _storage;

        public LoggingLevelSwitch? GetSwitch(string name) => _storage.GetValueOrDefault(name);
    }
}
