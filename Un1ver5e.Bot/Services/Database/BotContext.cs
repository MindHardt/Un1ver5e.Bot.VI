using Microsoft.EntityFrameworkCore;
using Un1ver5e.Bot.Models;

namespace Un1ver5e.Bot.Services.Database
{
    public partial class BotContext : DbContext
    {
        public DbSet<RateOption> RateOptions { get; set; } = null!;
        public DbSet<Splash> Splashes { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Webhook> Webhooks { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<UserData> Users { get; set; } = null!;

        /// <summary>
        /// Gets random <see cref="Splash"/> from <see cref="Splashes"/>.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<string> GetSplashAsync() => await Splashes
            .OrderBy(s => EF.Functions.Random())
            .Select(s => s.Text)
            .FirstAsync();

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        public string GetCreateScript() => Database.GenerateCreateScript();
    }
}
