using Microsoft.EntityFrameworkCore;
using Un1ver5e.Bot.Services.Tags;
using Un1ver5e.Bot.Services.Webhooks;

namespace Un1ver5e.Bot.Services.Database
{
    public partial class BotContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Webhook> Webhooks { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public string GetCreateScript() => Database.GenerateCreateScript();
    }
}
