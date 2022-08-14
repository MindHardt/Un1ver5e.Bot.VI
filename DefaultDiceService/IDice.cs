namespace Un1ver5e.Bot.Services.Dice
{
    public interface IDice
    {
        public IEnumerable<int> GetResults();
        public IThrowResult Throw(int modifyer = 0);
    }
}
