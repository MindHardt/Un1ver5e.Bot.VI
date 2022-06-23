namespace Un1ver5e.Bot.Services.Dice
{
    public interface IThrowResult
    {
        public IReadOnlyCollection<int> Throws { get; }
        public int GetThrowsSum();
        public int GetCompleteSum();
    }
}
