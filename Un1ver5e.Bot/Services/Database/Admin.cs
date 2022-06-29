namespace Un1ver5e.Bot.Services.Database
{
    /// <summary>
    /// Represents a bot's admin (owner)
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Discord ID of the user.
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// Date and time when user was granted admin access.
        /// </summary>
        public DateTime PromotionTime { get; set; }

        public Admin()
        {
            PromotionTime = DateTime.UtcNow;
        }
    }
}
