using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Reflection;
using Un1ver5e.Bot.Models;
using Un1ver5e.Bot.Services.Tags;
using Un1ver5e.Bot.Services.Webhooks;

namespace Un1ver5e.Bot.Services.Database
{
    public partial class BotContext : DbContext
    {
        public DbSet<RateOption> RateOptions { get; set; } = null!;
        public DbSet<Splash> Splashes { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Webhook> Webhooks { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;

        /// <summary>
        /// Gets random <see cref="Splash"/> from <see cref="Splashes"/>.
        /// </summary>
        /// <returns></returns>
        public string GetSplash() => Splashes
            .OrderBy(s => Guid.NewGuid())
            .Select(s => s.Text)
            .First();

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDbFunction(typeof(BotContext).GetRuntimeMethod(nameof(Tag.CanBeSeen), new[] { typeof(ulong) }))
                .HasTranslation(args =>
                    new SqlBinaryExpression(
                        System.Linq.Expressions.ExpressionType.Or,
                        args[0].))




            base.OnModelCreating(modelBuilder);
        }

        public string GetCreateScript() => Database.GenerateCreateScript();
    }
}
