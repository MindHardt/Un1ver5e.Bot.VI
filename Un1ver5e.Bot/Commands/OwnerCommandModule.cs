using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qmmands;
using Serilog.Core;
using Serilog.Events;
using System.Text;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Commands
{
    [RequireBotOwner]
    public class OwnerCommandModule : DiscordApplicationModuleBase
    {
        [SlashCommand("o-promote")]
        [Description("(Только для главного администратора)")]
        [RequireAuthor(298097988495081472)]
        public async ValueTask<IResult> PromoteCommand(IUser user)
        {
            await Deferral(false);

            using IServiceScope scope = Bot.Services.CreateScope();
            BotContext context = scope.ServiceProvider.GetService<BotContext>()!;

            Admin newAdmin = new()
            {
                Id = user.Id
            };

            context.Admins.Add(newAdmin);
            await context.SaveChangesAsync();

            LocalEmbed embed = new()
            {
                Title = $"👑 Назначил `{user.Name}` администратором!",
                Description = "Изменения вступят в силу после перезапуска бота.",
                ThumbnailUrl = user.GetAvatarUrl()
            };

            return Response(embed);
        }


        [SlashCommand("o-shutdown")]
        [Description("(Только для администраторов)")]
        public async ValueTask ShutDownCommand()
        {
            await Context.Interaction.Response().SendMessageAsync(new LocalInteractionMessageResponse().WithContent("https://tenor.com/view/dies-cat-dead-died-gif-13827091")); //dying cat gif
            Logger.LogCritical("Shutting down because {murderer} told me to :<", Context.Author.Name);
            await Bot.Services.GetRequiredService<IHost>().StopAsync();
        }


        [SlashCommand("o-verbosity")]
        [Description("(Только для администраторов)")]
        public IResult LogLevelSwitch(
            [Name("Уровень")] LogEventLevel level)
        {
            LoggingLevelSwitch levelSwitch = Context.Services.GetRequiredService<LoggingLevelSwitch>();

            levelSwitch.MinimumLevel = level;
            return Response(new LocalInteractionMessageResponse().WithContent(level.ToString()).WithIsEphemeral());
        }


        [SlashCommand("o-sqlscript")]
        [Description("(Только для администраторов)")]
        public IResult SqlScriptCommand()
        {
            string script;

            using (IServiceScope scope = Bot.Services.CreateScope())
            {
                script = scope.ServiceProvider.GetService<BotContext>()!.GetCreateScript();
            }

            byte[] asBytes = Encoding.UTF8.GetBytes(script);

            Stream stream = new MemoryStream(asBytes)
            {
                Position = 0
            };

            LocalAttachment file = new(stream, "script.sql");

            LocalInteractionMessageResponse resp = new LocalInteractionMessageResponse()
                .WithIsEphemeral(true)
                .AddAttachment(file);

            return Response(resp);
        }


        [SlashCommand("o-listlogs")]
        [Description("(Только для администраторов)")]
        public async ValueTask<IResult> LogFilesCommand()
        {
            await Deferral(true);

            IEnumerable<Page> pages = Directory.GetFiles("./Logs")
                .Select((file, index) => $"{index}. {file[(file.LastIndexOf('\\') + 1)..]}\n")
                .ChunkAtPages(Discord.Limits.Message.MaxContentLength - 6)
                .Select(p => new Page().WithContent($"```{p}```"));

            return Pages(pages);
        }

        [SlashCommand("o-inspectlogs")]
        [Description("(Только для администраторов)")]
        public IResult InspectLogsCommand(string logFile)
        {
            Stream log = new FileStream($"./Logs/{logFile}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            LocalInteractionMessageResponse response = new LocalInteractionMessageResponse()
                .AddAttachment(new LocalAttachment(log, logFile))
                .WithIsEphemeral(true);

            return Response(response);
        }
    }
}
