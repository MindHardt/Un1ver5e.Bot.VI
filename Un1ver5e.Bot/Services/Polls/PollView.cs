using Disqord;
using Disqord.Extensions.Interactivity.Menus;
using System.Text;

namespace Un1ver5e.Bot.Services.Polls
{
    public class PollView : ViewBase
    {
        private readonly Dictionary<string, IList<IMember>> _options = new();
        private readonly bool _isAnonymous;
        private readonly string _pollTimerDisplay;
        private readonly string _header;
        public PollView(string header, bool isAnonymous, TimeSpan pollTime, string[] options) : base(default)
        {
            _isAnonymous = isAnonymous;
            _header = header;
            _pollTimerDisplay = $"Опрос завершится через: {(DateTimeOffset.Now + pollTime).ToRelativeDiscordTime()}";
            foreach (string option in options)
            {
                _options[option] = new List<IMember>();
                ButtonViewComponent button = new(AssignVote)
                {
                    Label = option
                };
                AddComponent(button);
            }
            RefreshMessage();
            _ = ScheduleEndOfPoll(pollTime);
        }

        private async Task ScheduleEndOfPoll(TimeSpan voteTime)
        {
            await Task.Delay(voteTime);

            MessageTemplate = m => m
                .WithContent("Опрос завершен!")
                .AddEmbed(GetEmbed());
            ClearComponents();
            await Menu.ApplyChangesAsync();
            await DisposeAsync();
        }
        private async ValueTask AssignVote(ButtonEventArgs e)
        {
            foreach (var votes in _options.Values)
            {
                votes.Remove(e.Member!);
            }
            _options[e.Button.Label!].Add(e.Member!);
            RefreshMessage();
        }
        private void RefreshMessage()
        {
            MessageTemplate = m => m
            .AddEmbed(GetEmbed())
            .WithContent(_pollTimerDisplay);
        }
        private LocalEmbed GetEmbed()
        {
            int totalVotes = _options.Sum(x => x.Value.Count);
            LocalEmbedField[] fields = _options
                .Select(option => new LocalEmbedField()
                {
                    Name = $"> {option.Key}",
                    Value = GetPercentageBar(option.Value.Count, totalVotes).AsCodeBlock() +
                    (_isAnonymous ? string.Empty : string.Join('\n', option.Value.Select(u => u.Name))),
                    IsInline = false
                })
                .ToArray();

            return new LocalEmbed()
                .WithTitle(_header)
                .WithFields(fields);
        }
        private static string GetPercentageBar(int current, int total)
        {
            if (current == 0) return "⬜⬜⬜⬜⬜⬜⬜⬜⬜⬜ 0 [0%]";
            if (current == total) return $"🟦🟦🟦🟦🟦🟦🟦🟦🟦🟦 {total} [100%]";

            int coloredBars = ((int)Math.Round(current * 8f / total));
            StringBuilder bars = new("🟦", 8);
            for (int i = 0; i < coloredBars; i++)
            {
                bars.Append("🟦");
            }
            for (int i = coloredBars; i < 9; i++)
            {
                bars.Append('⬜');
            }

            int percentage = current * 100 / total;

            return $"{bars} {current} [{percentage}%]";
        }
    }
}
