using Arklens.Entities.Properties.Attributes;
using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;
using Un1ver5e.Bot.Services.Dice;

namespace Un1ver5e.Bot.Commands
{
    public class GetDndStatsCommand : DiscordApplicationGuildModuleBase
    {
        private readonly IDiceService _diceService;

        public GetDndStatsCommand(IDiceService diceService)
        {
            _diceService = diceService;
        }

        [SlashCommand("stats")]
        [Description("Характеристики для DND, с разными вариантами получения.")]
        public IResult GetDndStats(
            [Name("Метод"), Description("Метод получения характеристики.")]
            StatsMethod method)
        {
            int[] attributes = method switch
            {
                StatsMethod.Классический =>
                    Enumerable.Repeat(0, 6)
                    .Select(i => _diceService.ThrowByQuery("3d6").Throws.Sum())
                    .ToArray(),
                StatsMethod.Стандартный =>
                    Enumerable.Repeat(0, 6)
                    .Select(i => _diceService.ThrowByQuery("4d6").Throws
                        .OrderByDescending(t => t)
                        .Take(3)
                        .Sum())
                    .ToArray(),
                StatsMethod.Героический =>
                    Enumerable.Repeat(0, 6)
                    .Select(i => _diceService.ThrowByQuery("2d6").Throws.Sum() + 6)
                    .ToArray(),
                _ => throw new NotImplementedException()
            };
            attributes = attributes
                .OrderByDescending(a => a)
                .ToArray();

            IEnumerable<AttributeStat> stats = attributes.Select(i => new Strength(i));
            string[] statDisplays = stats.Select(s => $"{s.Value} > {s.GetRawValue()}").ToArray();

            string msg = $"```{string.Join('\n', statDisplays)}```";

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddEmbed(new LocalEmbed()
                    .WithDescription(msg)
                    .WithTitle("Ваши характеристики:"));

            return Response(response);
        }

        public enum StatsMethod
        {
            Классический,
            Стандартный,
            Героический
        }
    }
}
