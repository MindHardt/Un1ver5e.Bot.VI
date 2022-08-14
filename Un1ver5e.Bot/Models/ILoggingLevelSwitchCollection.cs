using Serilog.Core;

namespace Un1ver5e.Bot.Models
{
    internal interface ILoggingLevelSwitchCollection
    {
        /// <summary>
        /// Gets a <see cref="LoggingLevelSwitch"/> object related to <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LoggingLevelSwitch? GetSwitch(string name);
        /// <summary>
        /// Gets a collection of all switches with their names.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, LoggingLevelSwitch> GetAllSwitches();
        /// <summary>
        /// Adds <paramref name="pregeneratedValue"/> to a collection and saves it under <paramref name="name"/>. If <paramref name="pregeneratedValue"/> is <see langword="null"/> then a new <see cref="LoggingLevelSwitch"/> object is created.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LoggingLevelSwitch AddSwitch(string name, LoggingLevelSwitch? pregeneratedValue = null);
    }
}
