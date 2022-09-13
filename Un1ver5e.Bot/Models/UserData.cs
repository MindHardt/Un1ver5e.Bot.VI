namespace Un1ver5e.Bot.Models
{
    public record UserData
    {
        /// <summary>
        /// A Discord snowflake Id of the user.
        /// </summary>
        public ulong Id { get; set; }

        public int TagsCountLimit { get; set; } = 5;
    }
}
