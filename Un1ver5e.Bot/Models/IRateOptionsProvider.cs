namespace Un1ver5e.Bot.Models
{
    public interface IRateOptionsProvider
    {
        public ValueTask<string> GetOptionAsync(Random random);
    }
}
